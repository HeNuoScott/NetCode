using Unity.Entities;
using Unity.NetCode;


[GenerateAuthoringComponent]
public struct RedCubeComponent : IComponentData
{
    [GhostField]
    public int ExampleValue;
}