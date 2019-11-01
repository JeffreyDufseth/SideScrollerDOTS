using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace JeffreyDufseth.HealthManagement.Authoring
{
    [DisallowMultipleComponent]
    [RequiresEntityConversion]
    public class DamageArea : MonoBehaviour, IConvertGameObjectToEntity
    {
        public float DamagePerFrame = 100.0f;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            var componentData = new JeffreyDufseth.HealthManagement.DamageArea
            {
                DamagePerFrame = DamagePerFrame
            };
            dstManager.AddComponentData(entity, componentData);
        }
    }
}