using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using JeffreyDufseth.SolidManagement.Systems;
using JeffreyDufseth.VelocityCurveManagement.Systems;
using JeffreyDufseth.VelocityCurveManagement;
using JeffreyDufseth.SolidManagement;

namespace JeffreyDufseth.SideScrollerManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(ResetSolidAgentsSystem))]
    [UpdateBefore(typeof(VelocityCurveSystem))]
    public class SideScrollingCharacterControllerSystem : JobComponentSystem
    {
        [BurstCompile]
        struct SideScrollingCharacterControllerSystemJob : IJobForEachWithEntity_EBCCC<VelocityCurveBuffer, VelocityCurve, SideScrollingCharacterController, SolidAgent>
        {
            public float DeltaTime;
            public bool IsJumpPressedThisFrame;
            public bool IsJumpHeld;
            public float2 MovementInput;

            public void Execute(Entity entity, int index,
                                DynamicBuffer<VelocityCurveBuffer> velocityCurveBuffer,
                                ref VelocityCurve velocityCurve,
                                ref SideScrollingCharacterController sideScrollingCharacterController,
                                [ReadOnly] ref SolidAgent solidAgent)
            {
                //Z axis is always 0
                velocityCurve.Z = VelocityCurveAxis.Zero;

                //Vertical Movement
                ComputeVerticalMovement(ref velocityCurve,
                                        ref sideScrollingCharacterController,
                                        ref solidAgent);

                //Horizontal Movement
                ComputeHorizontalMovement(  ref velocityCurve,
                                            ref sideScrollingCharacterController,
                                            ref solidAgent);

                //Add this velocity curve to the character's velocity curve buffer
                velocityCurveBuffer.Add(new VelocityCurveBuffer
                {
                    VelocityCurveEntity = entity
                });
            }

            private void ComputeVerticalMovement(ref VelocityCurve velocityCurve,
                                                 ref SideScrollingCharacterController sideScrollingCharacterController,
                                                 [ReadOnly] ref SolidAgent solidAgent)
            {
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

                        velocityCurve.Y = VelocityCurveAxis.Quadratic(sideScrollingCharacterController.JumpAbsoluteVelocity,
                                                                        false,
                                                                        sideScrollingCharacterController.JumpAbsoluteDeceleration,
                                                                        sideScrollingCharacterController.TerminalVelocity);
                    }
                    else
                    {
                        //While grounded, continue to push down on the ground an amount
                        //equal to a single step off acceleration
                        velocityCurve.Y = VelocityCurveAxis.Linear( false,
                                                                    sideScrollingCharacterController.FallingAbsoluteAcceleration * DeltaTime);
                    }
                }
                else
                {
                    //Check for hitting your head on the ceiling
                    if ((velocityCurve.Y.CurrentVelocity > 0.0f)
                        && (solidAgent.IsCeilingCollided))
                    {
                        //Cancel the jump and reduce velocity to zero
                        velocityCurve.Y.CurrentVelocity = 0.0f;
                        sideScrollingCharacterController.IsJumpHeld = false;
                    }

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
            }

            private void ComputeHorizontalMovement( ref VelocityCurve velocityCurve,
                                                    ref SideScrollingCharacterController sideScrollingCharacterController,
                                                    [ReadOnly] ref SolidAgent solidAgent)
            {
                //Check for hitting a wall you're moving towards
                if (((velocityCurve.X.CurrentVelocity < 0.0f)
                    && (solidAgent.IsLeftWallCollided))
                    
                    ||

                    ((velocityCurve.X.CurrentVelocity > 0.0f)
                    && (solidAgent.IsRightWallCollided)))
                {
                    //We hit a wall.
                    //Reduce our current X velocity to zero
                    velocityCurve.X.CurrentVelocity = 0.0f;
                }

                //Move based on the movement input
                if (MovementInput.x == 0.0f)
                {
                    //The player isn't holding any direction.
                    //In the air, do nothing.
                    //On the ground, skid to a halt
                    if (solidAgent.IsGroundCollided)
                    {
                        velocityCurve.X = VelocityCurveAxis.Quadratic(velocityCurve.X.CurrentVelocity,
                                                                        velocityCurve.X.CurrentVelocity > 0.0f,
                                                                        sideScrollingCharacterController.WalkingAbsoluteDeceleration,
                                                                        0.0f);
                    }
                }
                else
                {
                    //Moving left and right
                    //Check if we're moving with the momentum or against it
                    if (((MovementInput.x > 0.0f)
                        && (velocityCurve.X.CurrentVelocity < 0.0f))
                        
                        ||

                        ((MovementInput.x < 0.0f)
                        && (velocityCurve.X.CurrentVelocity > 0.0f)))
                    {
                        //The player is skidding back towards zero velocity
                        velocityCurve.X = VelocityCurveAxis.Quadratic(  velocityCurve.X.CurrentVelocity,
                                                                        MovementInput.x > 0.0f,
                                                                        sideScrollingCharacterController.SkiddingAbsoluteDeceleration,
                                                                        0.0f);
                    }
                    else
                    {
                        //The player is accelerating towards maximum velocity
                        velocityCurve.X = VelocityCurveAxis.Quadratic(  velocityCurve.X.CurrentVelocity,
                                                                        MovementInput.x > 0.0f,
                                                                        sideScrollingCharacterController.WalkingAbsoluteAcceleration,
                                                                        sideScrollingCharacterController.WalkingAbsoluteMaximumVelocity);
                    }
                }
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new SideScrollingCharacterControllerSystemJob();
            job.DeltaTime = Time.fixedDeltaTime;
            job.IsJumpPressedThisFrame = Input.GetButtonDown("Jump");
            job.IsJumpHeld = Input.GetButton("Jump");
            job.MovementInput = new float2
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = Input.GetAxisRaw("Vertical")
            };

            return job.Schedule(this, inputDependencies);
        }
    }
}
