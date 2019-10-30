using Unity.Entities;

namespace JeffreyDufseth.VelocityCurve
{
    public struct VelocityCurve : IComponentData
    {
        public VelocityCurveAxis X;
        public VelocityCurveAxis Y;
        public VelocityCurveAxis Z;
    }
}