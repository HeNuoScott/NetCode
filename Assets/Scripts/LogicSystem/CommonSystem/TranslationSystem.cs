using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;

[AlwaysSynchronizeSystem]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class TranslationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Translation translation, in MovableComponent movable) =>
        {
            // ÷¥––Œª÷√“∆∂Ø
            translation.Value += movable.Direction * movable.Speed * deltaTime;
        }).Schedule();
    }
}
