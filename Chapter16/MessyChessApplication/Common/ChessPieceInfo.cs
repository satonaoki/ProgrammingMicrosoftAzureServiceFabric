using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class ChessPieceInfo
    {
        [DataMember]
        public int Team { get; set; }
        [DataMember]
        public ChessPieceType PieceType { get; set; }
        [DataMember]
        public string ActorId { get; set; }
        [DataMember]
        public int X { get; set; }
        [DataMember]
        public int Y { get; set; }
        public static ChessPieceInfo Empty
        {
            get
            {
                return new ChessPieceInfo { Team = 0, ActorId = "", PieceType = ChessPieceType.Undefined, X = 0, Y = 0 };
            }
        }
        public void CopyFrom(ChessPieceInfo other)
        {

            this.Team = other.Team;
            this.PieceType = other.PieceType;
            this.ActorId = other.ActorId;
            this.X = other.X;
            this.Y = other.Y;
        }
    }
}
