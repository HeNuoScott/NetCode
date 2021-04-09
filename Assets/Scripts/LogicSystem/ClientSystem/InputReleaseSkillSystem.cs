using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class InputReleaseSkillSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("InputSkills");
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(req, new ReleaseSkillRequest() { skillId = (int)KeyCode.Space });
            //如果TargetConnection设置为Entity.Null，则将广播该消息。在客户端上，您不必设置此值，因为您只会将其发送到服务器。
            //PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent() {TargetConnection = Entity });
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent());
        }
    }
}
