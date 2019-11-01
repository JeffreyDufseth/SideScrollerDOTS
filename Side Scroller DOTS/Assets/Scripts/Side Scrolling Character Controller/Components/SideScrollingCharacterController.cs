using Unity.Entities;

namespace JeffreyDufseth.SideScroller
{
    public struct SideScrollingCharacterController : IComponentData
    {
        public float FallingAbsoluteAcceleration;
        public float TerminalVelocity;

        public bool IsFalling;
    }
}