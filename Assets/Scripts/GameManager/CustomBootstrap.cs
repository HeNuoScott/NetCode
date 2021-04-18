using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

/// <summary>
/// ECS 世界 启动引导
/// </summary>
public class CustomBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        CustomInitialize(defaultWorldName);

        return true;
    }
    // 自定义引导实现内容  使其不自动创建客户端和服务端世界
    private void CustomInitialize(string defaultWorldName)
    {
        // 为了拥有一个有效的TypeManager实例，必须在生成系统列表之前创建默认的世界。
        // 当我们第一次创建一个世界时，TypeManage被初始化。
        var world = new World(defaultWorldName, WorldFlags.Game);
        World.DefaultGameObjectInjectionWorld = world;
        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);
        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
#pragma warning disable 0618
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        
    }

    // 默认引导实现内容 进入客户端 和 服务器 世界
    private void DefaultInitialize(string defaultWorldName)
    {
        // 为了拥有一个有效的TypeManager实例，必须在生成系统列表之前创建默认的世界。
        // 当我们第一次创建一个世界时，TypeManage被初始化。
        var world = new World(defaultWorldName, WorldFlags.Game);
        World.DefaultGameObjectInjectionWorld = world;

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
#if !UNITY_DOTSRUNTIME
        ScriptBehaviourUpdateOrder.AddWorldToCurrentPlayerLoop(world);
#endif

        PlayType playModeType = RequestedPlayType;
        int numClientWorlds = 1;

        int totalNumClients = numClientWorlds;
        if (playModeType != PlayType.Server)
        {
#if UNITY_EDITOR
            int numThinClients = RequestedNumThinClients;
            totalNumClients += numThinClients;
#endif
            for (int i = 0; i < numClientWorlds; ++i)
            {
                CreateClientWorld(world, "ClientWorld" + i);
            }
#if UNITY_EDITOR
            for (int i = numClientWorlds; i < totalNumClients; ++i)
            {
                var clientWorld = CreateClientWorld(world, "ClientWorld" + i);
                clientWorld.EntityManager.CreateEntity(typeof(ThinClientComponent));
            }
#endif
        }

        if (playModeType != PlayType.Client)
        {
            CreateServerWorld(world, "ServerWorld");
        }
    }

}