using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using JeffreyDufseth.VelocityCurveManagement.Systems;
using JeffreyDufseth.VelocityCurveManagement;
using Unity.Mathematics;

namespace JeffreyDufseth.SideScrollerManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(VelocityCurveSystem))]
    public class WindControllerSystem : JobComponentSystem
    {
        [BurstCompile]
        struct WindControllerSystemJob : IJobForEach<VelocityCurve, WindController>
        {
            public float DeltaTime;

            public void Execute(ref VelocityCurve velocityCurve, ref WindController windController)
            {
                //The wind never blows on the Z axis
                velocityCurve.Z = VelocityCurveAxis.Zero;

                //Turn the wind on and off
                windController.ElapsedTime += DeltaTime;

                if (windController.IsBlowing && 
                    (windController.ElapsedTime > windController.OnTime))
                {
                    //Turn the wind off.
                    windController.IsBlowing = false;
                    windController.ElapsedTime = 0.0f;

                    velocityCurve.X = VelocityCurveAxis.Quadratic(  velocityCurve.X.CurrentVelocity,
                                                                    windController.WindDirection.x < 0.0f,
                                                                    windController.WindAbsoluteDeceleration * math.abs(windController.WindDirection.x),
                                                                    0.0f);

                    velocityCurve.Y = VelocityCurveAxis.Quadratic(  velocityCurve.Y.CurrentVelocity,
                                                                    windController.WindDirection.y < 0.0f,
                                                                    windController.WindAbsoluteDeceleration * math.abs(windController.WindDirection.y),
                                                                    0.0f);
                }
                else if (!windController.IsBlowing &&
                        (windController.ElapsedTime > windController.OffTime))
                {
                    //Turn the wind on.
                    windController.IsBlowing = true;
                    windController.ElapsedTime = 0.0f;

                    velocityCurve.X = VelocityCurveAxis.Quadratic(  velocityCurve.X.CurrentVelocity,
                                                                    windController.WindDirection.x > 0.0f,
                                                                    windController.WindAbsoluteDeceleration * math.abs(windController.WindDirection.x),
                                                                    windController.WindAbsoluteMaximumVelocity * math.abs(windController.WindDirection.x));

                    velocityCurve.Y = VelocityCurveAxis.Quadratic(  velocityCurve.Y.CurrentVelocity,
                                                                    windController.WindDirection.y > 0.0f,
                                                                    windController.WindAbsoluteDeceleration * math.abs(windController.WindDirection.y),
                                                                    windController.WindAbsoluteMaximumVelocity * math.abs(windController.WindDirection.y));
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new WindControllerSystemJob();
            job.DeltaTime = Time.fixedDeltaTime;

            return job.Schedule(this, inputDependencies);
        }
    }
}
