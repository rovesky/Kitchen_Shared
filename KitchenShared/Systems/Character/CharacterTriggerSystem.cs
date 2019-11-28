using FootStone.ECS;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace FootStone.Kitchen
{
	[DisableAutoCreation]
	public class CharacterTriggerSystem : JobComponentSystem
	{
		BuildPhysicsWorld m_BuildPhysicsWorldSystem;
		ExportPhysicsWorld m_ExportPhysicsWorldSystem;
		EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

		EntityQuery m_CharacterControllersGroup;
		EntityQuery m_OverlappingGroup;
		EntityQuery m_TriggerVolumeGroup;

		protected override void OnCreate()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
			m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();
			m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

			EntityQueryDesc query = new EntityQueryDesc
			{
				All = new ComponentType[]
				{
				typeof(PhysicsCollider),
				typeof(CharacterMove),
				typeof(UserCommand),
				typeof(Translation),
				typeof(Rotation),
				}
			};
			m_CharacterControllersGroup = GetEntityQuery(query);
			m_OverlappingGroup = GetEntityQuery(typeof(OverlappingTriggerComponent));
			m_TriggerVolumeGroup = GetEntityQuery(typeof(TriggerDataComponent));
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var entities = m_CharacterControllersGroup.ToEntityArray(Allocator.TempJob);

			var physicsColliderGroup = GetComponentDataFromEntity<PhysicsCollider>(true);
			var userCommandGroup = GetComponentDataFromEntity<UserCommand>(true);
			var translationGroup = GetComponentDataFromEntity<Translation>(true);
			var rotationGroup = GetComponentDataFromEntity<Rotation>(true);
			var tickDuration = GetSingleton<WorldTime>().TickDuration;

			var triggerEntitiesCount = new NativeArray<int>(1, Allocator.TempJob);
			var characters = new NativeArray<int>(entities.Length, Allocator.TempJob);
			var triggerEntities = new NativeArray<Entity>(entities.Length, Allocator.TempJob);
			var ccJob = new GetTriggerOverlappingJob()
			{
				Entities = entities,
				// Archetypes
				PhysicsColliderGroup = physicsColliderGroup,
				UserCommandGroup = userCommandGroup,
				TranslationGroup = translationGroup,
				RotationGroup = rotationGroup,
				// Input
				DeltaTime = tickDuration,
				PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
				VolumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob),

				pCounter = triggerEntitiesCount,
				Characters = characters,
				TriggerEntities = triggerEntities,
			};

			inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
			inputDeps = ccJob.Schedule(inputDeps);

			var overlappingGroup = GetComponentDataFromEntity<OverlappingTriggerComponent>(true);
			var triggerDataGroup = GetComponentDataFromEntity<TriggerDataComponent>(true);
			JobHandle addNewJobHandle = new AddNewOverlappingJob
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
				TriggerEntitiesCount = triggerEntitiesCount,
				Characters = characters,
				TriggerDataGroup = triggerDataGroup,
				TriggerEntities = triggerEntities,
				OverlappingGroup = overlappingGroup,
			}.Schedule(inputDeps);
			m_EntityCommandBufferSystem.AddJobHandleForProducer(addNewJobHandle);

			JobHandle removeOldJobHandle = new RemoveOldOverlappingJob
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
				TriggerDataGroup = triggerDataGroup,
				TriggerEntities = triggerEntities,
				TriggerEntitiesCount = triggerEntitiesCount,
				OverlappingEntities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob),
			}.Schedule(inputDeps);
			m_EntityCommandBufferSystem.AddJobHandleForProducer(removeOldJobHandle);
			
			inputDeps = JobHandle.CombineDependencies(addNewJobHandle, removeOldJobHandle);
			inputDeps.Complete();

			m_EntityCommandBufferSystem.Update();

			characters.Dispose();
			triggerEntitiesCount.Dispose();
			triggerEntities.Dispose();

			return inputDeps;
		}

		//[BurstCompile]
		struct GetTriggerOverlappingJob : IJob
		{
			// Chunks can be deallocated at this point
			[DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> Entities;

			public float DeltaTime;

			[ReadOnly]
			public PhysicsWorld PhysicsWorld;
			[ReadOnly] public ComponentDataFromEntity<Translation> TranslationGroup;
			[ReadOnly] public ComponentDataFromEntity<Rotation> RotationGroup;
			[ReadOnly] public ComponentDataFromEntity<PhysicsCollider> PhysicsColliderGroup;
			[ReadOnly] public ComponentDataFromEntity<UserCommand> UserCommandGroup;

			[NativeFixedLength(1)] public NativeArray<int> pCounter;
			public NativeArray<int> Characters;
			public NativeArray<Entity> TriggerEntities;
			[DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> VolumeEntities;

			public unsafe void Execute()
			{
				pCounter[0] = 0;
				var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
				
				for (int i = 0; i < Entities.Length; i++)
				{
					var entity = Entities[i];
					var collider = PhysicsColliderGroup[entity];
					var userCommand = UserCommandGroup[entity];
					var translation = TranslationGroup[entity];
					var rotation = RotationGroup[entity];

					// Collision filter must be valid
					Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

					// Character transform
					RigidTransform transform = new RigidTransform
					{
						pos = translation.Value,
						rot = rotation.Value
					};

					ColliderDistanceInput input = new ColliderDistanceInput()
					{
						MaxDistance = 0.7f,
						Transform = transform,
						Collider = collider.ColliderPtr
					};

					var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity);

					distanceHits.Clear();
					PhysicsWorld.CalculateDistance(input, ref distanceHits);

					var triggerIndex = CheckTrigger(PhysicsWorld, VolumeEntities, selfRigidBodyIndex, rotation.Value, distanceHits);

					if (triggerIndex >= 0)
					{
						var triggerEntity = PhysicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;
						if (!TriggerEntities.Contains(triggerEntity))
						{
							Characters[pCounter[0]] = entity.Index;
							TriggerEntities[pCounter[0]] = triggerEntity;
							pCounter[0]++;
						}
					}
				}
			}
		}

		struct AddNewOverlappingJob : IJob
		{
			public EntityCommandBuffer CommandBuffer;

			[NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
			[ReadOnly] public NativeArray<int> Characters;
			[ReadOnly] public NativeArray<Entity> TriggerEntities;
			[ReadOnly] public ComponentDataFromEntity<OverlappingTriggerComponent> OverlappingGroup;
			[ReadOnly] public ComponentDataFromEntity<TriggerDataComponent> TriggerDataGroup;

			public void Execute()
			{
				for (int i = 0; i < TriggerEntitiesCount[0]; i++)
				{
					var overlappingEntity = TriggerEntities[i];
					if (!OverlappingGroup.Exists(overlappingEntity))
					{
						var triggerComponent = TriggerDataGroup[overlappingEntity];
						CommandBuffer.AddComponent(overlappingEntity, new OverlappingTriggerComponent { TriggerEntity = Characters[i] });
						CommandBuffer.AddComponent(overlappingEntity, new OnTriggerEnter());
						switch ((TriggerVolumeType)triggerComponent.VolumeType)
						{
							//TODO 根据不同的trigger添加不同的组件
							case TriggerVolumeType.None:
							default:
								break;
						}
					}
					else
					{
						var overlappingTriggerComponent = OverlappingGroup[overlappingEntity];
						if (overlappingTriggerComponent.TriggerEntity != Characters[i])
						{
							overlappingTriggerComponent.TriggerEntity = Characters[i];
							CommandBuffer.SetComponent(overlappingEntity, overlappingTriggerComponent);
						}
					}
				}
			}
		}

		struct RemoveOldOverlappingJob : IJob
		{
			public EntityCommandBuffer CommandBuffer;

			[NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
			[ReadOnly] public NativeArray<Entity> TriggerEntities;
			[DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> OverlappingEntities;
			[ReadOnly] public ComponentDataFromEntity<TriggerDataComponent> TriggerDataGroup;

			public void Execute()
			{
				for (int index = 0; index < OverlappingEntities.Length; index++)
				{
					var entity = OverlappingEntities[index];

					var isTriggered = false;
					for (int i = 0; i < TriggerEntitiesCount[0]; ++i)
					{
						if (TriggerEntities[i] == entity)
						{
							isTriggered = true;
							break;
						}
					}

					if (!isTriggered)
					{
						var triggerComponent = TriggerDataGroup[entity];

						//TODO 根据类型删除对应组件
						switch ((TriggerVolumeType)triggerComponent.VolumeType)
						{
							case TriggerVolumeType.None:
							default:
								break;
						}
						CommandBuffer.RemoveComponent<OverlappingTriggerComponent>(entity);
						CommandBuffer.AddComponent(entity, new OnTriggerExit());
					}
				}
			}
		}

		private static unsafe int CheckTrigger(PhysicsWorld world, NativeArray<Entity> volumeEntities, int selfRigidBodyIndex, quaternion selfRotation, NativeList<DistanceHit> distanceHits)
		{
			int triggerIndex = -1;
			for (int i = 0; i < distanceHits.Length; i++)
			{
				DistanceHit hit = distanceHits[i];
				if (hit.RigidBodyIndex != selfRigidBodyIndex)
				{
					var e = world.Bodies[hit.RigidBodyIndex].Entity;
					if (volumeEntities.Contains(e))
					{
						if (triggerIndex < 0)
						{
							triggerIndex = i;
						}
						else
						{
							if (distanceHits[triggerIndex].Distance > hit.Distance)
							{
								triggerIndex = i;
							}
						}
					}
				}
			}
			return triggerIndex;
		}
	}
}
