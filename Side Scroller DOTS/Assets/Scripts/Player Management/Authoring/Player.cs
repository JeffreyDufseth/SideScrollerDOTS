using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.PlayerManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class Player : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.PlayerManagement.Player
            {

            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}