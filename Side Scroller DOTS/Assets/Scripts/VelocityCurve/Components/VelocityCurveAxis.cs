using Unity.Entities;

namespace JeffreyDufseth.VelocityCurves
{
    public enum VelocityCurveTypes
    {
        Zero,
        Linear,
        Quadratic,
        LinearThenQuadratic
    }

    public struct VelocityCurveAxis
    {
        public VelocityCurveTypes Curve;
        public float CurrentVelocity;

        public float Acceleration;
        public float MaximumVelocity;
        public float MinimumVelocity;
        public float DelayTimeRemaining;


        //Defaults and helper methods
        public static VelocityCurveAxis Zero = new VelocityCurveAxis
        {
            Acceleration = 0,
            CurrentVelocity = 0,
            Curve = VelocityCurveTypes.Zero,
            DelayTimeRemaining = 0,
            MaximumVelocity = 0,
            MinimumVelocity = 0
        };
    }
}