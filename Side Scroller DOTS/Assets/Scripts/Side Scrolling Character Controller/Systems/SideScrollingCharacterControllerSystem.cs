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
using UnityEngine;
using JeffreyDufseth.Solids.Systems;

namespace JeffreyDufseth.SideScroller.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ResetSolidAgentsSystem))]
    [UpdateBefore(typeof(VelocityCurveSystem))]
    public class SideScrollingCharacterControllerSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SideScrollingCharacterControllerSystemJob : IJobForEachWithEntity_EBCCC<VelocityCurveBuffer, VelocityCurve, SideScrollingCharacterController, SolidAgent>
        {
            public bool IsJumpPressedThisFrame;
            public bool IsJumpHeld;

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
                    //We're grounded.
                    //Reset jumping and falling flags
                    sideScrollingCharacterController.IsJumping = false;
                    sideScrollingCharacterController.IsFalling = false;
                    sideScrollingCharacterController.IsJumpHeld = false;

                    //Check if we want to jump
                    if (IsJumpPressedThisFrame)
                    {
                        sideScrollingCharacterController.IsJumping = true;
                        sideScrollingCharacterController.IsJumpHeld = true;

                        //TODO determine jump velocity based on horizontal velocity
                        //This can be done with curves or cutoff points

                        velocityCurve.Y = VelocityCurveAxis.Quadratic(  sideScrollingCharacterController.JumpAbsoluteVelocity,
                                                                        false,
                                                                        sideScrollingCharacterController.JumpAbsoluteDeceleration,
                                                                        sideScrollingCharacterController.TerminalVelocity);
                    }
                    else
                    {
                        velocityCurve.Y = VelocityCurveAxis.Zero;
                    }
                }
                else
                {
                    //Differentiate between jumping and falling
                    if (!IsJumpPressedThisFrame)
                    {
                        sideScrollingCharacterController.IsJumpHeld = false;
                    }

                    //Did the player let go of jump?
                    if (sideScrollingCharacterController.IsJumping && !sideScrollingCharacterController.IsJumpHeld)
                    {
                        sideScrollingCharacterController.IsJumping = false;
                    }

                    //Check if we should start falling
                    if (!sideScrollingCharacterController.IsJumping && !sideScrollingCharacterController.IsFalling)
                    {
                        //Start Falling
                        sideScrollingCharacterController.IsFalling = true;

                        velocityCurve.Y = VelocityCurveAxis.Quadratic(velocityCurve.Y.CurrentVelocity,
                                                                        false,
                                                                        sideScrollingCharacterController.FallingAbsoluteAcceleration,
                                                                        sideScrollingCharacterController.TerminalVelocity);
                    }
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
            job.IsJumpPressedThisFrame = Input.GetButtonDown("Jump");
            job.IsJumpHeld = Input.GetButton("Jump");

            return job.Schedule(this, inputDependencies);
        }
    }
}
