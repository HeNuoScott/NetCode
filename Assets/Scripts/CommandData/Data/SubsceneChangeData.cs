using Unity.Entities;

/// <summary>
/// 加载/卸载 场景数据
/// </summary>
public struct SubsceneChangeData : IComponentData
{
    public Hash128 sceneGUID;
    public bool isLoadOrUnload;
}