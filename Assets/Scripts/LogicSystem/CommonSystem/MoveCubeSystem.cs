using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.NetCode;
using Unity.Transforms;
using UnityEngine;

/// <summary>
/// cube移动系统
/// </summary>
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MoveCubeSystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        var group = World.GetExistingSystem<GhostPredictionSystemGroup>();
        var tick = group.PredictingTick;
        var deltaTime = Time.DeltaTime;
        Entities.ForEach((DynamicBuffer<CubeInput> inputBuffer, ref Translation trans, ref PredictedGhostComponent prediction) =>
        {
            if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction)) return;
            inputBuffer.GetDataAtTick(tick, out CubeInput input);
            if (input.horizontal > 0) trans.Value.x += deltaTime;
            if (input.horizontal < 0) trans.Value.x -= deltaTime;
            if (input.vertical > 0) trans.Value.z += deltaTime;
            if (input.vertical < 0) trans.Value.z -= deltaTime;
        });
    }
}
