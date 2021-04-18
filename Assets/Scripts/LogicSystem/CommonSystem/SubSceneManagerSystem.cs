using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Scenes;
using UnityEngine;
using UnityEngine.Events;

[UpdateInWorld(UpdateInWorld.TargetWorld.ClientAndServer)]
[UpdateInGroup(typeof(ClientAndServerSimulationSystemGroup))]
public class SubSceneManagerSystem : ComponentSystem
{
    private SceneSystem sceneSystem;

    protected override void OnCreate()
    {
        sceneSystem = World.GetOrCreateSystem<SceneSystem>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((Entity entity, ref SubsceneChangeData subSceneData) =>
        {
            if (subSceneData.isLoadOrUnload)
            {
                // 加载场景
                sceneSystem.LoadSceneAsync(subSceneData.sceneGUID);
                UnityEngine.Debug.Log("场景加载!");
            }
            else
            {
                // 卸载场景
                sceneSystem.UnloadScene(subSceneData.sceneGUID);
                UnityEngine.Debug.Log("场景卸载!");
            }
            PostUpdateCommands.DestroyEntity(entity);
        });
    }
}
