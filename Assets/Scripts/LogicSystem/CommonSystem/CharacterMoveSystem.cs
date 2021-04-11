using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

/// <summary> 角色移动系统 </summary>
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class CharacterMoveSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((DynamicBuffer<CharacterSyncInputData> inputBuffer, ref MovableComponent movable, ref Rotation rotation, ref Translation trans, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;
            
            inputBuffer.GetDataAtTick(tick, out CharacterSyncInputData input);
            trans.Value += input.direction * movable.Speed * deltaTime;
            bool3 bool3 = input.direction == float3.zero;
            if (!bool3.x || !bool3.z) rotation.Value = quaternion.LookRotation(input.direction, new float3(0, 1, 0));
        });
    }
}