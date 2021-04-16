using Unity.NetCode;

/// <summary>
/// 释放技能 RPC
/// </summary>
public struct ReleaseSkillRequest : IRpcCommand 
{
    public int skillId;
}