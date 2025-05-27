using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8Puzzle
{
    public class Move
    {
        public Direction Direction { get; }
        public Move(Direction direction) => Direction = direction;
        public override string ToString() => Direction.ToString();
    }
}
