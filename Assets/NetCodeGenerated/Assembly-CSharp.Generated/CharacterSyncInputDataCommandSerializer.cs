//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using AOT;
using Unity.Burst;
using Unity.Networking.Transport;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;


namespace Assembly_CSharp.Generated
{
    public struct CharacterSyncInputDataSerializer : ICommandDataSerializer<CharacterSyncInputData>
    {
        public void Serialize(ref DataStreamWriter writer, in CharacterSyncInputData data)
        {
            writer.WriteFloat(data.direction.x);
            writer.WriteFloat(data.direction.y);
            writer.WriteFloat(data.direction.z);
        }

        public void Deserialize(ref DataStreamReader reader, ref CharacterSyncInputData data)
        {
            data.direction.x = reader.ReadFloat();
            data.direction.y = reader.ReadFloat();
            data.direction.z = reader.ReadFloat();
        }

        public void Serialize(ref DataStreamWriter writer, in CharacterSyncInputData data, in CharacterSyncInputData baseline, NetworkCompressionModel compressionModel)
        {
            writer.WritePackedFloatDelta(data.direction.x, baseline.direction.x, compressionModel);
            writer.WritePackedFloatDelta(data.direction.y, baseline.direction.y, compressionModel);
            writer.WritePackedFloatDelta(data.direction.z, baseline.direction.z, compressionModel);
        }

        public void Deserialize(ref DataStreamReader reader, ref CharacterSyncInputData data, in CharacterSyncInputData baseline, NetworkCompressionModel compressionModel)
        {
            data.direction.x = reader.ReadPackedFloatDelta(baseline.direction.x, compressionModel);
            data.direction.y = reader.ReadPackedFloatDelta(baseline.direction.y, compressionModel);
            data.direction.z = reader.ReadPackedFloatDelta(baseline.direction.z, compressionModel);
        }
    }
    public class CharacterSyncInputDataSendCommandSystem : CommandSendSystem<CharacterSyncInputDataSerializer, CharacterSyncInputData>
    {
        [BurstCompile]
        struct SendJob : IJobEntityBatch
        {
            public SendJobData data;
            public void Execute(ArchetypeChunk chunk, int orderIndex)
            {
                data.Execute(chunk, orderIndex);
            }
        }
        protected override void OnUpdate()
        {
            var sendJob = new SendJob{data = InitJobData()};
            ScheduleJobData(sendJob);
        }
    }
    public class CharacterSyncInputDataReceiveCommandSystem : CommandReceiveSystem<CharacterSyncInputDataSerializer, CharacterSyncInputData>
    {
        [BurstCompile]
        struct ReceiveJob : IJobEntityBatch
        {
            public ReceiveJobData data;
            public void Execute(ArchetypeChunk chunk, int orderIndex)
            {
                data.Execute(chunk, orderIndex);
            }
        }
        protected override void OnUpdate()
        {
            var recvJob = new ReceiveJob{data = InitJobData()};
            ScheduleJobData(recvJob);
        }
    }
}
