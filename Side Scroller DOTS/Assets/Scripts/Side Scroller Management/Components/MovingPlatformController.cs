using Unity.Entities;
using Unity.Mathematics;

namespace JeffreyDufseth.SideScrollerManagement
{
    public struct MovingPlatformController : IComponentData
    {
        public float3 PositionA;
        public float3 PositionB;
        public bool IsMovingTowardsA;
        public float AbsoluteVelocity;
    }
}