using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;
using UnityEngine.Events;

[UpdateInWorld(UpdateInWorld.TargetWorld.Default)]
[UpdateInGroup(typeof(InitializationSystemGroup))]
public class GoInitSystem : ComponentSystem
{
    public Queue<Action> actions = new Queue<Action>();

    protected override void OnCreate()
    {
        base.OnCreate();
        EntityManager.CreateEntity(typeof(ClientServerTickRate));
        SetSingleton<ClientServerTickRate>(new ClientServerTickRate
        {
            MaxSimulationStepsPerFrame = 60,
            NetworkTickRate = 60,
            SimulationTickRate = 60,
            TargetFrameRateMode = ClientServerTickRate.FrameRateMode.Auto
        });
    }

    protected override void OnUpdate()
    {
        while (actions.Count > 0)
        {
            actions.Dequeue()();
        }
    }
}

/// <summary>
/// 修改默认引导,使其不自动创建客户端和服务端世界
/// </summary>
public class CustomBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        TypeManager.Initialize();

        var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        GenerateSystemLists(systems);

        var world = new World(defaultWorldName);
        World.DefaultGameObjectInjectionWorld = world;

        DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, ExplicitDefaultWorldSystems);
#pragma warning disable 0618
        ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        return true;
    }
}