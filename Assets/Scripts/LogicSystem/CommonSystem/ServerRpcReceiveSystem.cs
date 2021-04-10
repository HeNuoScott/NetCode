using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class ServerRpcReceiveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref ReleaseSkillRequest cmd, ref ReceiveRpcCommandRequestComponent req) =>
        {
            
            Debug.LogError("We received a command!");
            var ghostCollection = GetSingletonEntity<GhostPrefabCollectionComponent>();
            var prefab = Entity.Null;
            var prefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
            // 查找预制体
            for (int ghostId = 0; ghostId < prefabs.Length; ++ghostId)
            {
                if (EntityManager.HasComponent<RedCubeComponent>(prefabs[ghostId].Value))
                    prefab = prefabs[ghostId].Value;
            }
            // 实例化
            var player = EntityManager.Instantiate(prefab);
            // 网络id
            var networkId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value;
            // 设置拥有者
            EntityManager.SetComponentData(player, new GhostOwnerComponent { NetworkId = networkId });

            PostUpdateCommands.DestroyEntity(entity);

        });
    }
}
