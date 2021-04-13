using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

/// <summary> 角色同步系统 </summary>
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class CharacterSyncSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        Entities.ForEach((DynamicBuffer<CharacterSyncData> inputBuffer, ref MovableComponent movable, ref LookRotationComponent rotatable, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;
            inputBuffer.GetDataAtTick(tick, out CharacterSyncData input);
            //这里只需要将数据进行更改 
            movable.Direction = rotatable.Direction = new float3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        });
    }
}