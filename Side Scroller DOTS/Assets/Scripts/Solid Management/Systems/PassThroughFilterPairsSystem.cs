using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;
using System;
using Unity.Burst;

namespace JeffreyDufseth.SolidManagement.Systems
{
    [UpdateBefore(typeof(StepPhysicsWorld))]
    public class PassThroughFilterPairsSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_PhysicsWorld;
        StepPhysicsWorld m_StepPhysicsWorld;

        EntityQuery m_passThroughQuery;

        protected override void OnCreate()
        {
            m_passThroughQuery = GetEntityQuery(new EntityQueryDesc
            {
                Any = new ComponentType[] { typeof(PassThrough) }
            });

            m_PhysicsWorld = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorld = World.Active.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct PassThroughFilterPairsSystemJob : IBodyPairsJob
        {
            public NativeSlice<RigidBody> Bodies;

            [ReadOnly] public ComponentDataFromEntity<PassThrough> PassThroughGroup;

            public unsafe void Execute(ref ModifiableBodyPair pair)
            {
                Entity entityA = pair.Entities.EntityA;
                Entity entityB = pair.Entities.EntityB;

                if (entityA != entityB)
                {
                    bool isAPassThrough = PassThroughGroup.Exists(entityA);
                    bool isBPassThrough = PassThroughGroup.Exists(entityB);

                    bool disableA = false;
                    bool disableB = false;


                    if (isAPassThrough)
                    {
                        Unity.Physics.RigidBody passThroughRigidBody = Bodies[pair.BodyIndices.BodyAIndex];
                        Unity.Physics.RigidBody colliderRigidBody = Bodies[pair.BodyIndices.BodyBIndex];

                        disableA = PassThroughSolid(ref entityA,
                                                    ref entityB,
                                                    ref PassThroughGroup,
                                                    ref passThroughRigidBody,
                                                    ref colliderRigidBody);
                    }

                    if (isBPassThrough)
                    {
                        Unity.Physics.RigidBody passThroughRigidBody = Bodies[pair.BodyIndices.BodyBIndex];
                        Unity.Physics.RigidBody colliderRigidBody = Bodies[pair.BodyIndices.BodyAIndex];

                        disableB = PassThroughSolid(ref entityB,
                                                    ref entityA,
                                                    ref PassThroughGroup,
                                                    ref passThroughRigidBody,
                                                    ref colliderRigidBody);
                    }


                    if (disableA || disableB)
                    {
                        pair.Disable();
                    }
                }
            }

            public bool PassThroughSolid(ref Entity passThroughEntity,
                                            ref Entity solidAgentEntity,
                                            ref ComponentDataFromEntity<PassThrough> PassThroughGroup,
                                            ref RigidBody passThroughRigidBody,
                                            ref RigidBody colliderRigidBody)
            {
                //Check each side
                PassThrough passThrough = PassThroughGroup[passThroughEntity];

                Aabb passThroughAabb = passThroughRigidBody.CalculateAabb();
                Aabb colliderAabb = colliderRigidBody.CalculateAabb();
                bool shouldDisable = true;

                bool clearsLeft = (colliderAabb.Max.x <= passThroughAabb.Min.x);
                bool clearsTop = (colliderAabb.Min.y >= passThroughAabb.Max.y);
                bool clearsRight = (colliderAabb.Min.x >= passThroughAabb.Max.x);
                bool clearsBottom = (colliderAabb.Max.y <= passThroughAabb.Min.y);

                bool fullyClearsLeft = (colliderAabb.Max.x < passThroughAabb.Min.x);
                bool fullyClearsTop = (colliderAabb.Min.y > passThroughAabb.Max.y);
                bool fullyClearsRight = (colliderAabb.Min.x > passThroughAabb.Max.x);
                bool fullyClearsBottom = (colliderAabb.Max.y < passThroughAabb.Min.y);

                //Left
                if (shouldDisable)
                {
                    if (((uint)passThrough.Directions & (uint)PassThroughDirections.Left) == 0)
                    {
                        //This side may be hit
                        if (clearsLeft &&
                            (!fullyClearsTop || !fullyClearsBottom))
                        {
                            shouldDisable = false;
                        }
                    }
                }

                //Top
                if (shouldDisable)
                {
                    if (((uint)passThrough.Directions & (uint)PassThroughDirections.Top) == 0)
                    {
                        //This side may be hit
                        if (clearsTop &&
                            (!fullyClearsLeft || !fullyClearsRight))
                        {
                            shouldDisable = false;
                        }
                    }
                }

                //Right
                if (shouldDisable)
                {
                    if (((uint)passThrough.Directions & (uint)PassThroughDirections.Right) == 0)
                    {
                        //This side may be hit
                        if (clearsRight &&
                            (!fullyClearsTop || !fullyClearsBottom))
                        {
                            shouldDisable = false;
                        }
                    }
                }

                //Bottom
                if (shouldDisable)
                {
                    if (((uint)passThrough.Directions & (uint)PassThroughDirections.Bottom) == 0)
                    {
                        //This side may be hit
                        if (clearsBottom &&
                            (!fullyClearsLeft || !fullyClearsRight))
                        {
                            shouldDisable = false;
                        }
                    }
                }

                return shouldDisable;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            if (m_passThroughQuery.CalculateChunkCount() == 0)
            {
                return inputDeps;
            }

            if (m_StepPhysicsWorld.Simulation.Type == SimulationType.NoPhysics)
            {
                return inputDeps;
            }

            // Add a custom callback to the simulation, which will inject our custom job after the body pairs have been created
            SimulationCallbacks.Callback callback = (ref ISimulation simulation, ref PhysicsWorld world, JobHandle inDeps) =>
            {
                inDeps.Complete(); //<todo Needed to initialize our modifier

                return new PassThroughFilterPairsSystemJob
                {
                    Bodies = m_PhysicsWorld.PhysicsWorld.Bodies,
                    PassThroughGroup = GetComponentDataFromEntity<PassThrough>(true)
                }.Schedule(simulation, ref world, inputDeps);
            };
            m_StepPhysicsWorld.EnqueueCallback(SimulationCallbacks.Phase.PostCreateDispatchPairs, callback);

            return inputDeps;
        }
    }
}