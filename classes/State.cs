using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace _8Puzzle
{
    public enum Direction
    {
        Up, Down, Left, Right
    }
    public class State
    {
        private readonly int[,] tiles;
        private const int rows = 3;
        private const int cols = 3;

        private int[,] goalState = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };

        public State(int[,] tiles)
        {
            this.tiles = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    this.tiles[i, j] = tiles[i, j];

        }

        public void UpdateBoard(State state)
        {
            int[,] ints = state.GetBoard();
            for(int i = 0;i < rows; i++)
            {
                for (int j = 0;j < cols; j++)
                {
                    
                }
            }
        }

        public State()
        {
            Random rng = new Random();
            tiles = GenerateSolvableState(rng);
        }

        public static int[,] GenerateSolvableState(Random rand)
        {
            int[] flatState;

            do
            {
                flatState = Enumerable.Range(0, 9)
                    .OrderBy(_ => rand.Next())
                    .ToArray();
            } while (!IsSolvable(flatState));


            int[,] board = new int[3, 3];
            for (int i = 0; i < 9; i++)
            {
                board[i / 3, i % 3] = flatState[i];
            }

            return board;
        }

        public static bool IsSolvable(int[] puzzle)
        {
            int inversions = 0;

            for (int i = 0; i < puzzle.Length; i++)
            {
                for (int j = i + 1; j < puzzle.Length; j++)
                {
                    if (puzzle[i] != 0 && puzzle[j] != 0 && puzzle[i] > puzzle[j])
                        inversions++;
                }
            }

            return inversions % 2 == 0;
        }

        public int[,] GetBoard()
        {
            int[,] copy = new int[rows, cols];
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    copy[i, j] = tiles[i, j];
            return copy;
        }

        public (int Row, int Col) GetPosition(int value)
        {
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (tiles[i, j] == value)
                        return (i, j);
            throw new ArgumentException("Value not found");
        }

        public bool IsGoalState()
        {
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                    if (this.tiles[i, j] != goalState[i, j])
                        return false;
            return true;
        }

        public List<Move> GetPossibleMoves()
        {
            var moves = new List<Move>();
            var (r, c) = GetPosition(0);
            if (r > 0) moves.Add(new Move(Direction.Up));
            if (r < rows - 1) moves.Add(new Move(Direction.Down));
            if (c > 0) moves.Add(new Move(Direction.Left));
            if (c < cols - 1) moves.Add(new Move(Direction.Right));
            return moves;
        }

        public State ApplyMove(Move move)
        {
            int[,] newTiles = GetBoard();
            var (r, c) = GetPosition(0);
            int nr = r, nc = c;
            switch (move.Direction)
            {
                case Direction.Up: nr = r - 1; break;
                case Direction.Down: nr = r + 1; break;
                case Direction.Left: nc = c - 1; break;
                case Direction.Right: nc = c + 1; break;
            }
            newTiles[r, c] = newTiles[nr, nc];
            newTiles[nr, nc] = 0;
            return new State(newTiles);
        }

        public override bool Equals(object? obj)
        {
            if (obj is not State other) return false;
            for (int i = 0; i < rows; i++)
                for (int j = 0; j < cols; j++)
                    if (tiles[i, j] != other.tiles[i, j])
                        return false;
            return true;
        }

        public override int GetHashCode()
        {
            int hash = 17;
            foreach (int t in tiles) hash = hash * 31 + t;
            return hash;
        }
    }
}
