using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.NetCode;

[AlwaysSynchronizeSystem]
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class LookRotationSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float deltaTime = Time.DeltaTime;
        Entities.ForEach((ref Rotation rotation, in LookRotationComponent rotatable) => 
        {
            bool3 bool3 = (rotatable.Direction == float3.zero);
            // Ö´ÐÐÐý×ª
            if (!bool3.x || !bool3.z)
            {
                rotation.Value = quaternion.LookRotation(rotatable.Direction, new float3(0, 1, 0));
            }
        }).Schedule();
    }
}