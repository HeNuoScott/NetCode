using Unity.Entities;
using Unity.NetCode;
using Unity.Jobs;
using Unity.Collections;

[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class CheckBulletDeleteSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDependencies)
    {
        /*������
         * WithStructuralChanges�������߳�
         * ֱ������ EntityManager.DestroyEntity(entity) ɾ��ʵ��
         float deltaTime = Time.DeltaTime;
         Entities.WithAll<BulletTag>()//ֱ����ȡ���а���BulletTag��ǩ��entity
                 .WithStructuralChanges()//�ṹ���ݻ��ı�ı�����ӣ�����ɾ��enity
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
