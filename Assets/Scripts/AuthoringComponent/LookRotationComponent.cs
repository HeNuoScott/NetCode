using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent] // 看向一个方向的组件
public struct LookRotationComponent : IComponentData
{
    [GhostField]
    public float3 Direction;
}