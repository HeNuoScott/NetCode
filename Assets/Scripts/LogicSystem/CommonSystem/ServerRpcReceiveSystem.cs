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
            if (cmd.skillId==(int)KeyCode.Space)
            {
                Debug.LogError("We received a command!    KeyCode.Space");
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
            }
            else if (cmd.skillId == (int)KeyCode.Q)
            {
                var networkId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value;
                Entities.WithAll<CharacterTag>().ForEach((Entity ent,ref MovableComponent move, ref GhostOwnerComponent ghostOwner) => 
                {
                    if (ghostOwner.NetworkId == networkId) move.Speed = 0f;
                });
            }
            else if (cmd.skillId == (int)KeyCode.E)
            {
                var networkId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value;
                Entities.WithAll<CharacterTag>().ForEach((Entity ent, ref MovableComponent move, ref GhostOwnerComponent ghostOwner) =>
                {
                    if (ghostOwner.NetworkId == networkId) move.Speed = 5f;
                });
            }
            else if (cmd.skillId == (int)KeyCode.R)
            {
                var networkId = EntityManager.GetComponentData<NetworkIdComponent>(req.SourceConnection).Value;
                Entities.WithAll<CharacterTag>().ForEach((Entity ent, ref MovableComponent move, ref GhostOwnerComponent ghostOwner) =>
                {
                    if (ghostOwner.NetworkId == networkId) move.Speed = 1f;
                });
            }

            PostUpdateCommands.DestroyEntity(entity);

        });
    }
}
