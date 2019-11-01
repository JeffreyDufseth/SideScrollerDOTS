using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScroller.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SideScrollingCharacterController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SideScroller.SideScrollingCharacterController
            {

            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}