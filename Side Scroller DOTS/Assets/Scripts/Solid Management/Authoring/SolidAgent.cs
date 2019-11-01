using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SolidManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SolidAgent : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SolidManagement.SolidAgent
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