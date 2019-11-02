using System;
using Unity.Entities;

namespace JeffreyDufseth.SolidManagement
{
    [Flags]
    public enum PassThroughDirections
    {
        Left = 1 << 0,
        Top = 1 << 1,
        Right = 1 << 2,
        Bottom = 1 << 3
    }

    public struct PassThrough : IComponentData
    {
        public PassThroughDirections Directions;
    }
}