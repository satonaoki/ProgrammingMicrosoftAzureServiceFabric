using ActorSwarm.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
