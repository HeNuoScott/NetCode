using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// 可以移动组件
/// </summary>
[GenerateAuthoringComponent]
public struct MovableCubeComponent : IComponentData
{
    [GhostField]
    public int ExampleValue;
}
