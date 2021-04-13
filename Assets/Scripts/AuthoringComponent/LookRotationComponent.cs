using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// 看向一个方向的组件
/// </summary>
[GenerateAuthoringComponent]
public struct LookRotationComponent : IComponentData
{
    [GhostField]
    public float3 Direction;
}