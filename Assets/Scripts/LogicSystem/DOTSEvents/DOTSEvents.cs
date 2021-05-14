using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
/*
 * 这个事件系统 主要实在 JobSystem子线程中运行 抓取事件 被交由主线程调用
 */

public class DOTSEvents_NextFrame<T> where T : struct, IComponentData
{
    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;
    private EntityManager entityManager;
    private EntityArchetype eventEntityArchetype;
    private EntityQuery eventEntityQuery;
    private Action<T> OnEventAction;

    private EventTrigger eventCaller;
    private EntityCommandBuffer entityCommandBuffer;

    public DOTSEvents_NextFrame(World world, Action<T> OnEventAction = null)
    {
        this.OnEventAction = OnEventAction;
        endSimulationEntityCommandBufferSystem = world.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        entityManager = world.EntityManager;

        eventEntityArchetype = entityManager.CreateArchetype(typeof(T));
        eventEntityQuery = entityManager.CreateEntityQuery(typeof(T));
    }

    public EventTrigger GetEventTrigger()
    {
        entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        eventCaller = new EventTrigger(eventEntityArchetype, entityCommandBuffer);
        return eventCaller;
    }

    public void CaptureEvents(JobHandle jobHandleWhereEventsWereScheduled, Action<T> OnEventAction = null)
    {
        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandleWhereEventsWereScheduled);
        eventCaller.Playback(endSimulationEntityCommandBufferSystem.CreateCommandBuffer(), eventEntityQuery, OnEventAction == null ? this.OnEventAction : OnEventAction);
    }



    public struct EventTrigger
    {
        [Obsolete]
        private struct EventJob : IJobForEachWithEntity<T>
        {
            public EntityCommandBuffer.ParallelWriter parallelWriter;
            public NativeList<T> nativeList;

            public void Execute(Entity entity, int index, ref T c0)
            {
                nativeList.Add(c0);
                parallelWriter.DestroyEntity(index, entity);
            }
        }

        private EntityCommandBuffer.ParallelWriter entityCommandBufferParallelWriter;
        private EntityArchetype entityArchetype;

        public EventTrigger(EntityArchetype entityArchetype, EntityCommandBuffer entityCommandBuffer)
        {
            this.entityArchetype = entityArchetype;
            entityCommandBufferParallelWriter = entityCommandBuffer.AsParallelWriter();
        }

        public void TriggerEvent(int entityInQueryIndex)
        {
            entityCommandBufferParallelWriter.CreateEntity(entityInQueryIndex, entityArchetype);
        }

        public void TriggerEvent(int entityInQueryIndex, T t)
        {
            Entity entity = entityCommandBufferParallelWriter.CreateEntity(entityInQueryIndex, entityArchetype);
            entityCommandBufferParallelWriter.SetComponent(entityInQueryIndex, entity, t);
        }


        public void Playback(EntityCommandBuffer destroyEntityCommandBuffer, EntityQuery eventEntityQuery, Action<T> OnEventAction)
        {
            if (eventEntityQuery.CalculateEntityCount() > 0)
            {
                NativeList<T> nativeList = new NativeList<T>(Allocator.TempJob);
                EventJob eventJob = new EventJob()
                {
                    parallelWriter = destroyEntityCommandBuffer.AsParallelWriter(),
                    nativeList = nativeList,
                };
                eventJob.Run(eventEntityQuery);

                foreach (T t in nativeList) OnEventAction(t);
                nativeList.Dispose();
            }
        }
    }

}

public class DOTSEvents_SameFrame<T> where T : struct, IComponentData
{
    private EntityManager entityManager;
    private EntityArchetype eventEntityArchetype;
    private EntityQuery eventEntityQuery;
    private Action<T> OnEventAction;

    private EventTrigger eventCaller;
    private EntityCommandBuffer entityCommandBuffer;

    public DOTSEvents_SameFrame(World world, Action<T> OnEventAction = null)
    {
        this.OnEventAction = OnEventAction;
        entityManager = world.EntityManager;

        eventEntityArchetype = entityManager.CreateArchetype(typeof(T));
        eventEntityQuery = entityManager.CreateEntityQuery(typeof(T));
    }

    public EventTrigger GetEventTrigger()
    {
        eventCaller = new EventTrigger(eventEntityArchetype, out entityCommandBuffer);
        return eventCaller;
    }

    public void CaptureEvents(JobHandle jobHandleWhereEventsWereScheduled, Action<T> OnEventAction = null)
    {
        eventCaller.Playback(jobHandleWhereEventsWereScheduled, entityCommandBuffer, entityManager, eventEntityQuery, OnEventAction == null ? this.OnEventAction : OnEventAction);
    }

    public struct EventTrigger
    {
        private EntityCommandBuffer.ParallelWriter parallelWriter;
        private EntityArchetype entityArchetype;

        public EventTrigger(EntityArchetype entityArchetype, out EntityCommandBuffer entityCommandBuffer)
        {
            this.entityArchetype = entityArchetype;
            entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
            parallelWriter = entityCommandBuffer.AsParallelWriter();
        }

        public void TriggerEvent(int entityInQueryIndex)
        {
            parallelWriter.CreateEntity(entityInQueryIndex, entityArchetype);
        }

        public void TriggerEvent(int entityInQueryIndex, T t)
        {
            Entity entity = parallelWriter.CreateEntity(entityInQueryIndex, entityArchetype);
            parallelWriter.SetComponent(entityInQueryIndex, entity, t);
        }

        public void Playback(JobHandle jobHandleWhereEventsWereScheduled, EntityCommandBuffer entityCommandBuffer, EntityManager EntityManager, EntityQuery eventEntityQuery, Action<T> OnEventAction)
        {
            jobHandleWhereEventsWereScheduled.Complete();
            entityCommandBuffer.Playback(EntityManager);
            entityCommandBuffer.Dispose();

            int entityCount = eventEntityQuery.CalculateEntityCount();
            if (entityCount > 0)
            {
                NativeArray<T> nativeArray = eventEntityQuery.ToComponentDataArray<T>(Allocator.TempJob);
                foreach (T t in nativeArray)
                {
                    OnEventAction(t);
                }
                nativeArray.Dispose();
            }
            EntityManager.DestroyEntity(eventEntityQuery);
        }
    }
}
