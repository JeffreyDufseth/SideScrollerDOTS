using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Transforms;
using JeffreyDufseth.VelocityCurves.Systems;
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
                //TODO - Inverse Inertia is zero'd out for all axes if at least one should lock
                //Look to remove this when Unity Physics supports locked axes natively

                if (lockAxes.ShouldLockX)
                {
                    translation.Value = new float3(lockAxes.LockX, translation.Value.y, translation.Value.z);
                    physicsMass.InverseInertia = new float3(0.0f, 0.0f, 0.0f);
                }

                if (lockAxes.ShouldLockY)
                {
                    translation.Value = new float3(translation.Value.x, lockAxes.LockY, translation.Value.z);
                    physicsMass.InverseInertia = new float3(0.0f, 0.0f, 0.0f);
                }

                if (lockAxes.ShouldLockZ)
                {
                    translation.Value = new float3(translation.Value.x, translation.Value.y, lockAxes.LockZ);
                    physicsMass.InverseInertia = new float3(0.0f, 0.0f, 0.0f);
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
