using Unity.Entities;

namespace JeffreyDufseth.VelocityCurveManagement
{
    public struct VelocityCurve : IComponentData
    {
        public VelocityCurveAxis X;
        public VelocityCurveAxis Y;
        public VelocityCurveAxis Z;
    }
}