using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;

/// <summary>
/// ����һ����������
/// </summary>
[GenerateAuthoringComponent]
public struct LookRotationComponent : IComponentData
{
    [GhostField]
    public float3 Direction;
}