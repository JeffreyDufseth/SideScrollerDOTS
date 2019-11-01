using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScrollerManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class LockAxes : MonoBehaviour, IConvertGameObjectToEntity
    {
        public bool ShouldLockX;
        public bool ShouldLockY;
        public bool ShouldLockZ;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            //Any locked axis is locked to it's initial position in the editor

            var componentData = new JeffreyDufseth.SideScrollerManagement.LockAxes
            {
                ShouldLockX = ShouldLockX,
                LockX = transform.position.x,

                ShouldLockY = ShouldLockY,
                LockY = transform.position.y,

                ShouldLockZ = ShouldLockZ,
                LockZ = transform.position.z
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}