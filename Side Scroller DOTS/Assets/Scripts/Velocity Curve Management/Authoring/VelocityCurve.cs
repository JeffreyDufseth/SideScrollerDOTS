using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.VelocityCurveManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VelocityCurve : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.VelocityCurveManagement.VelocityCurve
            {
                X = VelocityCurveAxis.Zero(),
                Y = VelocityCurveAxis.Zero(),
                Z = VelocityCurveAxis.Zero()
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}