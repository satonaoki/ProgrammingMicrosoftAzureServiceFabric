using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ActorSwarm.Common
{
[DataContract]
public class Resident: IVirtualActor
{
    ResidentState State;
    Shared2DArray<byte> SharedState;
    int mRange;
    int mId;
    private static Random mRand = new Random();
    private List<Tuple<int, int>> mOffsets = new List<Tuple<int, int>>
    {
            new Tuple<int, int>(-1,0),
            new Tuple<int, int>(-1,-1),
            new Tuple<int, int>(0,-1),
            new Tuple<int, int>(1,-1),
            new Tuple<int, int>(1,0),
            new Tuple<int, int>(1,1),
            new Tuple<int, int>(0,1),
            new Tuple<int, int>(-1,1)
    };
    public Resident(int range, int id, ResidentState state, Shared2DArray<byte> sharedState)
    {
        mRange = range;
        mId = id;
        State = state;
        SharedState = sharedState;
    }
    public Task ApproveProposalAsync(IProposal proposal)
    {
        if (proposal is Proposal2D<byte>)
        {
            var p = (Proposal2D<byte>)proposal;
            this.State.X = p.NewX;
            this.State.Y = p.NewY;
        }
        return Task.FromResult(1);
    }
    public Task<IProposal> ProposeAsync()
    {
        int count = 0;
        count += countNeighbour(this.State.X - 1, this.State.Y);
        count += countNeighbour(this.State.X - 1, this.State.Y - 1);
        count += countNeighbour(this.State.X, this.State.Y - 1);
        count += countNeighbour(this.State.X + 1, this.State.Y - 1);
        count += countNeighbour(this.State.X + 1, this.State.Y);
        count += countNeighbour(this.State.X + 1, this.State.Y + 1);
        count += countNeighbour(this.State.X, this.State.Y + 1);
        count += countNeighbour(this.State.X - 1, this.State.Y + 1);
        if (count <= 3)
        {
            var randList = mOffsets.OrderBy(p => mRand.Next());
            foreach (var item in randList)
            {
                if (findEmptyNeighbour(this.State.X + item.Item1, this.State.Y + item.Item2))
                {

                    return Task.FromResult<IProposal>(new Proposal2D<byte>(mId, 
                        this.State.X,
                        this.State.Y,
                        this.State.X + item.Item1,
                        this.State.Y + item.Item2,
                        this.State.Tag));
                }
            }
        }
        return Task.FromResult<IProposal>(null);
    }
    private int countNeighbour(int x, int y)
    {
            if (x >= 0 && x < this.mRange && y >= 0 && y < this.mRange)
                return SharedState[x, y] == this.State.Tag ? 1 : 0;
            else
                return 0;
    }
    private bool findEmptyNeighbour(int x, int y)
    {
        if (x >= 0 && x < this.mRange && y >= 0 && y < this.mRange)
            return SharedState[x, y] == 0;
        else
            return false;
    }
}
}
