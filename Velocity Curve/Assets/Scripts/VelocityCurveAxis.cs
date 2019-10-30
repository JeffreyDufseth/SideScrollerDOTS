using Unity.Entities;

namespace JeffreyDufseth.VelocityCurve
{
    public enum VelocityCurves
    {
        Zero,
        Linear,
        Quadratic,
        LinearThenQuadratic
    }

    public struct VelocityCurveAxis
    {
        public VelocityCurves Curve;
        public float CurrentVelocity;

        public float Acceleration;
        public float MaximumVelocity;
        public float MinimumVelocity;
        public float DelayTimeRemaining;
    }
}