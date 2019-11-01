using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScrollerManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SideScrollingCharacterController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float FallingAbsoluteAcceleration = 85.0f;
        public float TerminalVelocity = 15.0f;

        public float JumpAbsoluteDeceleration = 42.0f;
        public float JumpAbsoluteVelocity = 22.0f;

        public float WalkingAbsoluteAcceleration = 21.0f;
        public float WalkingAbsoluteMaximumVelocity = 11.25f;
        public float WalkingAbsoluteDeceleration = 14.0f;
        public float SkiddingAbsoluteDeceleration = 35.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SideScrollerManagement.SideScrollingCharacterController
            {
                FallingAbsoluteAcceleration = FallingAbsoluteAcceleration,
                TerminalVelocity = TerminalVelocity,

                JumpAbsoluteDeceleration = JumpAbsoluteDeceleration,
                JumpAbsoluteVelocity = JumpAbsoluteVelocity,

                WalkingAbsoluteAcceleration = WalkingAbsoluteAcceleration,
                WalkingAbsoluteMaximumVelocity = WalkingAbsoluteMaximumVelocity,
                WalkingAbsoluteDeceleration = WalkingAbsoluteDeceleration,
                SkiddingAbsoluteDeceleration = SkiddingAbsoluteDeceleration
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}