using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScroller.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class SideScrollingCharacterController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float FallingAbsoluteAcceleration = 85.0f;
        public float TerminalVelocity = 15.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SideScroller.SideScrollingCharacterController
            {
                FallingAbsoluteAcceleration = FallingAbsoluteAcceleration,
                TerminalVelocity = TerminalVelocity
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}