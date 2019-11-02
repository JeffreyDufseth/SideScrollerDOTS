using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.SideScrollerManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class WindController : MonoBehaviour, IConvertGameObjectToEntity
    {
        public Vector2 WindDirection;

        public float WindAbsoluteAcceleration = 20.0f;
        public float WindAbsoluteDeceleration = 50.0f;
        public float WindAbsoluteMaximumVelocity = 5.0f;

        public float OnTime = 5.0f;
        public float OffTime = 5.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.SideScrollerManagement.WindController
            {
                WindDirection = WindDirection.normalized,
                IsBlowing = false,

                WindAbsoluteAcceleration = WindAbsoluteAcceleration,
                WindAbsoluteDeceleration = WindAbsoluteDeceleration,

                WindAbsoluteMaximumVelocity = WindAbsoluteMaximumVelocity,

                OnTime = OnTime,
                OffTime = OffTime,
                ElapsedTime = 0.0f
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}