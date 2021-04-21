using Unity.Entities;
using Unity.NetCode;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class GoInGameServerSystem : ComponentSystem
{
    public uint connectedPlayers = 0;

    private List<uint> freeSpawnLocationIndices;

    protected override void OnCreate()
    {
        freeSpawnLocationIndices = new List<uint>();
        for (uint i = 0; i < GameSession.serverSession.numberOfPlayers; i++)
        {
            freeSpawnLocationIndices.Add(i);
        }
    }
    protected override void OnUpdate()
    {
        //UnityEngine.Debug.Log("Server Updata");
        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref GoInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            if (connectedPlayers < GameSession.serverSession.numberOfPlayers)
            {
                PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
                var sourceConnectionNetworkId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value;
                UnityEngine.Debug.Log(String.Format("Server setting connection {0} to in game", sourceConnectionNetworkId));
                var ghostCollection = GetSingletonEntity<GhostPrefabCollectionComponent>();
                var prefab = Entity.Null;
                var prefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
                for (int ghostId = 0; ghostId < prefabs.Length; ++ghostId)
                {
                    if (EntityManager.HasComponent<CharacterTag>(prefabs[ghostId].Value))
                        prefab = prefabs[ghostId].Value;
                }
                // 随机生成位置索引
                int indexOfNextFreeSpawnLocationIndex = Random.Range(0, freeSpawnLocationIndices.Count);
                uint nextSpawnLocationIndex = freeSpawnLocationIndices[indexOfNextFreeSpawnLocationIndex];
                freeSpawnLocationIndices.RemoveAt(indexOfNextFreeSpawnLocationIndex);

                var player = EntityManager.Instantiate(prefab);
                EntityManager.SetComponentData(player, new MovableComponent() { Speed = 1f });
                EntityManager.SetComponentData(player, new GhostOwnerComponent { NetworkId = sourceConnectionNetworkId });
                PostUpdateCommands.AddBuffer<CharacterSyncData>(player);

                PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });

                PostUpdateCommands.DestroyEntity(reqEnt);
            }
        });
    }
}
