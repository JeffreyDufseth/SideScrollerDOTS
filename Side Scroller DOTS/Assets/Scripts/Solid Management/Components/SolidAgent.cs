using Unity.Entities;

namespace JeffreyDufseth.SolidManagement
{
    public struct SolidAgent : IComponentData
    {
        public bool IsGroundCollided;
        public float GroundSurfaceNormal;

        public bool IsCeilingCollided;
        public float CeilingSurfaceNormal;

        public bool IsLeftWallCollided;
        public float LeftWallSurfaceNormal;

        public bool IsRightWallCollided;
        public float RightWallSurfaceNormal;
    }
}