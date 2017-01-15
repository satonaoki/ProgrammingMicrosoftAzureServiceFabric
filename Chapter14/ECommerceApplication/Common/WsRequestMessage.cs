using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gateway
{
    [ProtoContract]
    public class WsRequestMessage
    {
        [ProtoMember(1)]
        public string Operation;

        [ProtoMember(2)]
        public byte[] Value;
    }
}
