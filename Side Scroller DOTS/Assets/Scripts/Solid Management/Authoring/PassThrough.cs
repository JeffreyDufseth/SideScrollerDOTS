using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SolidManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class PassThrough : MonoBehaviour, IConvertGameObjectToEntity
    {
        public PassThroughDirections Directions;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SolidManagement.PassThrough
            {
                Directions = Directions
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}