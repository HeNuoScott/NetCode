using Unity.NetCode;

public struct ReleaseSkillRequest : IRpcCommand 
{
    public int skillId;
}