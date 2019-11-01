# VelocityCurve
This is a small library for Unity DOTS. It allows for the manual control of velocity within Unity Physics (DOTS).

# Design Goals

This package simplifies the manual modification of an entity's movement over time.  This allows higher level systems like character controllers or force areas to define their behaviours in terms of curves as opposed to final values.

# Components and Buffers

```csharp
public struct VelocityCurve : IComponentData
{
    public VelocityCurveAxis X;
    public VelocityCurveAxis Y;
    public VelocityCurveAxis Z;
}

public struct VelocityCurveAxis
{
    public VelocityCurves Curve;
    public float CurrentVelocity;

    public float Acceleration;
    public float MaximumVelocity;
    public float MinimumVelocity;
    public float DelayTimeRemaining;
    etc...
}
```

The VelocityCurve component contains a VelocityCurveAxis for X, Y, and Z. VelocityCurveAxis allows you to specify the type of curve, the starting velocity, the acceleration (if relevant), and more.

```csharp
public enum VelocityCurves
{
    Zero,
    Linear,
    Quadratic,
    LinearThenQuadratic
}
```

The VelocityCurveBuffer is how a VelocityCurve is applied to a specific entity. This buffer is cleared and repopulated every frame. This way, we get a clean stateless approach to applying velocity curves to individual entities. This helps us avoid bugs and runaway velocities.

```csharp
public struct VelocityCurveBuffer : IBufferElementData
{
    public Entity VelocityCurveEntity;
}
```

# Systems

```csharp
public class VelocityCurveSystem : JobComponentSystem
```
The VelocityCurveSystem runs before Unity Physics. It computes the final velocity for each VelocityCurve.

```csharp
public class ExportVelocityCurveSystem : JobComponentSystem
```
The ExportVelocityCurveSystem then applies these values to each entity with a PhysicsVelocity component (from Unity Physics) and a VelocityCurveBuffer. All referenced VelocityCurve values are added up and applied to the PhysicsVelocity component. It then clears the VelocityCurveBuffer. Due to this, the system is stateless.


