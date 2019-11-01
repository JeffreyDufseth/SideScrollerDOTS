using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Burst;

namespace JeffreyDufseth.HealthManagement.Systems
{
    [UpdateAfter(typeof(EndFramePhysicsSystem))]
    public class DamageAreaTriggerSystem : JobComponentSystem
    {
        BuildPhysicsWorld m_BuildPhysicsWorldSystem;
        StepPhysicsWorld m_StepPhysicsWorldSystem;

        protected override void OnCreate()
        {
            m_BuildPhysicsWorldSystem = World.Active.GetOrCreateSystem<BuildPhysicsWorld>();
            m_StepPhysicsWorldSystem = World.Active.GetOrCreateSystem<StepPhysicsWorld>();
        }

        [BurstCompile]
        struct DamageAreaTriggerSystemJob : ITriggerEventsJob
        {
            [ReadOnly] public ComponentDataFromEntity<DamageArea> DamageAreaGroup;
            public ComponentDataFromEntity<Health> HealthGroup;

            public void Execute(TriggerEvent triggerEvent)
            {
                Entity entityA = triggerEvent.Entities.EntityA;
                Entity entityB = triggerEvent.Entities.EntityB;

                //Damage areas will never damage themselves
                if (entityA != entityB)
                {
                    bool isADamageArea = DamageAreaGroup.Exists(entityA);
                    bool isBDamageArea = DamageAreaGroup.Exists(entityB);
                    bool isAHealth = HealthGroup.Exists(entityA);
                    bool isBHealth = HealthGroup.Exists(entityB);

                    //Entity A is a DamageArea and
                    //Entity B is a Health
                    if (isADamageArea && isBHealth)
                    {
                        DealDamage( entityA,
                                    entityB);
                    }

                    //Entity B is a DamageArea and
                    //Entity A is a Health
                    if (isBDamageArea && isAHealth)
                    {
                        DealDamage( entityB,
                                    entityA);
                    }
                }
            }

            public void DealDamage( Entity damageAreaEntity,
                                    Entity healthEntity)
            {
                DamageArea damageArea = DamageAreaGroup[damageAreaEntity];
                Health health = HealthGroup[healthEntity];

                health.CurrentDamage += damageArea.DamagePerFrame;

                HealthGroup[healthEntity] = health;
            }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            JobHandle jobHandle = new DamageAreaTriggerSystemJob
            {
                DamageAreaGroup = GetComponentDataFromEntity<DamageArea>(true),
                HealthGroup = GetComponentDataFromEntity<Health>(),
            }.Schedule(m_StepPhysicsWorldSystem.Simulation,
                        ref m_BuildPhysicsWorldSystem.PhysicsWorld, inputDeps);

            return jobHandle;
        }
    }
}