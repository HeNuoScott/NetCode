using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

// 当客户端与网络id连接，进入游戏并告诉服务器也进入游戏
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class GoInGameClientSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        //Debug.Log("Client Updata");
        Entities.WithNone<NetworkStreamInGame>().ForEach((Entity ent, ref NetworkIdComponent id) =>
        {
            //Debug.Log("Client execute send GoInGameRequest");
            PostUpdateCommands.AddComponent<NetworkStreamInGame>(ent);
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent<GoInGameRequest>(req);
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent { TargetConnection = ent });
        });
    }
}
