using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class CharacterInputSystem : ComponentSystem
{
    protected override void OnCreate()
    {
        // 设置单例在Updata中可直接获得(系统要是用单例,在系统初始化时添加此设置)
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        var localInputDataEntity = GetSingleton<CommandTargetComponent>().targetEntity;
        if (localInputDataEntity == Entity.Null)
        {
            var localPlayerId = GetSingleton<NetworkIdComponent>().Value;
            Entities.WithAll<MovableComponent>().WithNone<CharacterSyncData>().ForEach((Entity ent, ref GhostOwnerComponent ghostOwner) =>
            {
                if (ghostOwner.NetworkId == localPlayerId)
                {
                    PostUpdateCommands.AddBuffer<CharacterSyncData>(ent);
                    PostUpdateCommands.SetComponent(GetSingletonEntity<CommandTargetComponent>(), new CommandTargetComponent { targetEntity = ent });
                    CameraFollow.instance.CharacterEntity = ent;
                }
            });
            return;
        }

        var input = default(CharacterSyncData);
        input.Tick = World.GetExistingSystem<ClientSimulationSystemGroup>().ServerTick;
        input.Horizontal = Input.GetAxis("Horizontal");
        input.Vertical = Input.GetAxis("Vertical");
        var inputBuffer = EntityManager.GetBuffer<CharacterSyncData>(localInputDataEntity);
        inputBuffer.AddCommandData(input);
    }
}
