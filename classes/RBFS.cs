using _8Puzzle;
using _8Puzzle.Interfaces;
using System.Windows;

public class RBFS : IAlghorithm
{
    private IHeuristic heuristic;
    private Statistics stats;

    public RBFS(IHeuristic heuristic, Statistics stats)
    {
        this.heuristic = heuristic;
        this.stats = stats;
    }

    public List<Move>? Solve(State state)
    {
        var start = DateTime.Now;

        var startNode = new Node(state, null, null, 0, heuristic.Calculate(state));

        int visited = 0;
        var visitedStates = new HashSet<State>(); 

        var (goalNode, _, count) = Solution(startNode, int.MaxValue, visited);

        if (goalNode == null)
            return null;

        var path = Reconstruct(goalNode);
        int depth = path.Count;

        stats.CollectRBFSStatistics(count, depth, DateTime.Now - start);
        return path;
    }

    public (Node? node, int fCost, int visited) Solution(Node node, int fLimit, int visited)
    {
        visited++;

        if (node.State.IsGoalState())
            return (node, node.F, visited);

        var successors = node.State.GetPossibleMoves()
            .Select(move =>
            {
                var nextState = node.State.ApplyMove(move);

                if (node.Parent != null && node.Parent.State.Equals(nextState))
                    return null;

                int g = node.G + 1;
                int h = heuristic.Calculate(nextState);
                return new Node(nextState, node, move, g, h);
            })
            .Where(n => n != null)
            .Cast<Node>()
            .ToList();

        if (successors.Count == 0)
            return (null, int.MaxValue, visited);

        var fValues = successors.ToDictionary(s => s, s => Math.Max(s.F, node.F));

        while (true)
        {
            successors.Sort((a, b) => fValues[a].CompareTo(fValues[b]));
            var best = successors[0];

            if (fValues[best] > fLimit)
                return (null, fValues[best], visited);

            int alternative = successors.Count > 1 ? fValues[successors[1]] : int.MaxValue;

            var (result, newF, newVisited) = Solution(best, Math.Min(fLimit, alternative), visited);
            visited = newVisited;
            fValues[best] = newF;

            if (result != null)
                return (result, newF, visited);
        }
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
