using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
