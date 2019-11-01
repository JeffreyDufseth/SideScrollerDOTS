using Unity.Entities;

namespace JeffreyDufseth.SideScroller
{
    public struct SideScrollingCharacterController : IComponentData
    {
        public float FallingAbsoluteAcceleration;
        public float TerminalVelocity;

        public float JumpAbsoluteVelocity;
        public float JumpAbsoluteDeceleration;

        public bool IsJumping;
        public bool IsJumpHeld;
        public bool IsFalling;
    }
}