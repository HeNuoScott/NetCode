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
            //���TargetConnection����ΪEntity.Null���򽫹㲥����Ϣ���ڿͻ����ϣ����������ô�ֵ����Ϊ��ֻ�Ὣ�䷢�͵���������
            //PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent() {TargetConnection = Entity });
            PostUpdateCommands.AddComponent(req, new SendRpcCommandRequestComponent());
        }
    }
}
