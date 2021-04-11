using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// 可以移动组件
/// </summary>
[GenerateAuthoringComponent]
public struct MovableComponent : IComponentData
{
    [GhostField]
    public float Speed;
}