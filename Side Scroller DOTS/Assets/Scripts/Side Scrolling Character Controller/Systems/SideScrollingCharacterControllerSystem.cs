using System;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;
using Unity.Transforms;
using JeffreyDufseth.VelocityCurves;
using JeffreyDufseth.VelocityCurves.Systems;
using Unity.Burst;
using Unity.Jobs;
using JeffreyDufseth.Solids;

namespace JeffreyDufseth.SideScroller.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(VelocityCurveSystem))]
    public class SideScrollingCharacterControllerSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SideScrollingCharacterControllerSystemJob : IJobForEachWithEntity_EBCCC<VelocityCurveBuffer, VelocityCurve, SideScrollingCharacterController, SolidAgent>
        {
            public void Execute(Entity entity, int index,
                                DynamicBuffer<VelocityCurveBuffer> velocityCurveBuffer,
                                ref VelocityCurve velocityCurve,
                                ref SideScrollingCharacterController sideScrollingCharacterController,
                                [ReadOnly] ref SolidAgent solidAgent)
            {
                //Z axis is always 0
                velocityCurve.Z = VelocityCurveAxis.Zero;

                //Check if we're grounded
                if (solidAgent.IsGroundCollided)
                {
                    //TODO
                    velocityCurve.Y = VelocityCurveAxis.Zero;
                }
                else
                {
                    //Start Falling
                    sideScrollingCharacterController.IsFalling = true;

                    velocityCurve.Y = VelocityCurveAxis.Quadratic(  velocityCurve.Y.CurrentVelocity,
                                                                    false,
                                                                    sideScrollingCharacterController.FallingAbsoluteAcceleration,
                                                                    sideScrollingCharacterController.TerminalVelocity);
                }

                //Horizontal Movement
                //TODO
                velocityCurve.X = VelocityCurveAxis.Zero;

                //Vertical Movement
                //TODO

                //TODO for testing purposes, set the forward curve

                //velocityCurve.X = new VelocityCurveAxis
                //{
                //    Acceleration = 5.0f,
                //    CurrentVelocity = velocityCurve.X.CurrentVelocity,
                //    Curve = VelocityCurveTypes.Quadratic,
                //    DelayTimeRemaining = 0.0f,
                //    MaximumVelocity = 50.0f,
                //    MinimumVelocity = 0.0f
                //};

                //Add this velocity curve to the character's velocity curve buffer
                velocityCurveBuffer.Add(new VelocityCurveBuffer
                {
                    VelocityCurveEntity = entity
                });
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new SideScrollingCharacterControllerSystemJob();

            return job.Schedule(this, inputDependencies);
        }
    }
}
