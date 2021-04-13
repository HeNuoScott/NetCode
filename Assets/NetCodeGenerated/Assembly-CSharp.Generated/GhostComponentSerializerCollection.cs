//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using Unity.Entities;
using Unity.NetCode;
using Assembly_CSharp.Generated;

namespace Assembly_CSharp.Generated
{
    [UpdateInGroup(typeof(ClientAndServerInitializationSystemGroup))]
    public class GhostComponentSerializerRegistrationSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var ghostCollectionSystem = World.GetOrCreateSystem<GhostCollectionSystem>();
            ghostCollectionSystem.AddSerializer(LookRotationComponentGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(MovableComponentGhostComponentSerializer.State);
            ghostCollectionSystem.AddSerializer(RedCubeComponentGhostComponentSerializer.State);
        }

        protected override void OnUpdate()
        {
            var parentGroup = World.GetExistingSystem<InitializationSystemGroup>();
            if (parentGroup != null)
            {
                parentGroup.RemoveSystemFromUpdateList(this);
            }
        }
    }
}