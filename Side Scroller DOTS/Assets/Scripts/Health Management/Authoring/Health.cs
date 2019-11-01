using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.HealthManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class Health : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float CurrentHealth = 5.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.HealthManagement.Health
            {
                CurrentHealth = CurrentHealth,
                CurrentDamage = 0.0f
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}