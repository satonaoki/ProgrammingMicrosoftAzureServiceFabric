using System.Runtime.Serialization;

namespace Termite.Interfaces
{
    [DataContract]
    public sealed class TermiteState
    {
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        [DataMember]
        public bool HasWoodchip { get; set; }
    }
}
