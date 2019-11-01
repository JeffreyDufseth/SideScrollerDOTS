using Unity.Entities;
using Unity.Mathematics;
using JeffreyDufseth.HealthManagement.Systems;
using JeffreyDufseth.HealthManagement;
using Unity.Collections;
using UnityEngine.SceneManagement;

namespace JeffreyDufseth.PlayerManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(DamageAreaTriggerSystem))]
    public class RespawnPlayerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            //TODO this is all pretty rough and lacks nuance

            bool shouldRespawn = false;

            Entities.ForEach((ref Player player, ref Health health) =>
            {
                if (health.CurrentDamage >= health.CurrentHealth)
                {
                    //Destroy all entities and reload the scene
                    shouldRespawn = true;
                }
            });

            if (shouldRespawn)
            {
                NativeArray<Entity> allEntities = World.Active.EntityManager.GetAllEntities(Unity.Collections.Allocator.Temp);
                World.Active.EntityManager.DestroyEntity(allEntities);
                allEntities.Dispose();

                SceneManager.LoadScene(0);
            }
        }
    }
}
