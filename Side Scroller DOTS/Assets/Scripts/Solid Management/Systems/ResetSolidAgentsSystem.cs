using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;

namespace JeffreyDufseth.SolidManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    public class ResetSolidAgentsSystem : JobComponentSystem
    {
        [BurstCompile]
        struct ResetSolidAgentsSystemJob : IJobForEach<SolidAgent>
        {
            public void Execute(ref SolidAgent solidAgent)
            {
                solidAgent.IsCeilingCollided = false;
                solidAgent.CeilingSurfaceNormal = 0.0f;

                solidAgent.IsGroundCollided = false;
                solidAgent.GroundSurfaceNormal = 0.0f;

                solidAgent.IsLeftWallCollided = false;
                solidAgent.LeftWallSurfaceNormal = 0.0f;

                solidAgent.IsRightWallCollided = false;
                solidAgent.RightWallSurfaceNormal = 0.0f;
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new ResetSolidAgentsSystemJob();

            return job.Schedule(this, inputDependencies);
        }
    }
}
