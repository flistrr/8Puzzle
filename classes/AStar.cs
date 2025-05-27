using _8Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace _8Puzzle
{
    public class AStar : IAlghorithm
    {
        private readonly IHeuristic heuristic;
        private readonly Statistics stats;

        public AStar(IHeuristic heuristic, Statistics stats)
        {
            this.heuristic = heuristic;
            this.stats = stats;
        }

        public List<Move>? Solve(State initialState)
        {
            var start = DateTime.Now;

            if (initialState.IsGoalState())
            {
                MessageBox.Show("The state is already solved.");
                return new List<Move>();
            }
            

            int visited = 0;
            var open = new PriorityQueue<Node, int>();
            var all = new Dictionary<State, Node>();

            var startNode = new Node(initialState, null, null, 0, heuristic.Calculate(initialState));
            open.Enqueue(startNode, startNode.F);
            all[initialState] = startNode;

            while (open.Count > 0)
            {
                var cur = open.Dequeue();
                visited++;

                if (cur.State.IsGoalState())
                {
                    var sol = Reconstruct(cur);
                    stats.CollectAStarStatistics(visited, sol.Count, DateTime.Now - start);
                    return sol;
                }

                foreach (var mv in cur.State.GetPossibleMoves())
                {
                    var ns = cur.State.ApplyMove(mv);
                    int g = cur.G + 1;
                    int h = heuristic.Calculate(ns);

                    if (!all.TryGetValue(ns, out var ex) || g < ex.G)
                    {
                        var nn = new Node(ns, cur, mv, g, h);
                        open.Enqueue(nn, nn.F);
                        all[ns] = nn;
                    }
                }
            }

            stats.CollectAStarStatistics(visited, -1, DateTime.Now - start);
            return null;
        }


        public static List<Move> Reconstruct(Node node)
        {
            var path = new List<Move>();
            while (node.Parent != null)
            {
                path.Add(node.Move!);
                node = node.Parent;
            }
            path.Reverse();
            return path;
        }


    }
}
