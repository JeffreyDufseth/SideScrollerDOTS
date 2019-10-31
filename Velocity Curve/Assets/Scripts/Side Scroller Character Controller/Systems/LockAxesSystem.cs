using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Transforms;
using JeffreyDufseth.VelocityCurve;
using Unity.Mathematics;
using Unity.Burst;
using Unity.Jobs;

namespace JeffreyDufseth.SideScroller.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(ExportVelocityCurveSystem))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class LockAxesSystem : JobComponentSystem
    {
        [BurstCompile]
        struct LockZAxisSystemJob : IJobForEach<LockAxes, PhysicsMass, Translation>
        {
            public void Execute(ref LockAxes lockAxes, ref PhysicsMass physicsMass, ref Translation translation)
            {
                if (lockAxes.ShouldLockX)
                {
                    translation.Value = new float3(lockAxes.LockX, translation.Value.y, translation.Value.z);
                }

                if (lockAxes.ShouldLockY)
                {
                    translation.Value = new float3(translation.Value.x, lockAxes.LockY, translation.Value.z);
                }

                if (lockAxes.ShouldLockZ)
                {
                    translation.Value = new float3(translation.Value.x, translation.Value.y, lockAxes.LockZ);
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new LockZAxisSystemJob();

            return job.Schedule(this, inputDependencies);
        }
    }
}
