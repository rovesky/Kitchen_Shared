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
            var predictType = GetArchetypeChunkComponentType<TransformPredictedState>();
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
                EntityPredictedType = predictType,
        
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
            public ArchetypeChunkComponentType<TransformPredictedState> EntityPredictedType;

            //  public ArchetypeChunkComponentType<Translation> TranslationType;
            //  public ArchetypeChunkComponentType<Rotation> RotationType;
            [ReadOnly] public ArchetypeChunkComponentType<CharacterMoveSetting>   CharacterMoveType;
            [ReadOnly] public ArchetypeChunkComponentType<UserCommand>     UserCommandComponentType;
            [ReadOnly] public ArchetypeChunkComponentType<PhysicsCollider> PhysicsColliderType;

            // Stores impulses we wish to apply to dynamic bodies the character is interacting with.
            // This is needed to avoid race conditions when 2 characters are interacting with the
            // same body at the same time.
            public NativeStream.Writer DeferredImpulseWriter;


            public unsafe void Execute(ArchetypeChunk chunk, int chunkIndex, int firstEntityIndex)
            {
                var up = math.up();

                var chunkEntityData = chunk.GetNativeArray(EntityType);
                var chunkCharacterMoveData = chunk.GetNativeArray(CharacterMoveType);
                var chunkPredictData = chunk.GetNativeArray(EntityPredictedType);
                var chunkUserCommand = chunk.GetNativeArray(UserCommandComponentType);
                var chunkCharacterMoveInternalData = chunk.GetNativeArray(CharacterMovePredictedType);
                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);
                // var chunkTranslationData = chunk.GetNativeArray(TranslationType);
                //  var chunkRotationData = chunk.GetNativeArray(RotationType);

                DeferredImpulseWriter.BeginForEachIndex(chunkIndex);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = chunkEntityData[i];
                    var characterMove = chunkCharacterMoveData[i];
                    var userCommand = chunkUserCommand[i];
                    var movePredictedData = chunkCharacterMoveInternalData[i];
                    var collider = chunkPhysicsColliderData[i];
                    var predictData = chunkPredictData[i];

                    // Collision filter must be valid
                    Assert.IsTrue(collider.ColliderPtr->Filter.IsValid);

                    // Character step input
                    var stepInput = new CharacterControllerStepInput
                    {
                        World = PhysicsWorld,
                        DeltaTime = DeltaTime,
                        Up = math.up(),
                        Gravity = characterMove.Gravity,
                        MaxIterations = characterMove.MaxIterations,
                        Tau = KDefaultTau,
                        Damping = KDefaultDamping,
                        SkinWidth = characterMove.SkinWidth,
                        ContactTolerance = characterMove.ContactTolerance,
                        MaxSlope = characterMove.MaxSlope,
                        RigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(entity),
                        CurrentVelocity = movePredictedData.LinearVelocity,
                        MaxMovementSpeed = characterMove.MaxVelocity
                    };

                    // Character transform
                    var transform = new RigidTransform()
                    {
                        pos = predictData.Position,
                        rot = predictData.Rotation
                    };

                    // Check support
                    CheckSupport(ref PhysicsWorld, ref collider, stepInput, transform, characterMove.MaxSlope,
                        out movePredictedData.SupportedState, out var surfaceNormal, out var surfaceVelocity);

                    // User input
                    var desiredVelocity = movePredictedData.LinearVelocity;
                    HandleUserInput(characterMove, userCommand, stepInput.Up, surfaceVelocity,
                        ref movePredictedData,ref desiredVelocity);

                    // Calculate actual velocity with respect to surface
                    if (movePredictedData.SupportedState == CharacterSupportState.Supported)
                        CalculateMovement(predictData.Rotation, stepInput.Up, movePredictedData.IsJumping,
                            movePredictedData.LinearVelocity, desiredVelocity, surfaceNormal, surfaceVelocity,
                            out movePredictedData.LinearVelocity);
                    else
                        movePredictedData.LinearVelocity = desiredVelocity;

                    //// World collision + integrate
                    CollideAndIntegrate(stepInput, characterMove.CharacterMass,
                        characterMove.AffectsPhysicsBodies > 0,
                        collider.ColliderPtr, ref transform, ref movePredictedData.LinearVelocity,
                        ref DeferredImpulseWriter);

                    // var newVelocity = desiredVelocity;
                    var newPosition = transform.pos;
                    //World collision + integrate
                    //CharacterControllerUtilities.CollideAndIntegrate(ref PhysicsWorld, characterMove.SkinWidth,
                    //    characterMove.ContactTolerance, characterMove.MaxVelocity,
                    //    collider.ColliderPtr, DeltaTime, transform, up, entity, ref newPosition, ref newVelocity);

                    //characterMoveInternalData.LinearVelocity = newVelocity;

                    // Write back and orientation integration
                    predictData.Position = newPosition;
                    // chracter rotate
                    if (math.distancesq(userCommand.TargetDir, float3.zero) > 0.0001f)
                    {
                        var fromRotation = predictData.Rotation;
                        var toRotation = quaternion.LookRotationSafe(userCommand.TargetDir, up);
                        var angle = Quaternion.Angle(fromRotation, toRotation);
                        predictData.Rotation = Quaternion.RotateTowards(fromRotation, toRotation,
                            math.abs(angle - 180.0f) < float.Epsilon
                                ? -characterMove.RotationVelocity
                                : characterMove.RotationVelocity);
                    }

                    // Write back to chunk data
                    {
                        chunkCharacterMoveInternalData[i] = movePredictedData;
                        chunkPredictData[i] = predictData;
                    }
                }

                DeferredImpulseWriter.EndForEachIndex();
            }


            private void HandleUserInput(CharacterMoveSetting ccComponentData, UserCommand command, float3 up,
                float3 surfaceVelocity, ref CharacterMovePredictedState ccPredictedState, ref float3 linearVelocity)
            {
                // Reset jumping state and unsupported velocity
                if (ccPredictedState.SupportedState == CharacterSupportState.Supported)
                {
                    ccPredictedState.IsJumping = false;
                    ccPredictedState.UnsupportedVelocity = float3.zero;
                }


                var shouldJump = command.Buttons.IsSet(UserCommand.Button.Jump) &&
                                 ccPredictedState.SupportedState == CharacterSupportState.Supported;

                if (shouldJump)
                {
                    // Add jump speed to surface velocity and make character unsupported
                    ccPredictedState.IsJumping = true;
                    ccPredictedState.SupportedState = CharacterSupportState.Unsupported;
                    ccPredictedState.UnsupportedVelocity = surfaceVelocity + ccComponentData.JumpUpwardsVelocity * up;
                }
                else if (ccPredictedState.SupportedState != CharacterSupportState.Supported)
                {
                    // Apply gravity
                    ccPredictedState.UnsupportedVelocity += ccComponentData.Gravity * DeltaTime;
                }

                // If unsupported then keep jump and surface momentum
                linearVelocity = (float3) command.TargetDir * ccComponentData.MaxVelocity +
                                 (ccPredictedState.SupportedState != CharacterSupportState.Supported
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
                    
                  
                    var pm = PhysicsMassData[impulse.Entity];
                    var ep = VelocityPredictedData[impulse.Entity];
                    var transform = TransformPredictedData[impulse.Entity];

                 //   FSLog.Info($"impulse.Entity:{impulse.Entity},pm:{pm.InverseMass}");

                    // Don't apply on kinematic bodies
                    if (!(pm.InverseMass > 0.0f))
                       continue;

                  //  if(ep.Linear.y > 0.01f)
                     //   continue;
                  //  FSLog.Info($"impulse.Entity:{impulse.Entity},Linear SqrMagnitude:{Vector3.SqrMagnitude(ep.Linear)}");
                  //  if (Vector3.SqrMagnitude(ep.Linear) > 10)
                     //   continue;
                    

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

                 //   FSLog.Info($"impulse.Entity:{impulse.Entity},Linear:{ep.Linear}");
                    // Write back
                    VelocityPredictedData[impulse.Entity] = ep;
                }
            }
        }
    }
}