﻿using Unity.Entities;
using Unity.NetCode;
using System;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class GoInGameServerSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //UnityEngine.Debug.Log("Server Updata");
        Entities.WithNone<SendRpcCommandRequestComponent>().ForEach((Entity reqEnt, ref GoInGameRequest req, ref ReceiveRpcCommandRequestComponent reqSrc) =>
        {
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(reqSrc.SourceConnection);
            UnityEngine.Debug.Log(String.Format("Server setting connection {0} to in game", EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value));
            var ghostCollection = GetSingletonEntity<GhostPrefabCollectionComponent>();
            var prefab = Entity.Null;
            var prefabs = EntityManager.GetBuffer<GhostPrefabBuffer>(ghostCollection);
            for (int ghostId = 0; ghostId < prefabs.Length; ++ghostId)
            {
                if (EntityManager.HasComponent<CharacterTag>(prefabs[ghostId].Value))
                    prefab = prefabs[ghostId].Value;
            }
            var player = EntityManager.Instantiate(prefab);
            EntityManager.SetComponentData(player, new MovableComponent() { Speed = 1f });
            EntityManager.SetComponentData(player, new GhostOwnerComponent { NetworkId = EntityManager.GetComponentData<NetworkIdComponent>(reqSrc.SourceConnection).Value });
            PostUpdateCommands.AddBuffer<CharacterSyncData>(player);

            PostUpdateCommands.SetComponent(reqSrc.SourceConnection, new CommandTargetComponent { targetEntity = player });

            PostUpdateCommands.DestroyEntity(reqEnt);
        });
    }
}
