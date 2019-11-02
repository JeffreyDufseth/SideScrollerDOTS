using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScrollerManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class MovingPlatformController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Transform PositionA;
        public Transform PositionB;
        public bool IsMovingTowardsA = true;
        public float AbsoluteVelocity = 5.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            if ((PositionA != null)
                && (PositionB != null))
            {
                var componentData = new JeffreyDufseth.SideScrollerManagement.MovingPlatformController
                {
                    PositionA = PositionA.position,
                    PositionB = PositionB.position,
                    IsMovingTowardsA = IsMovingTowardsA,
                    AbsoluteVelocity = AbsoluteVelocity
                };
                dstManager.AddComponentData(entity, componentData);
            }
        }
    }
}