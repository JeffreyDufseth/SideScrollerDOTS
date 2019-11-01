using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

namespace JeffreyDufseth.Solids.Systems
{
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public unsafe class SolidAgentCollisionSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.Active.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct CollideWithTerrainJob : ICollisionEventsJob
        {
            [ReadOnly] public PhysicsWorld World;
            [ReadOnly] public ComponentDataFromEntity<Translation> TranslationGroup;
            [ReadOnly] public ComponentDataFromEntity<Rotation> RotationGroup;
            public ComponentDataFromEntity<SolidAgent> SolidAgentGroup;

            public void Execute(CollisionEvent collisionEvent)
            {
                Entity entityA = collisionEvent.Entities.EntityA;
                Entity entityB = collisionEvent.Entities.EntityB;

                //Solids will never affect themselves
                if (entityA != entityB)
                {
                    bool isASolidAgent = SolidAgentGroup.Exists(entityA);
                    bool isBSolidAgent = SolidAgentGroup.Exists(entityB);
                    bool hasTranslationA = TranslationGroup.Exists(entityA);
                    bool hasTranslationB = TranslationGroup.Exists(entityB);
                    bool hasRotationA = TranslationGroup.Exists(entityA);
                    bool hasRotationB = TranslationGroup.Exists(entityB);

                    if (hasTranslationA && hasTranslationB && hasRotationA && hasRotationB)
                    {
                        //Entity A is a SolidAgent
                        if (isASolidAgent)
                        {
                            RigidBody solidAgentRigidBody = World.Bodies[collisionEvent.BodyIndices.BodyAIndex];
                            Translation solidAgentTranslation = TranslationGroup[entityA];
                            Rotation solidAgentRotation = RotationGroup[entityA];

                            RigidBody solidRigidBody = World.Bodies[collisionEvent.BodyIndices.BodyBIndex];
                            Translation solidTranslation = TranslationGroup[entityB];
                            Rotation solidRotation = RotationGroup[entityB];


                            CollideWithSolid(   ref entityA,
                                                ref SolidAgentGroup,
                                                ref solidAgentRigidBody,
                                                ref solidAgentTranslation,
                                                ref solidAgentRotation,
                                                ref solidRigidBody,
                                                ref solidTranslation,
                                                ref solidRotation);
                        }


                        //Entity B is a SolidAgent
                        if (isBSolidAgent)
                        {
                            RigidBody solidAgentRigidBody = World.Bodies[collisionEvent.BodyIndices.BodyBIndex];
                            Translation solidAgentTranslation = TranslationGroup[entityB];
                            Rotation solidAgentRotation = RotationGroup[entityB];

                            RigidBody solidRigidBody = World.Bodies[collisionEvent.BodyIndices.BodyAIndex];
                            Translation solidTranslation = TranslationGroup[entityA];
                            Rotation solidRotation = RotationGroup[entityA];

                            CollideWithSolid(   ref entityB,
                                                ref SolidAgentGroup,
                                                ref solidAgentRigidBody,
                                                ref solidAgentTranslation,
                                                ref solidAgentRotation,
                                                ref solidRigidBody,
                                                ref solidTranslation,
                                                ref solidRotation);
                        }
                    }

                }
            }

            public void CollideWithSolid(   ref Entity solidAgentEntity,
                                            ref ComponentDataFromEntity<SolidAgent> SolidAgentGroup,
                                            ref RigidBody solidAgentRigidBody,
                                            ref Translation solidAgentTranslation,
                                            ref Rotation solidAgentRotation,
                                            ref RigidBody solidRigidBody,
                                            ref Translation solidTranslation,
                                            ref Rotation solidRotation)
            {
                //TODO this method needs general purpose cleanup and testing

                //The solid agent is guaranteed to be a solid agent.
                //We assume anything the solid agent collides with is a solid.
                //Triggers, for example, will not be triggered here - only real collisions happen here

                BoxCollider* solidBoxCollider = (BoxCollider*)solidRigidBody.Collider;

                //Flatten out the translation, since the rigid body caster seems bugged?
                //TODO check into this with later updates to UnityPhysics
                BoxCollider* updatedBoxCollider = (BoxCollider*)BoxCollider.Create(
                    new BoxGeometry
                    {
                        BevelRadius = solidBoxCollider->BevelRadius,
                        Center = solidBoxCollider->Center + solidTranslation.Value,
                        Orientation = solidRotation.Value,
                        Size = solidBoxCollider->Size
                    },
                    solidBoxCollider->Filter,
                    solidBoxCollider->Material).GetUnsafePtr();

                RigidBody updatedSolidRigidBody = new RigidBody
                {
                    CustomTags = solidRigidBody.CustomTags,
                    Collider = (Collider*)updatedBoxCollider,
                    Entity = solidRigidBody.Entity,
                    WorldFromBody = new RigidTransform(quaternion.identity, float3.zero)
                };

                //Collision Check
                ColliderCastInput colliderCastInput = new ColliderCastInput
                {
                    Collider = solidAgentRigidBody.Collider,
                    Start = solidAgentTranslation.Value,
                    End = solidAgentTranslation.Value,
                    Orientation = solidAgentRotation.Value
                };

                
                ColliderCastHit colliderCastHit;
                bool isSolidAgentCollided = updatedSolidRigidBody.CastCollider(colliderCastInput, out colliderCastHit);

                if (isSolidAgentCollided)
                {
                    bool isGroundCollided = false;
                    bool isCeilingCollided = false;
                    bool isLeftWallCollided = false;
                    bool isRightWallCollided = false;

                    //TODO tune this range
                    if (colliderCastHit.SurfaceNormal.y >= 0.99999f)
                    {
                        isGroundCollided = true;
                    }
                    else if (colliderCastHit.SurfaceNormal.y <= -0.99999f)
                    {
                        isGroundCollided = true;
                    }
                    else if (colliderCastHit.SurfaceNormal.x >= 0.99999f)
                    {
                        isLeftWallCollided = true;
                    }
                    else if (colliderCastHit.SurfaceNormal.x <= -0.99999f)
                    {
                        isRightWallCollided = true;
                    }



                    if (isGroundCollided
                        || isCeilingCollided
                        || isLeftWallCollided
                        || isRightWallCollided)
                    {
                        SolidAgent solidAgent = SolidAgentGroup[solidAgentEntity];

                        if (isGroundCollided)
                        {
                            solidAgent.IsGroundCollided = true;
                            solidAgent.GroundSurfaceNormal = colliderCastHit.SurfaceNormal.y;
                        }

                        if (isCeilingCollided)
                        {
                            solidAgent.IsCeilingCollided = true;
                            solidAgent.CeilingSurfaceNormal = colliderCastHit.SurfaceNormal.y;
                        }

                        if (isLeftWallCollided)
                        {
                            solidAgent.IsLeftWallCollided = true;
                            solidAgent.LeftWallSurfaceNormal = colliderCastHit.SurfaceNormal.x;
                        }

                        if (isRightWallCollided)
                        {
                            solidAgent.IsRightWallCollided = true;
                            solidAgent.RightWallSurfaceNormal = colliderCastHit.SurfaceNormal.x;
                        }

                        SolidAgentGroup[solidAgentEntity] = solidAgent;
                    }
                }
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle jobHandle = new CollideWithTerrainJob
            {
                World = m_BuildPhysicsWorldSystem.PhysicsWorld,
                TranslationGroup = GetComponentDataFromEntity<Translation>(true),
                RotationGroup = GetComponentDataFromEntity<Rotation>(true),
                SolidAgentGroup = GetComponentDataFromEntity<SolidAgent>(),
            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

            return jobHandle;
        }
    }
}