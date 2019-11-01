using Unity.Entities;

namespace JeffreyDufseth.SideScrollerManagement
{
    public struct SideScrollingCharacterController : IComponentData
    {
        public float FallingAbsoluteAcceleration;
        public float TerminalVelocity;

        public float JumpAbsoluteVelocity;
        public float JumpAbsoluteDeceleration;

        public float WalkingAbsoluteAcceleration;
        public float WalkingAbsoluteMaximumVelocity;
        public float WalkingAbsoluteDeceleration;
        public float SkiddingAbsoluteDeceleration;

        public bool IsJumping;
        public bool IsJumpHeld;
        public bool IsFalling;
    }
}