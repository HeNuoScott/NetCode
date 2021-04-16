using Unity.Entities;

[GenerateAuthoringComponent] // 计时 数据
public struct CutTimerComponent : IComponentData
{
    public float Time;
}
