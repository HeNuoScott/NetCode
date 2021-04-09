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
            //����Ԥ����
            for (int ghostId = 0; ghostId < prefabs.Length; ++ghostId)
            {
                if (EntityManager.HasComponent<RedCubeComponent>(prefabs[ghostId].Value))
                    prefab = prefabs[ghostId].Value;
            }
            // ʵ����
            var player = EntityManager.Instantiate(prefab);
            // ����ӵ����
            EntityManager.SetComponentData(player, new GhostOwnerComponent { NetworkId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value });

            PostUpdateCommands.DestroyEntity(entity);

        });
    }
}