using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quarto
{
    class Minimax
    {
        public interface Node
        {
            public Node GenerateNextChild();

            public double Score
            {
                get;
            }

            public bool IsTerminalState
            {
                get;
            }
        }

        //public ReturnNode Minimax(Node current, bool maxTurn)
        //{
        //    double score;
        //    Node selection = null;

        //    var currentScore = current.Score;
        //    var children = current.Children;

        //    if (children == null || children.Count == 0)
        //    {
        //        score = currentScore; // Leaf node
        //    }
        //    else
        //    {
        //        score = double.MinValue;

        //        foreach (var child in children)
        //        {
        //            var hscore = Minimax(child, !maxTurn).Score;

        //            if(((score > hscore) && !maxTurn) || ((score < hscore) && maxTurn)) {
        //                score = hscore;
        //                selection = child;
        //            }

        //            if((score < -1 && !maxTurn) || (score > 1 && maxTurn)) {
        //                break;
        //            }
        //        }
        //    }

        //    return new ReturnNode(selection, score);
        //}

        //class ReturnNode
        //{
        //    public Node Selection;
        //    public double Score;

        //    public ReturnNode(Node selection, double score)
        //    {
        //        Selection = selection;
        //        Score = score;
        //    }
        //}

        public Node Minimax(Node current, int depth)
        {
            if (current.IsTerminalState || depth <= 0)
                return null;
            else
            {
                double score, bestScore = double.NegativeInfinity;
                Node bestChild = null, child = current.GenerateNextChild();
                while (child != null)
                {
                    score = Min(child, depth - 1);
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestChild = child;
                    }
                    child = current.GenerateNextChild();
                }
                return bestChild;
            }
        }

        private double Max(Node current, int depth)
        {
            if (current.IsTerminalState || depth <= 0)
                return current.Score;
            else
            {
                double score = double.NegativeInfinity;
                Node child = current.GenerateNextChild();
                while (child != null)
                {
                    score = Math.Max(score, Min(child, depth - 1));
                    child = current.GenerateNextChild();
                }
                return score;
            }
        }

        private double Min(Node current, int depth)
        {
            if (current.IsTerminalState || depth <= 0)
                return current.Score;
            else
            {
                double score = double.PositiveInfinity;
                Node child = current.GenerateNextChild();
                while (child != null)
                {
                    score = Math.Max(score, Max(child, depth - 1));
                    child = current.GenerateNextChild();
                }
                return score;
            }
        }
    }
    
    class QuartoNode : Minimax.Node
    {
        public static QuartoNode InitialNode;

        static QuartoNode()
        {
            InitialNode = new QuartoNode();
            InitialNode.pieceToPlace = 0;
            InitialNode.unusedPieces = new List<Piece>((IEnumerable<Piece>)Enum.GetValues(typeof(Piece)));
            InitialNode.unusedTiles = new List<byte>(new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 });
            InitialNode.tiles = new Piece[16];
        }
        
        private Piece pieceToPlace;
        private List<Piece> unusedPieces;
        private List<byte> unusedTiles;
        private Piece[] tiles;
        private int i, j;

        private QuartoNode Clone()
        {
            QuartoNode clone = new QuartoNode();
            clone.pieceToPlace = this.pieceToPlace;
            clone.unusedPieces = new List<Piece>(this.unusedPieces);
            clone.unusedTiles = new List<byte>(this.unusedTiles);
            clone.tiles = new Piece[this.tiles.Length];
            this.tiles.CopyTo(clone.tiles, 0);
            return clone;
        }

        public bool Minimax.Node.IsTerminalState
        {
            get
            {
                return unusedTiles.Count == 0 || 
                    ((tiles[0] & tiles[1] & tiles[2] & tiles[3]) |
                    (tiles[4] & tiles[5] & tiles[6] & tiles[7]) |
                    (tiles[8] & tiles[9] & tiles[10] & tiles[11]) |
                    (tiles[12] & tiles[13] & tiles[14] & tiles[15]) |
                    (tiles[0] & tiles[4] & tiles[8] & tiles[12]) |
                    (tiles[1] & tiles[5] & tiles[9] & tiles[13]) |
                    (tiles[2] & tiles[6] & tiles[10] & tiles[14]) |
                    (tiles[3] & tiles[7] & tiles[11] & tiles[15]) |
                    (tiles[0] & tiles[5] & tiles[10] & tiles[15]) |
                    (tiles[3] & tiles[6] & tiles[9] & tiles[12])) != 0;
            }
        }

        public QuartoNode Minimax.Node.GenerateNextChild()
        {
            if (pieceToPlace == 0)
            {
                if (i < unusedPieces.Count)
                {
                    QuartoNode next = this.Clone();
                    next.pieceToPlace = next.unusedPieces[i++];
                    next.unusedPieces.Remove(next.pieceToPlace);
                    return next;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                if (j < unusedTiles.Count)
                {
                    QuartoNode next = this.Clone();
                    byte nextTile = next.unusedTiles[j++];
                    next.tiles[nextTile] = next.pieceToPlace;
                    next.unusedTiles.Remove(nextTile);
                    next.pieceToPlace = next.unusedPieces[i++];
                    next.unusedPieces.Remove(next.pieceToPlace);
                    i = i < unusedPieces.Count ? i + 1 : 0;
                    return next;
                }
                else
                {
                    return null;
                }
            }
        }

        public double Score
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    [FlagsAttribute]
    enum Attribute : byte
    {
        None = 0,
        Big = 1,
        Small = 2,
        Blue = 4,
        Red = 8,
        Hollow = 16,
        Solid = 32,
        Cube = 64,
        Sphere = 128,
    }

    enum Piece : byte
    {
        BigBlueHollowCube = Attribute.Big | Attribute.Blue | Attribute.Hollow | Attribute.Cube,
        BigBlueHollowSphere = Attribute.Big | Attribute.Blue | Attribute.Hollow | Attribute.Sphere,
        BigBlueSolidCube = Attribute.Big | Attribute.Blue | Attribute.Solid | Attribute.Cube,
        BigBlueSolidSphere = Attribute.Big | Attribute.Blue | Attribute.Solid | Attribute.Sphere,
        BigRedHollowCube = Attribute.Big | Attribute.Red | Attribute.Hollow | Attribute.Cube,
        BigRedHollowSphere = Attribute.Big | Attribute.Red | Attribute.Hollow | Attribute.Sphere,
        BigRedSolidCube = Attribute.Big | Attribute.Red | Attribute.Solid | Attribute.Cube,
        BigRedSolidSphere = Attribute.Big | Attribute.Red | Attribute.Solid | Attribute.Sphere,
        SmallBlueHollowCube = Attribute.Small | Attribute.Blue | Attribute.Hollow | Attribute.Cube,
        SmallBlueHollowSphere = Attribute.Small | Attribute.Blue | Attribute.Hollow | Attribute.Sphere,
        SmallBlueSolidCube = Attribute.Small | Attribute.Blue | Attribute.Solid | Attribute.Cube,
        SmallBlueSolidSphere = Attribute.Small | Attribute.Blue | Attribute.Solid | Attribute.Sphere,
        SmallRedHollowCube = Attribute.Small | Attribute.Red | Attribute.Hollow | Attribute.Cube,
        SmallRedHollowSphere = Attribute.Small | Attribute.Red | Attribute.Hollow | Attribute.Sphere,
        SmallRedSolidCube = Attribute.Small | Attribute.Red | Attribute.Solid | Attribute.Cube,
        SmallRedSolidSphere = Attribute.Small | Attribute.Red | Attribute.Solid | Attribute.Sphere,
    }
}
