using FootStone.ECS;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Extensions;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Assertions;
using static FootStone.Kitchen.CharacterMoveUtilities;

namespace FootStone.Kitchen
{



    [DisableAutoCreation]
    public class CharacterMoveSystem : JobComponentSystem
    {
        private const float KDefaultTau = 0.4f;
        private const float KDefaultDamping = 0.9f;

        private BuildPhysicsWorld m_BuildPhysicsWorldSystem;

        private EntityQuery m_CharacterControllersGroup;
        private KitchenEndFramePhysicsSystem m_EndFramePhysicsSystem;
        private KitchenExportPhysicsWorld m_ExportPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.GetOrCreateSystem<BuildPhysicsWorld>();
            m_ExportPhysicsWorldSystem = World.GetOrCreateSystem<KitchenExportPhysicsWorld>();
            m_EndFramePhysicsSystem = World.GetOrCreateSystem<KitchenEndFramePhysicsSystem>();

            var query = new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(ServerEntity),
                    typeof(CharacterMoveSetting),
                    typeof(UserCommand),
                    typeof(CharacterMovePredictedState),
                    typeof(TransformPredictedState),
                    typeof(PhysicsCollider)
                }
            };
            m_CharacterControllersGroup = GetEntityQuery(query);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_CharacterControllersGroup.CalculateEntityCount() == 0)
                return inputDeps;

            var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);

            var characterMoveType = GetArchetypeChunkComponentType<CharacterMoveSetting>();
            var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
            var movePredictedType = GetArchetypeChunkComponentType<CharacterMovePredictedState>();
            var transformType = GetArchetypeChunkComponentType<TransformPredictedState>();
            var velocityType = GetArchetypeChunkComponentType<VelocityPredictedState>();
            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
            var entityType = GetArchetypeChunkEntityType();

            var deferredImpulses = new NativeStream(chunks.Length, Allocator.TempJob);
            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var ccJob = new CharacterControllerJob
            {
                EntityType = entityType,
                // Archetypes
                CharacterMoveType = characterMoveType,
                UserCommandComponentType = userCommandType,
                CharacterMovePredictedType = movePredictedType,
                PhysicsColliderType = physicsColliderType,
                TransformType = transformType,
                VelocityType = velocityType,

                // Input
                DeltaTime = tickDuration,
                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
                DeferredImpulseWriter = deferredImpulses.AsWriter()
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);

            var applyJob = new ApplyDefferedPhysicsUpdatesJob
            {
                Chunks = chunks,
                DeferredImpulseReader = deferredImpulses.AsReader(),
                PhysicsMassData = GetComponentDataFromEntity<PhysicsMass>(),
                TransformPredictedData = GetComponentDataFromEntity<TransformPredictedState>(),
                VelocityPredictedData = GetComponentDataFromEntity<VelocityPredictedState>(),
            };

            inputDeps = applyJob.Schedule(inputDeps);
            var disposeHandle = deferredImpulses.Dispose(inputDeps);

            // Must finish all jobs before physics step end
            m_EndFramePhysicsSystem.HandlesToWaitFor.Add(disposeHandle);

            return inputDeps;
        }

        [BurstCompile]
        private struct CharacterControllerJob : IJobChunk
        {
            public float DeltaTime;

            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;

            public ArchetypeChunkComponentType<CharacterMovePredictedState> CharacterMovePredictedType;
            public ArchetypeChunkComponentType<TransformPredictedState> TransformType;
            public ArchetypeChunkComponentType<VelocityPredictedState> VelocityType;

            [ReadOnly] public ArchetypeChunkComponentType<CharacterMoveSetting> CharacterMoveType;
            [ReadOnly] public ArchetypeChunkComponentType<UserCommand> UserCommandComponentType;
            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;

            // Stores impulses we wish to apply to dynamic bodies the character is interacting with.
            // This is needed to avoid race conditions when 2 characters are interacting with the
            // same body at the same time.
            public NativeStream.Writer DeferredImpulseWriter;


            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var up = math.up();

                var chunkEntityData = chunk.GetNativeArray(EntityType);
                var chunkMoveSettingData = chunk.GetNativeArray(CharacterMoveType);
                var chunkTransformData = chunk.GetNativeArray(TransformType);
                var chunkVelocityData = chunk.GetNativeArray(VelocityType);
                var chunkUserCommand = chunk.GetNativeArray(UserCommandComponentType);
                var chunkMovePredictedData = chunk.GetNativeArray(CharacterMovePredictedType);
                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);

                DeferredImpulseWriter.BeginForEachIndex(chunkIndex);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = chunkEntityData[i];
                    var moveSetting = chunkMoveSettingData[i];
                    var userCommand = chunkUserCommand[i];
                    var movePredictedData = chunkMovePredictedData[i];
                    var collider = chunkPhysicsColliderData[i];
                    var transformData = chunkTransformData[i];
                    var velocityData = chunkVelocityData[i];

                    // Collision filter must be valid
                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

                //    velocityData.Linear = new float3(4,0,0);
                    // Character step input
                    var stepInput = new CharacterControllerStepInput
                    {
                        World = PhysicsWorld,
                        DeltaTime = DeltaTime,
                        Up = math.up(),
                        Gravity = moveSetting.Gravity,
                        MaxIterations = moveSetting.MaxIterations,
                        Tau = KDefaultTau,
                        Damping = KDefaultDamping,
                        SkinWidth = moveSetting.SkinWidth,
                        ContactTolerance = moveSetting.ContactTolerance,
                        MaxSlope = moveSetting.MaxSlope,
                        RigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity),
                        CurrentVelocity = velocityData.Linear,
                     //   CurrentVelocity = float3.zero,
                        MaxMovementSpeed = moveSetting.MaxVelocity
                    };

              //      FSLog.Info($"stepInput.CurrentVelocity:{stepInput.CurrentVelocity}");

                    // Character transform
                    var transform = new RigidTransform()
                    {
                        pos = transformData.Position,
                        rot = transformData.Rotation
                    };

                    var moveInternalState = new MoveInternalState()
                    {
                        SupportedState = CharacterSupportState.Unsupported,
                        IsJumping = false
                    };

                    // Check support
                    CheckSupport(ref PhysicsWorld, ref collider, stepInput, transform, moveSetting.MaxSlope,
                        out moveInternalState.SupportedState, out var surfaceNormal, out var surfaceVelocity);

                    // User input
                    // var desiredVelocity = velocityData.Linear;
                    HandleUserInput(moveSetting, userCommand, stepInput.Up, surfaceVelocity,
                        ref movePredictedData, out var desiredVelocity, ref moveInternalState);
                    
                     desiredVelocity += velocityData.Linear;

                //    FSLog.Info($"stepInput.CurrentVelocity:{stepInput.CurrentVelocity},desiredVelocity:{desiredVelocity}");
                    // Calculate actual velocity with respect to surface
                    if (moveInternalState.SupportedState == CharacterSupportState.Supported)
                    {
                        CalculateMovement(transformData.Rotation, stepInput.Up, moveInternalState.IsJumping,
                            stepInput.CurrentVelocity, desiredVelocity, surfaceNormal, surfaceVelocity,
                            out velocityData.Linear);
                        FSLog.Info($"velocityData.Linear:{velocityData.Linear}," +
                                   $"stepInput.CurrentVelocity:{stepInput.CurrentVelocity}");
                    }
                    else
                        velocityData.Linear = desiredVelocity ;

                    // World collision + integrate
                    CollideAndIntegrate(stepInput, moveSetting.CharacterMass, moveSetting.AffectsPhysicsBodies > 0,
                        collider.ColliderPtr, ref transform, ref velocityData.Linear, ref DeferredImpulseWriter);
                    //  FSLog.Info($"end  velocityData.Linear:{ velocityData.Linear}");
                    // Write back and orientation integration
                    transformData.Position = transform.pos;
                    // character rotate

                 
                    if (math.distancesq(userCommand.TargetDir, float3.zero) > 0.0001f)
                    {
                       // var dir = Vector3.SqrMagnitude(velocityData.Linear) < 0.001f 
                           // ? Vector3.zero :(Vector3) math.normalize(velocityData.Linear);

                        var fromRotation = transformData.Rotation;
                        var toRotation = quaternion.LookRotationSafe(userCommand.TargetDir, up);
                        var angle = Quaternion.Angle(fromRotation, toRotation);
                        transformData.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                            math.abs(angle - 180.0f) < float.Epsilon
                                ? -moveSetting.RotationVelocity
                                : moveSetting.RotationVelocity);
                    }

                    // Write back to chunk data
                    {
                        chunkMovePredictedData[i] = movePredictedData;
                        chunkTransformData[i] = transformData;
                        chunkVelocityData[i] = velocityData;
                    }
                }

                DeferredImpulseWriter.EndForEachIndex();
            }


            private void HandleUserInput(CharacterMoveSetting ccComponentData, UserCommand command, float3 up,
                float3 surfaceVelocity, ref CharacterMovePredictedState ccPredictedState, out float3 linearVelocity,
                ref MoveInternalState moveInternalState)
            {
                // Reset jumping state and unsupported velocity
                if (moveInternalState.SupportedState == CharacterSupportState.Supported)
                {
                    moveInternalState.IsJumping = false;
                    ccPredictedState.UnsupportedVelocity = float3.zero;
                }

                var shouldJump = command.Buttons.IsSet(UserCommand.Button.Jump) &&
                                 moveInternalState.SupportedState == CharacterSupportState.Supported;

                if (shouldJump)
                {
                    // Add jump speed to surface velocity and make character unsupported
                    moveInternalState.IsJumping = true;
                    moveInternalState.SupportedState = CharacterSupportState.Unsupported;
                    ccPredictedState.UnsupportedVelocity = surfaceVelocity + ccComponentData.JumpUpwardsVelocity * up;
                }
                else if (moveInternalState.SupportedState != CharacterSupportState.Supported)
                {
                    // Apply gravity
                    ccPredictedState.UnsupportedVelocity += ccComponentData.Gravity * DeltaTime;
                }

                // If unsupported then keep jump and surface momentum
                linearVelocity = (float3) command.TargetDir * ccComponentData.MaxVelocity +
                                 (moveInternalState.SupportedState != CharacterSupportState.Supported
                                     ? ccPredictedState.UnsupportedVelocity
                                     : float3.zero);
            }

            private static void CalculateMovement(quaternion currentRotationAngle, float3 up, bool isJumping,
                float3 currentVelocity, float3 desiredVelocity, float3 surfaceNormal, float3 surfaceVelocity,
                out float3 linearVelocity)
            {
                var forward = math.forward(currentRotationAngle);

                Rotation surfaceFrame;
                float3 binorm;
                {
                    binorm = math.cross(forward, up);
                    binorm = math.normalize(binorm);

                    var tangent = math.cross(binorm, surfaceNormal);
                    tangent = math.normalize(tangent);

                    binorm = math.cross(tangent, surfaceNormal);
                    binorm = math.normalize(binorm);

                    surfaceFrame.Value = new quaternion(new float3x3(binorm, tangent, surfaceNormal));
                }

                var relative = currentVelocity - surfaceVelocity;
                relative = math.rotate(math.inverse(surfaceFrame.Value), relative);

              //  FSLog.Info($"relative1:{relative}");
                float3 diff;
                {
                    var sideVec = math.cross(forward, up);
                    var fwd = math.dot(desiredVelocity, forward);
                    var side = math.dot(desiredVelocity, sideVec);
                    var len = math.length(desiredVelocity);
                    var desiredVelocitySf = new float3(-side, -fwd, 0.0f);
                    desiredVelocitySf = math.normalizesafe(desiredVelocitySf, float3.zero);
                    desiredVelocitySf *= len;
                    diff = desiredVelocitySf - relative;
                }

                relative += diff;
             //   FSLog.Info($"relative2:{relative}");
                linearVelocity = math.rotate(surfaceFrame.Value, relative) + surfaceVelocity +
                                 (isJumping ? math.dot(desiredVelocity, up) * up : float3.zero);
            }
        }

        // [BurstCompile]
        private struct ApplyDefferedPhysicsUpdatesJob : IJob
        {
            // Chunks can be deallocated at this point
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;
            public NativeStream.Reader DeferredImpulseReader;
            public ComponentDataFromEntity<PhysicsMass> PhysicsMassData;
            public ComponentDataFromEntity<VelocityPredictedState> VelocityPredictedData;
            public ComponentDataFromEntity<TransformPredictedState> TransformPredictedData;

            public void Execute()
            {
                var index = 0;
                var maxIndex = DeferredImpulseReader.ForEachCount;
                DeferredImpulseReader.BeginForEachIndex(index++);
                while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex)
                    DeferredImpulseReader.BeginForEachIndex(index++);

                while (DeferredImpulseReader.RemainingItemCount > 0)
                {
                    // Read the data
                    var impulse = DeferredImpulseReader.Read<DeferredCharacterControllerImpulse>();
                    while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex)
                        DeferredImpulseReader.BeginForEachIndex(index++);

                    if(!PhysicsMassData.HasComponent(impulse.Entity))
                        continue;

                    var pm = PhysicsMassData[impulse.Entity];
                    var ep = VelocityPredictedData[impulse.Entity];
                    var transform = TransformPredictedData[impulse.Entity];

                    //   FSLog.Info($"impulse.Entity:{impulse.Entity},pm:{pm.InverseMass}");

                    // Don't apply on kinematic bodies
                    if (!(pm.InverseMass > 0.0f))
                    {
                        ep.Linear = new float3(6,0,0);
                    }
                    //   continue;
                    else
                    {
                        var rigidTransform = new PhysicsVelocity()
                        {
                            Linear = ep.Linear,
                            Angular = ep.Angular
                        };
                        // Apply impulse
                        rigidTransform.ApplyImpulse(pm, new Translation() {Value = transform.Position}
                            , new Rotation() {Value = transform.Rotation}, impulse.Impulse, impulse.Point);

                        ep.Linear = rigidTransform.Linear;
                        //ep.Linear.y = 0.0f;
                        //ep.Linear /= 1.5f;
                        ep.Angular = rigidTransform.Angular;
                    }

                    FSLog.Info($"impulse.Entity:{impulse.Entity},impulse.Impulse:{impulse.Impulse},Linear:{ep.Linear}");
                    // Write back
                    VelocityPredictedData[impulse.Entity] = ep;
                }
            }
        }
    }

    internal struct MoveInternalState
    {
        public CharacterSupportState SupportedState;
        public bool IsJumping;
    }
}