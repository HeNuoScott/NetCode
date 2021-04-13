using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;

[AlwaysSynchronizeSystem]
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class CutTimerSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        float deltaTime = Time.DeltaTime;
        Entities.WithAll<CutTimerComponent>().ForEach((ref CutTimerComponent cutTimer) =>
        {
            cutTimer.Time -= deltaTime;
        }).Run();

        return default;
    }
}
