using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class CharacterMoveInputSystem : ComponentSystem
{
    protected override void OnCreate()
    {
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
        float2 curInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        input.direction = new float3(curInput.x, 0, curInput.y);
        var inputBuffer = EntityManager.GetBuffer<CharacterSyncData>(localInputDataEntity);
        inputBuffer.AddCommandData(input);
    }
}
