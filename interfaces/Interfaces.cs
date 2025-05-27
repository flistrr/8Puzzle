namespace _8Puzzle.Interfaces
{
    public interface IAlghorithm
    {
        List<Move>? Solve(State initialState);
    }

    public interface IHeuristic
    {
        int Calculate(State state, State? goalState = null);
    }
}