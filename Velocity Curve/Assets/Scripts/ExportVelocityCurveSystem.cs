using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;

namespace JeffreyDufseth.VelocityCurve
{
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class ExportVelocityCurveSystem : JobComponentSystem
    {
        [BurstCompile]
        struct ExportVelocityCurveSystemJob : IJobForEachWithEntity_EBC<VelocityCurveBuffer, PhysicsVelocity>
        {
            [DeallocateOnJobCompletion] [ReadOnly] public ComponentDataFromEntity<VelocityCurve> VelocityCurveGroup;

            public void Execute(Entity entity, int index, DynamicBuffer<VelocityCurveBuffer> velocityCurveBuffer, ref PhysicsVelocity physicsVelocity)
            {
                float3 newVelocity = new float3(0, 0, 0);

                for(int i = 0; i < velocityCurveBuffer.Length; i++)
                {
                    VelocityCurve velocityCurve = VelocityCurveGroup[entity];

                    newVelocity.x += velocityCurve.X.CurrentVelocity;
                    newVelocity.y += velocityCurve.Y.CurrentVelocity;
                    newVelocity.z += velocityCurve.Z.CurrentVelocity;
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new ExportVelocityCurveSystemJob();
            job.VelocityCurveGroup = GetComponentDataFromEntity<VelocityCurve>();

            return job.Schedule(this, inputDependencies);
        }
    }
}