using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using JeffreyDufseth.PlayerManagement;

namespace JeffreyDufseth.SideScrollerManagement.Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(TransformSystemGroup))]
    public class CameraFollowPlayerSystem : ComponentSystem
    {
        protected override void OnUpdate()
        {
            Entities.ForEach((ref Player player, ref Translation translation) =>
            {
                UnityEngine.Camera.main.transform.position = new UnityEngine.Vector3(translation.Value.x, translation.Value.y, UnityEngine.Camera.main.transform.position.z);
            });
        }
    }
}
