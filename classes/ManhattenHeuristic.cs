using _8Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8Puzzle
{
    public class ManhattanHeuristic : IHeuristic
    {
        private static readonly (int Row, int Col)[] goalPositions = new (int, int)[9]
        {
        (2,2), (0,0), (0,1), (0,2), (1,0), (1,1), (1,2), (2,0), (2,1)
        };

        public int Calculate(State state, State? goalState = null)
        {
            int[,] board = state.GetBoard();
            int dist = 0;
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    int v = board[i, j];
                    if (v != 0)
                    {
                        var (gr, gc) = goalPositions[v];
                        dist += Math.Abs(i - gr) + Math.Abs(j - gc);
                    }
                }
            return dist;
        }
    }


}
