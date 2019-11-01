using Unity.Entities;

namespace JeffreyDufseth.VelocityCurves
{
    public struct VelocityCurve : IComponentData
    {
        public VelocityCurveAxis X;
        public VelocityCurveAxis Y;
        public VelocityCurveAxis Z;
    }
}