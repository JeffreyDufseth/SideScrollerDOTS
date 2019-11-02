using Unity.Entities;
using Unity.Mathematics;

namespace JeffreyDufseth.SideScrollerManagement
{
    public struct WindController : IComponentData
    {
        public float2 WindDirection;
        public bool IsBlowing;

        public float WindAbsoluteAcceleration;
        public float WindAbsoluteDeceleration;

        public float WindAbsoluteMaximumVelocity;

        public float OnTime;
        public float OffTime;
        public float ElapsedTime;
    }
}