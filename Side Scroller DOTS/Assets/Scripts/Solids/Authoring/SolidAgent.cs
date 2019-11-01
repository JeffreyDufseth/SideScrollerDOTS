using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.Solids.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SolidAgent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.Solids.SolidAgent
            {
                IsCeilingCollided = false,
                IsGroundCollided = false,
                IsLeftWallCollided = false,
                IsRightWallCollided = false
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}