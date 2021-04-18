using Unity.NetCode;

/// <summary>
/// 玩家同步数据 ICommandData交互数据
/// </summary>
public struct CharacterSyncData : ICommandData
{
    /// <summary> 预测标记,用于表明此帧操作是预测 还是实际操作 </summary>
    public uint Tick { get; set; }
    public float Horizontal;
    public float Vertical;
}