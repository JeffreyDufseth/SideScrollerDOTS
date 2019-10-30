using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.VelocityCurve.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VelocityCurve : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.VelocityCurve.VelocityCurve
            {
                X = VelocityCurveAxis.Zero,
                Y = VelocityCurveAxis.Zero,
                Z = VelocityCurveAxis.Zero
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}