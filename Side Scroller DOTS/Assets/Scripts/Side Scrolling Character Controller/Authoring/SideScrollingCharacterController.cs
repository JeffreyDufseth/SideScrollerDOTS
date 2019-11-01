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

        public float JumpAbsoluteDeceleration = 42.0f;
        public float JumpAbsoluteVelocity = 22.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SideScroller.SideScrollingCharacterController
            {
                FallingAbsoluteAcceleration = FallingAbsoluteAcceleration,
                TerminalVelocity = TerminalVelocity,

                JumpAbsoluteDeceleration = JumpAbsoluteDeceleration,
                JumpAbsoluteVelocity = JumpAbsoluteVelocity
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}