using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ReleaseSkillSystem : ComponentSystem
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
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(req, new ReleaseSkillRequest() { skillId = (int)KeyCode.Q });
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent());
        }
        else if (Input.GetKeyUp(KeyCode.E))
        {
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(req, new ReleaseSkillRequest() { skillId = (int)KeyCode.E });
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent());
        }
        else if (Input.GetKeyUp(KeyCode.R))
        {
            var req = PostUpdateCommands.CreateEntity();
            PostUpdateCommands.AddComponent(req, new ReleaseSkillRequest() { skillId = (int)KeyCode.R });
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent());
        }
    }
}
