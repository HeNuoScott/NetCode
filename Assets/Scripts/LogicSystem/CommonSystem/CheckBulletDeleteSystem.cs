using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;
using Unity.Collections;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class CheckBulletDeleteSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        /*不建议
         * WithStructuralChanges会拖慢线程
         * 直接利用 EntityManager.DestroyEntity(entity) 删除实体
         float deltaTime = Time.DeltaTime;
         Entities.WithAll<BulletTag>()//直接提取所有包含BulletTag标签的entity
                 .WithStructuralChanges()//结构数据化改变的必须添加，例如删除enity
                 .WithNone<DeleteTag>()
                 .WithBurst()
                 .ForEach((Entity entity, in CutTimerComponent cutTimer) =>
                 {
                     if (cutTimer.Time <= 0) commandBuffer.AddComponent(entity, new DeleteTag());
                 }).Run();
       */
        float deltaTime = Time.DeltaTime;
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithAll<BulletTag>()
            .WithNone<DeleteTag>()
            .WithBurst()
            .ForEach((Entity entity, in CutTimerComponent cutTimer) =>
            {
                if (cutTimer.Time <= 0) commandBuffer.AddComponent(entity, new DeleteTag());
            }).Run();

        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        return default;
    }
}
