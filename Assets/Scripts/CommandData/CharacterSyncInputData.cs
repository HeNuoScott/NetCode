using Unity.NetCode;
using Unity.Mathematics;

public struct CharacterSyncInputData : ICommandData
{
    public uint Tick { get; set; }
    public float3 direction;
}