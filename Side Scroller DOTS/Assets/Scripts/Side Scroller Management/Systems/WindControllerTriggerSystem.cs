using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;
using JeffreyDufseth.VelocityCurveManagement;

namespace JeffreyDufseth.SideScrollerManagement.Systems
{
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class WindControllerTriggerSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.Active.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct WindControllerTriggerSystemJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<WindController> WindControllerGroup;
            public BufferFromEntity<VelocityCurveBuffer> VelocityCurveBufferGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                //The wind will never affect itself
                if (entityA != entityB)
                {
                    bool isAWindController = WindControllerGroup.Exists(entityA);
                    bool isBWindController = WindControllerGroup.Exists(entityB);
                    bool isAVelocityCurveBuffer = VelocityCurveBufferGroup.Exists(entityA);
                    bool isBVelocityCurveBuffer = VelocityCurveBufferGroup.Exists(entityB);

                    //Entity A is a WindController and
                    //Entity B is a VelocityCurveBuffer
                    if (isAWindController && isBVelocityCurveBuffer)
                    {
                        ApplyWind(  entityA,
                                    entityB);
                    }

                    //Entity B is a WindController and
                    //Entity A is a VelocityCurveBuffer
                    if (isBWindController && isAVelocityCurveBuffer)
                    {
                        ApplyWind(  entityB,
                                    entityA);
                    }
                }
            }

            public void ApplyWind(  Entity windControllerEntity,
                                    Entity velocityCurveBufferEntity)
            {
                DynamicBuffer<VelocityCurveBuffer> velocityCurveBuffer = VelocityCurveBufferGroup[velocityCurveBufferEntity];
                velocityCurveBuffer.Add(new VelocityCurveBuffer
                {
                    VelocityCurveEntity = windControllerEntity
                });
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle jobHandle = new WindControllerTriggerSystemJob
            {
                WindControllerGroup = GetComponentDataFromEntity<WindController>(true),
                VelocityCurveBufferGroup = GetBufferFromEntity<VelocityCurveBuffer>(),
            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

            return jobHandle;
        }
    }
}