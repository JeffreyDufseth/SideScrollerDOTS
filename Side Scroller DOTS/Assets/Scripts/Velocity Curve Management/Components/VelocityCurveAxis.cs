using Unity.Entities;

namespace JeffreyDufseth.VelocityCurveManagement
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

        public bool IsPositive;
        public float AbsoluteVelocity;
        public float AbsoluteAcceleration;
        public float MaximumAbsoluteVelocity;
        public float DelayTimeRemaining;


        //Defaults and helper methods
        public static VelocityCurveAxis Zero = new VelocityCurveAxis
        {
            AbsoluteAcceleration = 0,
            CurrentVelocity = 0,
            Curve = VelocityCurveTypes.Zero,
            DelayTimeRemaining = 0,
            IsPositive = false,
            MaximumAbsoluteVelocity = 0
        };

        //Linear
        public static VelocityCurveAxis Linear(float currentVelocity,
                                                bool isPositive,
                                                float absoluteVelocity,
                                                float maximumAbsoluteVelocity)
        {
            return new VelocityCurveAxis
            {
                AbsoluteAcceleration = 0.0f,
                AbsoluteVelocity = absoluteVelocity,
                CurrentVelocity = currentVelocity,
                Curve = VelocityCurveTypes.Linear,
                DelayTimeRemaining = 0.0f,
                IsPositive = isPositive,
                MaximumAbsoluteVelocity = maximumAbsoluteVelocity
            };
        }

        //Quadratic
        public static VelocityCurveAxis Quadratic(  float currentVelocity,
                                                    bool isPositive,
                                                    float absoluteAcceleration,
                                                    float maximumAbsoluteVelocity)
        {
            return new VelocityCurveAxis
            {
                AbsoluteAcceleration = absoluteAcceleration,
                AbsoluteVelocity = 0.0f,
                CurrentVelocity = currentVelocity,
                Curve = VelocityCurveTypes.Quadratic,
                DelayTimeRemaining = 0.0f,
                IsPositive = isPositive,
                MaximumAbsoluteVelocity = maximumAbsoluteVelocity
            };
        }

        //LinearThenQuadratic
        public static VelocityCurveAxis LinearThenQuadratic(float currentVelocity,
                                                            bool isPositive,
                                                            float absoluteVelocity,
                                                            float absoluteAcceleration,
                                                            float delayTimeRemaining,
                                                            float maximumAbsoluteVelocity)
        {
            return new VelocityCurveAxis
            {
                AbsoluteAcceleration = absoluteAcceleration,
                AbsoluteVelocity = absoluteVelocity,
                CurrentVelocity = currentVelocity,
                Curve = VelocityCurveTypes.LinearThenQuadratic,
                DelayTimeRemaining = delayTimeRemaining,
                IsPositive = isPositive,
                MaximumAbsoluteVelocity = maximumAbsoluteVelocity
            };
        }
    }
}