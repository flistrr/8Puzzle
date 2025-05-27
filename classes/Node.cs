using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8Puzzle
{
    public class Node : IComparable<Node>
    {
        public State State { get; }
        public Node? Parent { get; }
        public Move? Move { get; }
        public int G { get; }
        public int H { get; }
        public int F => G + H;
        public Node(State s, Node? p, Move? m, int g, int h) { State = s; Parent = p; Move = m; G = g; H = h; }
        public int CompareTo(Node? other) => other == null ? 1 : F.CompareTo(other.F);
    }
}
