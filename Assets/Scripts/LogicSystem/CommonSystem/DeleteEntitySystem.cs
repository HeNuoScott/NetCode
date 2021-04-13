using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class DeleteEntitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithAll<DeleteTag>()
            .WithoutBurst()
            .ForEach((Entity entity) =>
            {
                commandBuffer.DestroyEntity(entity);
            }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;
    }
}