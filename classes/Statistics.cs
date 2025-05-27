using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _8Puzzle
{
    public class Statistics
    {
        public int AStarVisitedStates { get; private set; }
        public int AStarSolutionDepth { get; private set; }
        public TimeSpan AStarElapsedTime { get; private set; }

        public int RBFSVisitedStates { get; private set; }
        public int RBFSSolutionDepth { get; private set; }
        public TimeSpan RBFSElapsedTime { get; private set; }

        public void CollectAStarStatistics(int visited, int depth, TimeSpan duration)
        {
            AStarVisitedStates = visited;
            AStarSolutionDepth = depth;
            AStarElapsedTime = duration;
        }
        public void CollectRBFSStatistics(int visited, int depth, TimeSpan duration)
        {
            RBFSVisitedStates = visited;
            RBFSSolutionDepth = depth;
            RBFSElapsedTime = duration;
        }
    }
}
