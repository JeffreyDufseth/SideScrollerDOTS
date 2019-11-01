using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace JeffreyDufseth.VelocityCurveManagement.Systems
{
    [UpdateBefore(typeof(ExportVelocityCurveSystem))]
    public class VelocityCurveSystem : JobComponentSystem
    {
        [BurstCompile]
        struct VelocityCurveSystemJob : IJobForEach<VelocityCurve>
        {
            [ReadOnly] public float deltaTime;

            public void Execute(ref VelocityCurve velocityCurve)
            {
                velocityCurve.X = CalculateLinearVelocity(velocityCurve.X, deltaTime);
                velocityCurve.Y = CalculateLinearVelocity(velocityCurve.Y, deltaTime);
                velocityCurve.Z = CalculateLinearVelocity(velocityCurve.Z, deltaTime);
            }

            private VelocityCurveAxis CalculateLinearVelocity(VelocityCurveAxis velocityCurveAxis, float timeStep)
            {
                float newVelocity = 0.0f;
                float direction = velocityCurveAxis.IsPositive ? 1.0f : -1.0f;

                switch (velocityCurveAxis.Curve)
                {
                    case VelocityCurveTypes.Zero:
                        {
                            newVelocity = 0.0f;
                        }
                        break;

                    case VelocityCurveTypes.Linear:
                        {
                            newVelocity = velocityCurveAxis.AbsoluteVelocity * direction;
                        }
                        break;

                    case VelocityCurveTypes.Quadratic:
                        {
                            newVelocity = velocityCurveAxis.CurrentVelocity + (velocityCurveAxis.AbsoluteAcceleration * direction * timeStep);
                        }
                        break;

                    case VelocityCurveTypes.LinearThenQuadratic:
                        {
                            //Play linear for a bit, then switch over to
                            //quadratic, using any remaining time on it
                            if (velocityCurveAxis.DelayTimeRemaining > 0.0f)
                            {
                                if (velocityCurveAxis.DelayTimeRemaining >= timeStep)
                                {
                                    velocityCurveAxis.DelayTimeRemaining = velocityCurveAxis.DelayTimeRemaining - timeStep;

                                    //Stay linear
                                    newVelocity = velocityCurveAxis.AbsoluteVelocity * direction;
                                }
                                else
                                {
                                    //Quadradic with remaining time
                                    float remainingTime = timeStep - velocityCurveAxis.DelayTimeRemaining;
                                    velocityCurveAxis.DelayTimeRemaining = 0.0f;

                                    newVelocity = velocityCurveAxis.CurrentVelocity + (velocityCurveAxis.AbsoluteAcceleration * direction * remainingTime);
                                }
                            }
                            else
                            {
                                //Full quadradic
                                newVelocity = velocityCurveAxis.CurrentVelocity + (velocityCurveAxis.AbsoluteAcceleration * direction * timeStep);
                            }
                        }
                        break;
                }


                //Apply limits
                if (velocityCurveAxis.IsPositive)
                {
                    newVelocity = math.min(newVelocity, velocityCurveAxis.MaximumAbsoluteVelocity * direction);
                }
                else
                {
                    newVelocity = math.max(newVelocity, velocityCurveAxis.MaximumAbsoluteVelocity * direction);
                }

                velocityCurveAxis.CurrentVelocity = newVelocity;

                return velocityCurveAxis;
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new VelocityCurveSystemJob();
            job.deltaTime = Time.deltaTime;

            return job.Schedule(this, inputDependencies);
        }
    }
}