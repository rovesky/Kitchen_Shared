using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine.Assertions;

namespace Assets.Scripts.ECS
{
	[DisableAutoCreation]
	public class CharacterMoveSystem : JobComponentSystem
	{
		BuildPhysicsWorld m_BuildPhysicsWorldSystem;
		ExportPhysicsWorld m_ExportPhysicsWorldSystem;
		EndFramePhysicsSystem m_EndFramePhysicsSystem;

		EntityQuery m_CharacterControllersGroup;

		protected override void OnCreate()
		{
			m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
			m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<ExportPhysicsWorld>();
			m_EndFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();

			EntityQueryDesc query = new EntityQueryDesc
			{
				All = new ComponentType[]
				{
				typeof(PhysicsCollider),
				typeof(CharacterDataComponent),
				typeof(MoveInput),
				typeof(UserCommand),
				typeof(EntityPredictData),
				}
			};
			m_CharacterControllersGroup = GetEntityQuery(query);
		}

		protected override JobHandle OnUpdate(JobHandle inputDeps)
		{
			var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);

			var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
			var characterDataType = GetArchetypeChunkComponentType<CharacterDataComponent>();
			var moveInputType = GetArchetypeChunkComponentType<MoveInput>();
			var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
			var predictDataType = GetArchetypeChunkComponentType<EntityPredictData>();
			var tickDuration = GetSingleton<WorldTime>().TickDuration;

			var ccJob = new CharacterMoveControlJob()
			{
				Chunks = chunks,
				// Archetypes
				PhysicsColliderType = physicsColliderType,
				CharacterDataType = characterDataType,
				MoveInputType = moveInputType,
				UserCommandType = userCommandType,
				PredictDataType = predictDataType,
				// Input
				DeltaTime = tickDuration,
				PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
			};

			inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
			inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);

			return inputDeps;
		}

		[BurstCompile]
		struct CharacterMoveControlJob : IJobChunk
		{
			// Chunks can be deallocated at this point
			[DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;

			public float DeltaTime;

			[ReadOnly]
			public PhysicsWorld PhysicsWorld;

			public ArchetypeChunkComponentType<EntityPredictData> PredictDataType;
			[ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;
			[ReadOnly] public ArchetypeChunkComponentType<CharacterDataComponent> CharacterDataType;
			[ReadOnly] public ArchetypeChunkComponentType<MoveInput> MoveInputType;
			[ReadOnly] public ArchetypeChunkComponentType<UserCommand> UserCommandType;

			public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
			{
				var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
				var chunkCharacterDataData = chunk.GetNativeArray(CharacterDataType);
				var chunkMoveInputData = chunk.GetNativeArray(MoveInputType);
				var chunkUserCommandData = chunk.GetNativeArray(UserCommandType);
				var chunkPredictDataData = chunk.GetNativeArray(PredictDataType);

				const int maxQueryHits = 128;
				var distanceHits = new NativeList<DistanceHit>(Allocator.Temp);
				//var castHits = new NativeArray<ColliderCastHit>(maxQueryHits, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
				var constraints = new NativeArray<SurfaceConstraintInfo>(4 * maxQueryHits, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

				for (int i = 0; i < chunk.Count; i++)
				{
					var collider = chunkPhysicsColliderData[i];
					var characterData = chunkCharacterDataData[i];
					var moveInput = chunkMoveInputData[i];
					var userCommand = chunkUserCommandData[i];
					var predictData = chunkPredictDataData[i];

					// Collision filter must be valid
					Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

					// Character transform
					RigidTransform transform = new RigidTransform
					{
						pos = predictData.position,
						rot = predictData.rotation
					};

					ColliderDistanceInput input = new ColliderDistanceInput()
					{
						MaxDistance = 10.3f,
						Transform = transform,
						Collider = collider.ColliderPtr
					};

					//var selfRigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(characterData.Entity);

					PhysicsWorld.CalculateDistance(input, ref distanceHits);

					int numConstraints = 0;
					float skinWidth = characterData.SkinWidth;
					CharacterControllerUtilities.CheckSupport(PhysicsWorld, skinWidth, distanceHits, ref constraints, out numConstraints);

					float3 desiredVelocity = userCommand.targetPos * moveInput.Speed;

					// Solve
					float3 newVelocity = desiredVelocity;
					float3 newPosition = transform.pos;
					float remainingTime = DeltaTime;
					float3 up = math.up();
					SimplexSolver.Solve(PhysicsWorld, remainingTime, up, numConstraints, ref constraints, ref newPosition, ref newVelocity, out float integratedTime);

					predictData.position = newPosition;
					chunkPredictDataData[i] = predictData;
				}
			}
		}
	}
}
