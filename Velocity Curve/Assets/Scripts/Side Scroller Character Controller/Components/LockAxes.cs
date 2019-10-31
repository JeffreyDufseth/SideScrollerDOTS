using Unity.Entities;

namespace JeffreyDufseth.SideScroller
{
    public struct LockAxes : IComponentData
    {
        public bool ShouldLockX;
        public float LockX;

        public bool ShouldLockY;
        public float LockY;

        public bool ShouldLockZ;
        public float LockZ;
    }
}