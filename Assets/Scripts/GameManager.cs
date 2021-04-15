using Unity.Networking.Transport;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("需要在场景中隐藏")]
    [SerializeField] private GameObject ShareData;

    private void Start()
    {
        Application.targetFrameRate = 60;

        World.DefaultGameObjectInjectionWorld.GetExistingSystem<GoInitSystem>().actions.Enqueue(() =>
        {
#if UNITY_CLIENT
            // 创建客户端世界
            var clientWorld = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, "ClientWorld");
#elif UNITY_SERVER
            // 创建服务端世界
            var serverWorld = ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, "ServerWorld");
#elif UNITY_EDITOR
            // 创建服务端世界
            var serverWorld = ClientServerBootstrap.CreateServerWorld(World.DefaultGameObjectInjectionWorld, "ServerWorld");
            // 创建客户端世界
            var clientWorld = ClientServerBootstrap.CreateClientWorld(World.DefaultGameObjectInjectionWorld, "ClientWorld");
#endif
        });

        World.DefaultGameObjectInjectionWorld.GetExistingSystem<GoInitSystem>().actions.Enqueue(() =>
        {
            //必须立即初始化GhostPrefabCollectionComponent组件 不然在服务器初始化和客户端初始化是找不到对应的共享预制体
            ShareData.SetActive(true);
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
                else if (world.GetExistingSystem<ServerSimulationSystemGroup>() != null)
                {
                    // 服务器自动监听端口
                    NetworkEndPoint ep = NetworkEndPoint.AnyIpv4;
                    ep.Port = 7979;
                    network.Listen(ep);
                }
            }
        });
    }

    private void OnDisable()
    {
        List<World> worldsToDispose = new List<World>();
        foreach (var world in World.All)
        {
            if (world.Name.Equals("ServerWorld") || world.Name.Equals("ClientWorld"))
            {
                worldsToDispose.Add(world);
            }
        }
        foreach (var world in worldsToDispose)
        {
            Debug.Log("Disposing world " + world.Name);
            world.EntityManager.CompleteAllJobs();
            world.Dispose();
        }
    }
}