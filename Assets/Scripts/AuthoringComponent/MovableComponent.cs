﻿using Unity.Mathematics;
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
    [GhostField]
    public float3 Direction;
}