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
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((DynamicBuffer<CharacterSyncData> inputBuffer, ref MovableComponent movable, ref Rotation rotation, ref Translation trans, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;
            inputBuffer.GetDataAtTick(tick, out CharacterSyncData input);

            //这里只需要将数据进行更改 

            //位移 旋转 应在相应的系统中进行操作
            trans.Value += input.direction * movable.Speed * deltaTime;
            bool3 bool3 = input.direction == float3.zero;
            if (!bool3.x || !bool3.z) rotation.Value = quaternion.LookRotation(input.direction, new float3(0, 1, 0));
        });
    }
}