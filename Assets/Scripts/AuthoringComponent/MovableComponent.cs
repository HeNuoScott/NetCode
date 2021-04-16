using Unity.Mathematics;
using Unity.Entities;
using Unity.NetCode;

[GenerateAuthoringComponent] // 可以移动组件
public struct MovableComponent : IComponentData
{
    [GhostField]
    public float Speed;
    [GhostField]
    public float3 Direction;
}