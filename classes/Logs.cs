using _8Puzzle.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace _8Puzzle.classes
{
    class Logs
    {
        public int[] InitState { get; set; }
        public int VisitedStates { get; set; }
        public int SolutionDepth { get; set; }
        public TimeSpan TimeElapsed { get; set; }
        public string Alghorithm { get; set; }


        public static int[] Flatten(int[,] state)
        {
            int rows = state.GetLength(0);
            int cols = state.GetLength(1);
            int[] flat = new int[rows * cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    flat[i * cols + j] = state[i, j];
                }
            }
            return flat;
        }


        public static void LogSolution(Logs log, string filePath)
        {
            List<Logs> logs = new List<Logs>();
            if (File.Exists(filePath))
            {
                string existingJson = File.ReadAllText(filePath);
                if (!string.IsNullOrEmpty(existingJson))
                {
                    logs = JsonSerializer.Deserialize<List<Logs>>(existingJson);
                }
            }

            logs.Add(log);

            var options = new JsonSerializerOptions { WriteIndented = false };
            string json = JsonSerializer.Serialize(logs, options);
            File.WriteAllText(filePath, json);

            if(logs == null)
            {
                Console.WriteLine("No log was written.");
            }

        }

    }
}
