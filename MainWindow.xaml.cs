using System.Collections.ObjectModel;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Text.Json.Serialization;
using System.Text.Json;
using _8Puzzle.classes;
using System.IO;
using System.Diagnostics;



namespace _8Puzzle
{
    public partial class MainWindow : Window
    {
        public Button[,] buttons = new Button[3, 3];
        public int[,] tiles = new int[3, 3];
        public State state = new State();
        public int[,] goalState = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };

        public MainWindow()
        {
            this.InitializeComponent();
            InitButtons();
            tiles = new int[,] { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 0 } };
            state = new State(tiles);
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    int value = tiles[row, col];
                    buttons[row, col].Content = value == 0 ? "" : value.ToString();
                }
            }
        }

        private void InitButtons()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    var button = new Button
                    {
                        FontSize = 20,
                        Margin = new Thickness(20),
                        Background = Brushes.White,
                        Foreground = Brushes.Black,
                        FontWeight = FontWeights.Bold,

                    };
                    button.Click += Tile_Click;

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);

                    buttons[row, col] = button;
                    GameGrid.Children.Add(button);
                }
            }
        }
        private void Tile_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

            int emptyRow = -1, emptyCol = -1;

            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    if (tiles[r, c] == 0)
                    {
                        emptyRow = r;
                        emptyCol = c;
                        break;
                    }
                }
                if (emptyRow != -1) break;
            }

            if ((Math.Abs(row - emptyRow) == 1 && col == emptyCol) ||
                (Math.Abs(col - emptyCol) == 1 && row == emptyRow))
            {
                tiles[emptyRow, emptyCol] = tiles[row, col];
                tiles[row, col] = 0;

                UpdateButtons();
            }
        }

        private void Shuffle_Click(object sender, RoutedEventArgs e)
        {
            tiles = new State().GetBoard();
            UpdateButtons();
        }

        private async void Solve_Click(object sender, RoutedEventArgs e)
        {

            MessageBox.Show("Solver started");
            string chosenMethod = (Choice.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "A*";



            var initialState = new State(tiles);
            var copy = new State(initialState.GetBoard());
            var heuristic = new ManhattanHeuristic();

            List<Move>? primary, secondary;

            Statistics statsPrimary = new Statistics();
            Statistics statsSecondary = new Statistics();

            await Task.Run(() =>
            {
                switch (chosenMethod)
                {
                    case "A*":
                        primary = new AStar(heuristic, statsPrimary).Solve(initialState);
                        secondary = new RBFS(heuristic, statsSecondary).Solve(initialState);
                        ShowStats(statsPrimary, statsSecondary, '1');
                        break;
                    case "RBFS":
                        primary = new RBFS(heuristic, statsPrimary).Solve(initialState);
                        secondary = new AStar(heuristic, statsSecondary).Solve(initialState);
                        ShowStats(statsPrimary, statsSecondary, '2');
                        break;
                    default:
                        throw new ArgumentException();
                }
            });

            var log = new Logs()
            {
                InitState = Flatten(tiles),
                Alghorithm = chosenMethod,
                VisitedStates = (chosenMethod == "A*") ? statsPrimary.AStarVisitedStates : statsPrimary.RBFSVisitedStates,
                SolutionDepth = (chosenMethod == "A*") ? statsPrimary.AStarSolutionDepth : statsPrimary.RBFSSolutionDepth,
                TimeElapsed = (chosenMethod == "A*") ? statsPrimary.AStarElapsedTime : statsPrimary.RBFSElapsedTime
            };

            Logs.LogSolution(log, "logs.json");


        }

        private void ShowStats(Statistics primary, Statistics secondary, char order)
        {
            switch (order)
            {
                case '1':
                    MessageBox.Show($"A*: {primary.AStarElapsedTime} | {primary.AStarSolutionDepth} | {primary.AStarVisitedStates}"
                        + $"\nRBFS {secondary.RBFSElapsedTime} | {secondary.RBFSSolutionDepth} | {secondary.RBFSVisitedStates}");
                    break;

                case '2':
                    MessageBox.Show($"RBFS {primary.RBFSElapsedTime} | {primary.RBFSSolutionDepth} | {primary.RBFSVisitedStates}"
                        + $"\nA*: {secondary.AStarElapsedTime} | {secondary.AStarSolutionDepth} | {secondary.AStarVisitedStates}");
                    break;
                default:
                    break;
            }
        }

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

        private void ClearLogs_Click(object sender, RoutedEventArgs e)
        {
            FileInfo logInfo = new FileInfo("logs.json");

            if (File.Exists(logInfo.FullName) && logInfo.Length > 0)
            {
                File.Delete(logInfo.FullName);
                MessageBox.Show("logs.json was successfuly deleted");
            }
            else
            {
                MessageBox.Show("There is no log file at the moment, please create one by autopmatically solving the state.");
            }
        }

        private void OpenLogs_Click(object sender, RoutedEventArgs e)
        {
            if (File.Exists("logs.json"))
            {
                string filePath = "C:\\Users\\flistrrr\\Documents\\kr\\8Puzzle\\bin\\Debug\\net6.0-windows\\logs.json";

                Process.Start("notepad.exe", filePath);
            }
            else
            {
                MessageBox.Show("There is no log files. Please solve some states for it to be created");
            }
        }
    }
}