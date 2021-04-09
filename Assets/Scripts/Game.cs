using Unity.Networking.Transport;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

// Control system updating in the default world
[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
public class Game : ComponentSystem
{
    struct InitGameComponent : IComponentData
    {
    }
    protected override void OnCreate()
    {
        Application.targetFrameRate = 60;
        RequireSingletonForUpdate<InitGameComponent>();
        // 创建一个单例 并且在更新中只运行一次
        EntityManager.CreateEntity(typeof(InitGameComponent));
    }

    protected override void OnUpdate()
    {
        // 销毁存在的单例 以免在次运行
        EntityManager.DestroyEntity(GetSingletonEntity<InitGameComponent>());
        foreach (var world in World.All)
        {
            var network = world.GetExistingSystem<NetworkStreamReceiveSystem>();
            if (world.GetExistingSystem<ClientSimulationSystemGroup>() != null)
            {
                // 客户端连接服务器端口
                NetworkEndPoint ep = NetworkEndPoint.LoopbackIpv4;
                ep.Port = 7979;
                network.Connect(ep);
            }
#if UNITY_EDITOR
            else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
            {
                // 服务器自动监听端口
                NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                ep.Port = 7979;
                network.Listen(ep);
            }
#endif
        }
    }
}