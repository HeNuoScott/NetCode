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