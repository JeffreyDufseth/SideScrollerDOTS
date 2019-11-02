using Unity.Entities;
using Unity.Collections;
using Unity.Physics;
using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using JeffreyDufseth.VelocityCurveManagement.Systems;
using JeffreyDufseth.VelocityCurveManagement;
using Unity.Mathematics;
using Unity.Transforms;

namespace JeffreyDufseth.SideScrollerManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateBefore(typeof(VelocityCurveSystem))]
    public class MovingPlatformControllerSystem : JobComponentSystem
    {
        [BurstCompile]
        struct MovingPlatformControllerSystemJob : IJobForEachWithEntity_EBCCC<VelocityCurveBuffer, VelocityCurve, MovingPlatformController, Translation>
        {
            public float DeltaTime;

            public void Execute(Entity entity, int index,
                                DynamicBuffer<VelocityCurveBuffer> velocityCurveBuffer,
                                ref VelocityCurve velocityCurve, ref MovingPlatformController movingPlatformController, ref Translation translation)
            {
                //The wind never blows on the Z axis
                velocityCurve.Z = VelocityCurveAxis.Zero();

                //Check if we've reached our current destination
                float3 destination = movingPlatformController.IsMovingTowardsA ? movingPlatformController.PositionA : movingPlatformController.PositionB;
                float distanceToDestination = math.abs(math.distance(translation.Value, destination));

                if (distanceToDestination == 0.0f)
                {
                    //Reverse the destination
                    movingPlatformController.IsMovingTowardsA = !movingPlatformController.IsMovingTowardsA;

                    destination = movingPlatformController.IsMovingTowardsA ? movingPlatformController.PositionA : movingPlatformController.PositionB;
                    distanceToDestination = math.abs(math.distance(translation.Value, destination));
                }



                //Move towards the destination, being carefull not to move past it
                float distanceToMoveThisTimeStep = DeltaTime * movingPlatformController.AbsoluteVelocity;
                float distanceToMoveThisTimeStepSquared = distanceToMoveThisTimeStep * distanceToMoveThisTimeStep;

                float3 distanceAvailableToMove = destination - translation.Value;
                float distanceAvailableToMoveSquared = math.lengthsq(distanceAvailableToMove);

                float3 directionToMove = math.normalize(distanceAvailableToMove);

                //TODO cleanup, this whole section is confusing!
                float3 linearVelocity = new float3();

                if (distanceAvailableToMoveSquared < distanceToMoveThisTimeStepSquared)
                {
                    linearVelocity = distanceAvailableToMove / DeltaTime;
                }
                else
                {
                    linearVelocity = directionToMove * movingPlatformController.AbsoluteVelocity;
                }


                //Set the velocity curve
                velocityCurve.X = VelocityCurveAxis.Linear( linearVelocity.x > 0.0f,
                                                            math.abs(linearVelocity.x));

                velocityCurve.Y = VelocityCurveAxis.Linear( linearVelocity.y > 0.0f,
                                                            math.abs(linearVelocity.y));


                //Add this velocity curve to the moving platform's velocity curve buffer
                velocityCurveBuffer.Add(new VelocityCurveBuffer
                {
                    VelocityCurveEntity = entity
                });
            }
        }


        protected override JobHandle OnUpdate(JobHandle inputDependencies)
        {
            var job = new MovingPlatformControllerSystemJob();
            job.DeltaTime = Time.fixedDeltaTime;

            return job.Schedule(this, inputDependencies);
        }
    }
}
