using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine.Assertions;

namespace Assets.Scripts.ECS
{
	[DisableAutoCreation]
	public class CharacterTriggerSystem : JobComponentSystem
	{
		EntityManager m_EntityManager;
		BuildPhysicsWorld m_BuildPhysicsWorldSystem;
		ExportPhysicsWorld m_ExportPhysicsWorldSystem;
		EndFramePhysicsSystem m_EndFramePhysicsSystem;
		EndSimulationEntityCommandBufferSystem m_EntityCommandBufferSystem;

		EntityQuery m_CharacterControllersGroup;
		EntityQuery m_OverlappingGroup;
		EntityQuery m_TriggerVolumeGroup;

		protected override void OnCreate()
		{
			m_EntityManager = World.EntityManager;
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
			m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();
			m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
			m_EntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

			EntityQueryDesc query = new EntityQueryDesc
			{
				All = new ComponentType[]
				{
				typeof(PhysicsCollider),
				typeof(CharacterDataComponent),
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
			var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);

			var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
			var characterDataType = GetArchetypeChunkComponentType<CharacterDataComponent>();
			var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
			var translationType = GetArchetypeChunkComponentType<Translation>();
			var rotationType = GetArchetypeChunkComponentType<Rotation>();
			var tickDuration = GetSingleton<WorldTime>().TickDuration;

			var CharacterIndex = new NativeArray<int>(1, Allocator.TempJob);
			var triggerEntitiesIndex = new NativeArray<int>(1, Allocator.TempJob);
			var triggerEntities = new NativeArray<Entity>(1, Allocator.TempJob);
			var ccJob = new GetTriggerOverlappingJob()
			{
				Chunks = chunks,
				// Archetypes
				PhysicsColliderType = physicsColliderType,
				CharacterDataType = characterDataType,
				UserCommandType = userCommandType,
				TranslationType = translationType,
				RotationType = rotationType,
				// Input
				DeltaTime = tickDuration,
				PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
				VolumeEntities = m_TriggerVolumeGroup.ToEntityArray(Allocator.TempJob),

				Character = CharacterIndex,
				pCounter = triggerEntitiesIndex,
				TriggerEntities = triggerEntities,
			};

			inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
			inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);
			//inputDeps.Complete();

			var overlappingGroup = GetComponentDataFromEntity<OverlappingTriggerComponent>(true);
			var triggerDataGroup = GetComponentDataFromEntity<TriggerDataComponent>(true);
			JobHandle addNewJobHandle = new AddNewOverlappingJob
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
				Character = CharacterIndex,
				TriggerDataGroup = triggerDataGroup,
				TriggerEntities = triggerEntities,
				TriggerEntitiesCount = triggerEntitiesIndex,
				OverlappingGroup = overlappingGroup,
			}.Schedule(inputDeps);
			m_EntityCommandBufferSystem.AddJobHandleForProducer(addNewJobHandle);
			addNewJobHandle.Complete();

			JobHandle removeOldJobHandle = new RemoveOldOverlappingJob
			{
				CommandBuffer = m_EntityCommandBufferSystem.CreateCommandBuffer(),
				Character = CharacterIndex,
				TriggerDataGroup = triggerDataGroup,
				TriggerEntities = triggerEntities,
				TriggerEntitiesCount = triggerEntitiesIndex,
				OverlappingEntities = m_OverlappingGroup.ToEntityArray(Allocator.TempJob),
			}.Schedule(inputDeps);
			m_EntityCommandBufferSystem.AddJobHandleForProducer(removeOldJobHandle);

			inputDeps = JobHandle.CombineDependencies(addNewJobHandle, removeOldJobHandle);

			return inputDeps;
		}

		[BurstCompile]
		struct GetTriggerOverlappingJob : IJobChunk
		{
			// Chunks can be deallocated at this point
			[DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;

			public float DeltaTime;

			[ReadOnly]
			public PhysicsWorld PhysicsWorld;

			[ReadOnly] public ArchetypeChunkComponentType<Translation> TranslationType;
			[ReadOnly] public ArchetypeChunkComponentType<Rotation> RotationType;
			[ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
			[ReadOnly] public ArchetypeChunkComponentType<CharacterDataComponent> CharacterDataType;
			[ReadOnly] public ArchetypeChunkComponentType<UserCommand> UserCommandType;

			[NativeFixedLength(1)] public NativeArray<int> Character;
			[NativeFixedLength(1)] public NativeArray<int> pCounter;
			[NativeFixedLength(1)] public NativeArray<Entity> TriggerEntities;
			[DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> VolumeEntities;

			public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
			{
				var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
				var chunkCharacterDataData = chunk.GetNativeArray(CharacterDataType);
				var chunkUserCommandData = chunk.GetNativeArray(UserCommandType);
				var chunkTranslationData = chunk.GetNativeArray(TranslationType);
				var chunkRotationData = chunk.GetNativeArray(RotationType);

				const int maxQueryHits = 128;
				var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
				//var castHits = new NativeArray<ColliderCastHit>(maxQueryHits, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				var constraints = new NativeArray<SurfaceConstraintInfo>(4 * maxQueryHits, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

				for (int i = 0; i < chunk.Count; i++)
				{
					var collider = chunkPhysicsColliderData[i];
					var characterData = chunkCharacterDataData[i];
					var userCommand = chunkUserCommandData[i];
					var translation = chunkTranslationData[i];
					var rotation = chunkRotationData[i];

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

					var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(characterData.Entity);

					PhysicsWorld.CalculateDistance(input, ref distanceHits);

					var triggerIndex = CheckTrigger(PhysicsWorld, VolumeEntities, selfRigidBodyIndex, rotation.Value, distanceHits);
					pCounter[0] = 0;
					if (triggerIndex >= 0)
					{
						Character[0] = characterData.Entity.Index;
						pCounter[0] = 1;
						TriggerEntities[0] = PhysicsWorld.Bodies[distanceHits[triggerIndex].RigidBodyIndex].Entity;
					}
				}
			}
		}

		struct AddNewOverlappingJob : IJob
		{
			public EntityCommandBuffer CommandBuffer;

			[ReadOnly] public NativeArray<Entity> TriggerEntities;
			[NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
			[NativeFixedLength(1)] [ReadOnly] public NativeArray<int> Character;
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
						CommandBuffer.AddComponent(overlappingEntity, new OverlappingTriggerComponent());
						CommandBuffer.AddComponent(overlappingEntity, new OnTriggerEnter());
						switch ((TriggerVolumeType)triggerComponent.VolumeType)
						{
							//TODO 根据不同的trigger添加不同的组件
							case TriggerVolumeType.None:
							default:
								break;
						}
					}
				}
			}
		}

		struct RemoveOldOverlappingJob : IJob
		{
			public EntityCommandBuffer CommandBuffer;

			[DeallocateOnJobCompletion] [ReadOnly] public NativeArray<Entity> TriggerEntities;
			[DeallocateOnJobCompletion] [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> TriggerEntitiesCount;
			[DeallocateOnJobCompletion] [NativeFixedLength(1)] [ReadOnly] public NativeArray<int> Character;
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

					var triggerComponent = TriggerDataGroup[entity];

					if (!isTriggered)
					{
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
