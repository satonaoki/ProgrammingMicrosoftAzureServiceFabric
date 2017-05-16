using Common;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ActorSwarm
{
    [DataContract]
    [KnownType(typeof(Common.ResidentState))]
    [KnownType(typeof(Common.Resident))]
    public sealed class SwarmState
    {
        [DataMember]
        public Shared2DArray<byte> SharedState { get; set; }
        [DataMember]
        public List<ResidentState> VirtualActorStates { get; set; }
        [DataMember]
        public List<IVirtualActor> VirutalActors { get; set; }
    }
}
