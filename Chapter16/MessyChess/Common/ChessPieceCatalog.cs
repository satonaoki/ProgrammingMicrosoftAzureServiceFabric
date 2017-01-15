using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common
{

[DataContract]
public class ChessPieceCatalog
{
    private static Random mRand = new Random();
    [DataMember]
    public List<Tuple<bool, ChessPieceType>> Pieces { get; set; }

    public ChessPieceCatalog()
    {
        Pieces = new List<Tuple<bool, ChessPieceType>>();
    }
    public  ChessPieceType GetAPiece()
    {
        List<int> candidates = new List<int>();
        for (int i = 0; i < Pieces.Count; i++)
            if (Pieces[i].Item1 == false)
                candidates.Add(i);
        if (candidates.Count == 0)
            return ChessPieceType.Undefined;
        else
        {
            var index = mRand.Next(0, candidates.Count);
            Pieces[candidates[index]] = new Tuple<bool, ChessPieceType>(true, Pieces[candidates[index]].Item2);
            return Pieces[candidates[index]].Item2;
        }
    }

    public void ReturnAPiece(ChessPieceType piece)
    {
        for (int i = 0; i < Pieces.Count; i++)
        {
            if (Pieces[i].Item2 == piece && Pieces[i].Item1 == true)
            {
                Pieces[i] = new Tuple<bool, ChessPieceType>(false, Pieces[i].Item2);
                return;
            }
        }
        throw new ArgumentException(string.Format("Piece '{0}' can't be returned.", piece));
            
    }
    public static ChessPieceCatalog Initialize()
    {
        ChessPieceCatalog catalog = new ChessPieceCatalog();

        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.King));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Queen));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Bishop));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Bishop));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Knight));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Knight));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Rook));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Rook));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));
        catalog.Pieces.Add(new Tuple<bool, ChessPieceType>(false, ChessPieceType.Pawn));

        return catalog;
    }
}
}
