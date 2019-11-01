using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.VelocityCurves.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class VelocityCurveBuffer : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddBuffer<JeffreyDufseth.VelocityCurves.VelocityCurveBuffer>(entity);
        }
    }
}