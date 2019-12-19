using FootStone.ECS;
using System;
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
using static FootStone.Kitchen.CharacterControllerUtilitiesNew;
using static Unity.Physics.PhysicsStep;

namespace FootStone.Kitchen
{
    public struct CharacterMoveInternalState : IComponentData
    {
      //  public float CurrentRotationAngle;
        public CharacterSupportState SupportedState;
        public float3 UnsupportedVelocity;
        public float3 LinearVelocity;
        public Entity Entity;
        public bool IsJumping;
    }


    [DisableAutoCreation]
    public class CharacterMoveSystemNew : JobComponentSystem
    {
        const float k_DefaultTau = 0.4f;
        const float k_DefaultDamping = 0.9f;

        [BurstCompile]
        struct CharacterControllerJob : IJobChunk
        {
            public float DeltaTime;

            [ReadOnly] public PhysicsWorld PhysicsWorld;
            [ReadOnly] public ArchetypeChunkEntityType EntityType;

            public ArchetypeChunkComponentType<CharacterMoveInternalState> CharacterControllerInternalType;
            public ArchetypeChunkComponentType<EntityPredictedState> PredictType;
            //  public ArchetypeChunkComponentType<Translation> TranslationType;
            //  public ArchetypeChunkComponentType<Rotation> RotationType;
            [ReadOnly] public ArchetypeChunkComponentType<CharacterMove> CharacterControllerComponentType;
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
                var chunkCCData = chunk.GetNativeArray(CharacterControllerComponentType);
                var chunkPredictData = chunk.GetNativeArray(PredictType);
                var chunkUserCommand = chunk.GetNativeArray(UserCommandComponentType);
                var chunkCCInternalData = chunk.GetNativeArray(CharacterControllerInternalType);
                var chunkPhysicsColliderData = chunk.GetNativeArray(PhysicsColliderType);

                // var chunkTranslationData = chunk.GetNativeArray(TranslationType);
                //  var chunkRotationData = chunk.GetNativeArray(RotationType);

                DeferredImpulseWriter.BeginForEachIndex(chunkIndex);

                for (var i = 0; i < chunk.Count; i++)
                {
                    var entity = chunkEntityData[i];
                    var characterMove = chunkCCData[i];
                    var userCommand = chunkUserCommand[i];
                    var ccInternalData = chunkCCInternalData[i];
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
                        Tau = k_DefaultTau,
                        Damping = k_DefaultDamping,
                        SkinWidth = characterMove.SkinWidth,
                        ContactTolerance = characterMove.ContactTolerance,
                        MaxSlope = characterMove.MaxSlope,
                        RigidBodyIndex = PhysicsWorld.GetRigidBodyIndex(ccInternalData.Entity),
                        CurrentVelocity = ccInternalData.LinearVelocity,
                        MaxMovementSpeed = characterMove.MaxVelocity
                    };

                    // Character transform
                    var transform = predictData.Transform;

                    // Check support
                    CheckSupport(ref PhysicsWorld, ref collider, stepInput, transform, characterMove.MaxSlope,
                        out ccInternalData.SupportedState, out float3 surfaceNormal, out float3 surfaceVelocity);

                    // User input
                    var desiredVelocity = ccInternalData.LinearVelocity;
                    HandleUserInput(characterMove, userCommand, stepInput.Up, surfaceVelocity, ref ccInternalData,
                        ref desiredVelocity);

                    // Calculate actual velocity with respect to surface
                    if (ccInternalData.SupportedState == CharacterSupportState.Supported)
                    {

                        CalculateMovement(predictData.Transform.rot, stepInput.Up, ccInternalData.IsJumping,
                            ccInternalData.LinearVelocity, desiredVelocity, surfaceNormal, surfaceVelocity,
                            out ccInternalData.LinearVelocity);
                        // ccInternalData.LinearVelocity = desiredVelocity;
                    }
                    else
                    {
                        ccInternalData.LinearVelocity = desiredVelocity;
                    }

                    //// World collision + integrate
                    //CollideAndIntegrate(stepInput, ccComponentData.CharacterMass,
                    //    ccComponentData.AffectsPhysicsBodies > 0,
                    //    collider.ColliderPtr, ref transform, ref ccInternalData.LinearVelocity,
                    //    ref DeferredImpulseWriter);

                    //World collision + integrate
                    var newVelocity = desiredVelocity;
                    var newPosition = transform.pos;

                    CharacterControllerUtilities.CollideAndIntegrate(ref PhysicsWorld,characterMove.SkinWidth,
                        characterMove.ContactTolerance,characterMove.MaxVelocity,
                        collider.ColliderPtr, DeltaTime,transform,up,entity,ref newPosition,ref newVelocity);

                    // Write back and orientation integration
                    predictData.Transform.pos = newPosition;
                    predictData.Velocity.Linear = newVelocity;
                    // chracter rotate
                    if (math.distancesq(userCommand.TargetDir, float3.zero) > 0.0001f)
                    {
                        var fromRotation = predictData.Transform.rot;
                        var toRotation = quaternion.LookRotationSafe(userCommand.TargetDir, up);
                        var angle = Quaternion.Angle(fromRotation, toRotation);
                        predictData.Transform.rot = Quaternion.RotateTowards(fromRotation, toRotation,
                            math.abs(angle - 180.0f) < float.Epsilon
                                ? -characterMove.RotationVelocity
                                : characterMove.RotationVelocity);
                    }

                    // Write back to chunk data
                    {
                        chunkCCInternalData[i] = ccInternalData;
                        chunkPredictData[i] = predictData;
                    }
                }

                DeferredImpulseWriter.EndForEachIndex();
            }

          

            private void HandleUserInput(CharacterMove ccComponentData, UserCommand command, float3 up,
                float3 surfaceVelocity,ref CharacterMoveInternalState ccInternalState, ref float3 linearVelocity)
            {
                // Reset jumping state and unsupported velocity
                if (ccInternalState.SupportedState == CharacterSupportState.Supported)
                {
                    ccInternalState.IsJumping = false;
                    ccInternalState.UnsupportedVelocity = float3.zero;
                }

          
                var shouldJump = command.Buttons.IsSet(UserCommand.Button.Jump) &&
                                 ccInternalState.SupportedState == CharacterSupportState.Supported;

                if (shouldJump)
                {
                    // Add jump speed to surface velocity and make character unsupported
                    ccInternalState.IsJumping = true;
                    ccInternalState.SupportedState = CharacterSupportState.Unsupported;
                    ccInternalState.UnsupportedVelocity = surfaceVelocity + ccComponentData.JumpUpwardsVelocity * up;
                }
                else if (ccInternalState.SupportedState != CharacterSupportState.Supported)
                {
                    // Apply gravity
                    ccInternalState.UnsupportedVelocity += ccComponentData.Gravity * DeltaTime;
                }

                // If unsupported then keep jump and surface momentum
                linearVelocity = (float3)command.TargetDir * ccComponentData.MaxVelocity +
                                 (ccInternalState.SupportedState != CharacterSupportState.Supported
                                     ? ccInternalState.UnsupportedVelocity
                                     : float3.zero);

            }

            private void CalculateMovement(quaternion currentRotationAngle, float3 up, bool isJumping,
                float3 currentVelocity, float3 desiredVelocity, float3 surfaceNormal, float3 surfaceVelocity,
                out float3 linearVelocity)
            {
                var forward = math.forward(currentRotationAngle);

                Rotation surfaceFrame;
                float3 binorm;
                {
                    binorm = math.cross(forward, up);
                    binorm = math.normalize(binorm);

                    float3 tangent = math.cross(binorm, surfaceNormal);
                    tangent = math.normalize(tangent);

                    binorm = math.cross(tangent, surfaceNormal);
                    binorm = math.normalize(binorm);

                    surfaceFrame.Value = new quaternion(new float3x3(binorm, tangent, surfaceNormal));
                }

                float3 relative = currentVelocity - surfaceVelocity;
                relative = math.rotate(math.inverse(surfaceFrame.Value), relative);

                float3 diff;
                {
                    float3 sideVec = math.cross(forward, up);
                    float fwd = math.dot(desiredVelocity, forward);
                    float side = math.dot(desiredVelocity, sideVec);
                    float len = math.length(desiredVelocity);
                    float3 desiredVelocitySF = new float3(-side, -fwd, 0.0f);
                    desiredVelocitySF = math.normalizesafe(desiredVelocitySF, float3.zero);
                    desiredVelocitySF *= len;
                    diff = desiredVelocitySF - relative;
                }

                relative += diff;

                linearVelocity = math.rotate(surfaceFrame.Value, relative) + surfaceVelocity +
                                 (isJumping ? math.dot(desiredVelocity, up) * up : float3.zero);
            }
        }

        [BurstCompile]
        struct ApplyDefferedPhysicsUpdatesJob : IJob
        {
            // Chunks can be deallocated at this point
            [DeallocateOnJobCompletion] public NativeArray<ArchetypeChunk> Chunks;

            public NativeStream.Reader DeferredImpulseReader;

            public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocityData;
            public ComponentDataFromEntity<PhysicsMass> PhysicsMassData;
            public ComponentDataFromEntity<Translation> TranslationData;
            public ComponentDataFromEntity<Rotation> RotationData;

            public void Execute()
            {
                int index = 0;
                int maxIndex = DeferredImpulseReader.ForEachCount;
                DeferredImpulseReader.BeginForEachIndex(index++);
                while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex)
                {
                    DeferredImpulseReader.BeginForEachIndex(index++);
                }

                while (DeferredImpulseReader.RemainingItemCount > 0)
                {
                    // Read the data
                    var impulse = DeferredImpulseReader.Read<DeferredCharacterControllerImpulse>();
                    while (DeferredImpulseReader.RemainingItemCount == 0 && index < maxIndex)
                    {
                        DeferredImpulseReader.BeginForEachIndex(index++);
                    }

                    PhysicsVelocity pv = PhysicsVelocityData[impulse.Entity];
                    PhysicsMass pm = PhysicsMassData[impulse.Entity];
                    Translation t = TranslationData[impulse.Entity];
                    Rotation r = RotationData[impulse.Entity];

                    // Don't apply on kinematic bodies
                    if (pm.InverseMass > 0.0f)
                    {
                        // Apply impulse
                        pv.ApplyImpulse(pm, t, r, impulse.Impulse, impulse.Point);

                        // Write back
                        PhysicsVelocityData[impulse.Entity] = pv;
                    }
                }
            }
        }

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
                    typeof(CharacterMove),
                    typeof(UserCommand),
                    typeof(CharacterMoveInternalState),
                    typeof(CharacterPredictedState),
                    typeof(PhysicsCollider),
                  //  typeof(Translation),
                 //   typeof(Rotation),
                }
            };
            m_CharacterControllersGroup = GetEntityQuery(query);
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_CharacterControllersGroup.CalculateEntityCount() == 0)
                return inputDeps;

            var chunks = m_CharacterControllersGroup.CreateArchetypeChunkArray(Allocator.TempJob);

            var ccComponentType = GetArchetypeChunkComponentType<CharacterMove>();
            var userCommandType = GetArchetypeChunkComponentType<UserCommand>();
            var ccInternalType = GetArchetypeChunkComponentType<CharacterMoveInternalState>();
            var predictType = GetArchetypeChunkComponentType<EntityPredictedState>();
            var physicsColliderType = GetArchetypeChunkComponentType<PhysicsCollider>();
            var entityType = GetArchetypeChunkEntityType();
            // var translationType = GetArchetypeChunkComponentType<Translation>();
            // var rotationType = GetArchetypeChunkComponentType<Rotation>();

            var deferredImpulses = new NativeStream(chunks.Length, Allocator.TempJob);
            var tickDuration = GetSingleton<WorldTime>().TickDuration;
            var ccJob = new CharacterControllerJob
            {
                EntityType = entityType,
                // Archetypes
                CharacterControllerComponentType = ccComponentType,
                UserCommandComponentType = userCommandType,
                CharacterControllerInternalType = ccInternalType,
                PhysicsColliderType = physicsColliderType,
                PredictType = predictType,
           //     TranslationType = translationType,
           //      RotationType = rotationType,
           // Input
                DeltaTime = tickDuration,
                PhysicsWorld = m_BuildPhysicsWorldSystem.PhysicsWorld,
                DeferredImpulseWriter = deferredImpulses.AsWriter()
            };

            inputDeps = JobHandle.CombineDependencies(inputDeps, m_ExportPhysicsWorldSystem.FinalJobHandle);
            inputDeps = ccJob.Schedule(m_CharacterControllersGroup, inputDeps);

            var applyJob = new ApplyDefferedPhysicsUpdatesJob()
            {
                Chunks = chunks,
                DeferredImpulseReader = deferredImpulses.AsReader(),
                PhysicsVelocityData = GetComponentDataFromEntity<PhysicsVelocity>(),
                PhysicsMassData = GetComponentDataFromEntity<PhysicsMass>(),
                TranslationData = GetComponentDataFromEntity<Translation>(),
                RotationData = GetComponentDataFromEntity<Rotation>()
            };

            inputDeps = applyJob.Schedule(inputDeps);
            var disposeHandle = deferredImpulses.Dispose(inputDeps);

            // Must finish all jobs before physics step end
            m_EndFramePhysicsSystem.HandlesToWaitFor.Add(disposeHandle);

            return inputDeps;
        }
    }
}