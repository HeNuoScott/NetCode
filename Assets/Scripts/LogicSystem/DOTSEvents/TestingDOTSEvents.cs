using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities;
using UnityEngine;
using Unity.Jobs;
using System;

public class TestingDOTSEvents : MonoBehaviour
{
    private void Start()
    {
        World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<PipeMoveSystem_Done>().OnPipePassed += TestingDOTSEvents_OnPipePassed;
    }

    private void TestingDOTSEvents_OnPipePassed(object sender, System.EventArgs e)
    {
        Debug.Log("Pipe Event!");
    }

}

public struct TestData : IComponentData
{
    public bool isTrigger;
}
public class PipeMoveSystem : JobComponentSystem
{
    public event EventHandler OnPipePassed;

    public struct EventComponent : IComponentData
    {
        public double ElapsedTime;
    }

    private EndSimulationEntityCommandBufferSystem endSimulationEntityCommandBufferSystem;

    protected override void OnCreate()
    {
        endSimulationEntityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        double elapsedTime = Time.ElapsedTime;
        float3 moveDir = new float3(-1f, 0f, 0f);
        float moveSpeed = 4f;

        EntityCommandBuffer entityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();
        EntityCommandBuffer.ParallelWriter entityCommandBufferParallelWriter = entityCommandBuffer.AsParallelWriter();
        EntityArchetype eventEntityArchetype = EntityManager.CreateArchetype(typeof(EventComponent));

        double ElapsedTime = Time.ElapsedTime;

        JobHandle jobHandle = Entities.ForEach((int entityInQueryIndex, ref Translation translation, ref TestData pipe) =>
        {
            float xBefore = translation.Value.x;
            translation.Value += moveDir * moveSpeed * deltaTime;
            float xAfter = translation.Value.x;

            if (pipe.isTrigger && xBefore > 0 && xAfter <= 0)
            {
                // Passed the Player
                Entity eventEntity = entityCommandBufferParallelWriter.CreateEntity(entityInQueryIndex, eventEntityArchetype);
                entityCommandBufferParallelWriter.SetComponent(entityInQueryIndex, eventEntity, new EventComponent
                {
                    ElapsedTime = ElapsedTime
                });
            }
        }).Schedule(inputDeps);

        endSimulationEntityCommandBufferSystem.AddJobHandleForProducer(jobHandle);

        EntityCommandBuffer captureEventsEntityCommandBuffer = endSimulationEntityCommandBufferSystem.CreateCommandBuffer();

        Entities.WithoutBurst().ForEach((Entity entity, ref EventComponent eventComponent) =>
        {
            Debug.Log(eventComponent.ElapsedTime + " ### " + ElapsedTime);
            OnPipePassed?.Invoke(this, EventArgs.Empty);
            captureEventsEntityCommandBuffer.DestroyEntity(entity);
        }).Run();

        return jobHandle;
    }

}
public class PipeMoveSystem_Done : JobComponentSystem
{
    public event EventHandler OnPipePassed;
    private DOTSEvents_NextFrame<TriggerEvent> dotsEvents;
    public struct TriggerEvent : IComponentData
    {
        public double triggerTime;
    }

    protected override void OnCreate()
    {
        dotsEvents = new DOTSEvents_NextFrame<TriggerEvent>(World);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        double elapsedTime = Time.ElapsedTime;
        float3 moveDir = new float3(-1f, 0f, 0f);
        float moveSpeed = 4f;

        DOTSEvents_NextFrame<TriggerEvent>.EventTrigger eventTrigger = dotsEvents.GetEventTrigger();

        JobHandle jobHandle = Entities.ForEach((int entityInQueryIndex, ref Translation translation, ref TestData test) =>
        {
            float xBefore = translation.Value.x;
            translation.Value += moveDir * moveSpeed * deltaTime;
            float xAfter = translation.Value.x;

            if (test.isTrigger && xBefore > 0 && xAfter <= 0)
            {
                // Passed the Player
                eventTrigger.TriggerEvent(entityInQueryIndex, new TriggerEvent { triggerTime = elapsedTime });
            }
        }).Schedule(inputDeps);

        dotsEvents.CaptureEvents(jobHandle, (TriggerEvent basicEvent) =>
        {
            Debug.Log(basicEvent.triggerTime + " ###### " + elapsedTime);
            OnPipePassed?.Invoke(this, EventArgs.Empty);
        });
        return jobHandle;
    }
}