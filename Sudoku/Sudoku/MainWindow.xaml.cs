using System.Net;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Newtonsoft.Json;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    public static class Inner {

        public static bool api;
        public static string difficulty;
    }

    public partial class MainWindow : Window
    {
        public int Mistakes = 0;
        public int SelectedNum = 0;
        public JToken solution;
        public int[] CountNums = new int[9];
        //public List<List<int>> TheGrid = new List<List<int>>();
        public int[] Numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        int[,] grid = new int[9,9];
        public int[,] solvedGrid = new int[9, 9];
        public MainWindow()
        {
            InitializeComponent();
            if (Inner.api == false)
            {
                Time.Text = Inner.difficulty;
                grid = GeneratePuzzle();

                int[,] newGrid = grid.Clone() as int[,];

                SudokuSolver solve = new SudokuSolver(newGrid);
                if (solve.SolveSudoku())
                    solvedGrid = solve.board;
            }
            else
            {

                using (WebClient wc = new WebClient())
                {


                    var json = wc.DownloadString("https://sudoku-api.vercel.app/api/dosuku");
                    JObject tableJson = JObject.Parse(json);
                    var table = tableJson.GetValue("newboard")["grids"][0];
                    var vals = table["value"];
                    solution = table["solution"];
                    Time.Text = table["difficulty"].ToString();

                    for (int i = 0; i < 9; i++)
                    {
                        for (int f = 0; f < 9; f++)
                        {
                            grid[i, f] = Convert.ToInt32(vals[i][f]);
                            solvedGrid[i, f] = Convert.ToInt32(solution[i][f]);
                        }
                    }
                }
            }




            for (int i = 0; i < 9; i++)
            {
                NButtons.ColumnDefinitions.Add(new());
                Button b = new();
                b.Content = (i + 1).ToString();
                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.VerticalAlignment = VerticalAlignment.Stretch;
                b.Background = new SolidColorBrush(Colors.White);
                b.Margin = new Thickness(0, 10, 0, 10);
                b.FontSize = 20;
                b.Click += SetNum;
                Grid.SetColumn(b, i);
                NButtons.Children.Add(b);
            }
            CreateGrid();
        }


        private const int GridSize = 9;

        public int[,] GeneratePuzzle()
        {
            int[,] grid = new int[GridSize, GridSize];

            if (FillGrid(grid, 0, 0))
            {
                float diff = 0;
                switch(Inner.difficulty)
                {
                    case "Easy":
                        diff = 3;
                        break;
                    case "Medium":
                        diff = 1.8f;
                        break;
                    case "Hard":
                        diff = 1.3f;
                        break;
                }


                RemoveCells(grid, Convert.ToInt32(GridSize * GridSize / diff));
                return grid;
            }
            return GeneratePuzzle();
        }


        private bool FillGrid(int[,] grid, int row, int col)
        {

            if (col == GridSize)
            {
                col = 0;
                row++;
            }

            if (row == GridSize)
            {
                return true;
            }

            if (grid[row, col] != 0)
            {
                return FillGrid(grid, row, col + 1);
            }
            Numbers.Shuffle<int>();
            for (int num = 1; num <= GridSize; num++)
            {
                if (row == 0)
                {
              
                    num = Numbers[num - 1];
                }

                if (IsValidPlacement(grid, row, col, num))
                {
                    grid[row, col] = num;


                    if (FillGrid(grid, row, col + 1))
                    {
                        return true;
                    }
                    grid[row, col] = 0;
                }
            }

            return false;
        }

        private bool IsValidPlacement(int[,] grid, int row, int col, int num)
        {

            for (int i = 0; i < GridSize; i++)
            {
                if (grid[row, i] == num || grid[i, col] == num)
                {
                    return false;
                }
            }

            int boxStartRow = row - row % 3;
            int boxStartCol = col - col % 3;

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (grid[boxStartRow + i, boxStartCol + j] == num)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void RemoveCells(int[,] grid, int numCells)
        {
            Random random = new Random();

            while (numCells > 0)
            {
                int row = random.Next(GridSize);
                int col = random.Next(GridSize);

                if (grid[row, col] != 0)
                {
                    grid[row, col] = 0;
                    numCells--;
                }
            }
        }

        private void SetNum(object sender, RoutedEventArgs s)
        {
            Button self = sender as Button;
            HighLight(self.Content.ToString());
        }

        private void PlaceNum(object sender, RoutedEventArgs s)
        {

            Button b = sender as Button;
            int row = Grid.GetRow(b);
            int col = Grid.GetColumn(b);
            if (SelectedNum != 0)
            {

                //int sol = Convert.ToInt32(solution[row][col]);
                int sol = solvedGrid[row,col];

                if (sol == SelectedNum)
                {

                    b.Content = sol;
                    HighLightWrong(row, col, sol.ToString());
                    b.Click -= PlaceNum;
                    b.Click += Find;
                    b.Foreground = new SolidColorBrush(Colors.Blue);
                    b.Background = new SolidColorBrush(Colors.LightBlue);
                    CountNums[sol - 1]++;
                    Count.Text = $"9/{CountNums[sol - 1]}";
                    Num(sol.ToString());
                }
                else
                {

                    b.Content = SelectedNum;
                    b.Foreground = new SolidColorBrush(Colors.Red);
                    b.Background = new SolidColorBrush(Colors.PaleVioletRed);
                    Mistakes++;
                    Mistake.Text = $"Mistakes: {Mistakes}";
                }
            }

            if (Check())
            {
                MessageBox.Show("Win!");
                foreach (var childs in MainGrid.Children)
                {
                    Button bu = childs as Button;
                    bu.Background = new SolidColorBrush(Colors.White);
                }
            }
        }

        private bool Check()
        {

            foreach (var childs in MainGrid.Children)
            {

                Button child = childs as Button;
                if (child.Content == "" || child.Foreground.ToString() == "#FFFF0000")
                {
                    return false;
                }
            }

            return true;
        }

        private void Find(object sender, RoutedEventArgs e)
        {

            Button b = sender as Button;
            HighLight(b.Content.ToString());
            HighLightWrong(Grid.GetRow(b), Grid.GetColumn(b), b.Content.ToString());

        }

        private void HighLight(string num)
        {

            foreach (var childs in MainGrid.Children)
            {

                Button child = childs as Button;
                //if (child.Foreground == new SolidColorBrush(Colors.Red)) { 

                //    child.Background = new SolidColorBrush(Colors.Gray);
                //    child.Content = "";
                //}
                if (child.Content.ToString() == num)
                {
                    if (child.Foreground.ToString() == "#FFFF0000") child.Background = new SolidColorBrush(Colors.PaleVioletRed);
                    else child.Background = new SolidColorBrush(Colors.LightBlue);
                }
                else if (child.Content != "")
                {
                    child.Background = new SolidColorBrush(Colors.White);
                }
                else
                {
                    child.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
            Num(num);

        }

        private void HighLightWrong(int row, int col, string num)
        {

            foreach (var childs in MainGrid.Children)
            {

                Button child = childs as Button;
                if (child.Content.ToString() == "")
                {
                    child.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (child.Content.ToString() != num)
                {
                    child.Background = new SolidColorBrush(Colors.White);
                }
            }

            int cubeRow = row / 3;
            int cubeCol = col / 3;
            for (int i = 0; i < 9; i++)
            {
                for (int f = 0; f < 9; f++)
                {
                    if (i == row && f == col)
                    {
                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    else if (i == row)
                    {
                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else if (f == col)
                    {

                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else if (i >= cubeRow * 3 && i < (cubeRow + 1) * 3 && f >= cubeCol * 3 && f < (cubeCol + 1) * 3)
                    {

                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                }
            }
        }

        private void Num(string num)
        {

            SelectedNum = Convert.ToInt32(num);
            foreach (var childs in NButtons.Children)
            {
                Button child = childs as Button;
                child.Background = new SolidColorBrush(Colors.White);
                child.IsEnabled = true;
                if (child.Content.ToString() == num)
                {
                    child.IsEnabled = false;
                }
                if (CountNums[Convert.ToInt32(child.Content) - 1] == 9)
                {
                    child.Visibility = Visibility.Collapsed;
                    //child.IsEnabled = false;
                    if (child.Content.ToString() == num)
                        SelectedNum = 0;
                }

            }
            Count.Text = $"9/{CountNums[Convert.ToInt32(num) - 1]}";

        }

        private void CreateGrid()
        {

                for (int i = 0; i < 9; i++)
                {
                    MainGrid.RowDefinitions.Add(new());
                    MainGrid.ColumnDefinitions.Add(new());
                    for (int j = 0; j < 9; j++)
                    {

                        Button b = new();
                        //int val = Convert.ToInt32(vals[i][j]);
                        int val = grid[i,j];
                        b.FontSize = 20;
                        if (val == 0)
                        {
                            b.Content = "";
                            b.Background = new SolidColorBrush(Colors.LightGray);
                            b.Click += PlaceNum;
                        }
                        else
                        {
                            b.Content = val;
                            b.Background = new SolidColorBrush(Colors.White);
                            b.Click += Find;
                            CountNums[val - 1]++;
                        }
                        b.HorizontalAlignment = HorizontalAlignment.Stretch;
                        b.VerticalAlignment = VerticalAlignment.Stretch;
                        b.Margin = new Thickness(1);
                        Grid.SetRow(b, i);
                        Grid.SetColumn(b, j);
                        MainGrid.Children.Add(b);
                    }

                }

                //MessageBox.Show(table["value"].ToString());

        }
    }
    public static class MSSystemExtenstions
    {
        private static Random rng = new Random();
        public static void Shuffle<T>(this T[] array)
        {

            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n);
                n--;
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}