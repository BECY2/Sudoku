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
    public partial class MainWindow : Window
    {
        public int Mistakes = 0;
        public int SelectedNum = 0;
        public JToken solution;
        public int[] CountNums = new int[9];
        public List<List<int>> TheGrid = new List<List<int>>();
        public int[] Numbers = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        public MainWindow()
        {
            InitializeComponent();


            //string kiirt = "";
            //foreach (var item in Numbers)
            //{
            //    kiirt += item.ToString();
            //}
            //MessageBox.Show(kiirt);


            //for (int i = 0; i < 9; i++)
            //{
            //    TheGrid.Add(new List<int>());
            //    for (int f = 0; f < 9; f++)
            //    {
            //        TheGrid[i].Add(0);
            //    }
            //}

            //CretaeGrids(TheGrid);

            //MessageBox.Show(TheGrid.ToString());

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

            //MessageBox.Show(child.Foreground.ToString());

            CreateGrid();
        }

        private void SetNum(object sender, RoutedEventArgs s)
        {
            Button self = sender as Button;
            HighLight(self.Content.ToString());
        }

        private void PlaceNum(object sender, RoutedEventArgs s) { 
        
            Button b = sender as Button;
            int row = Grid.GetRow(b);
            int col = Grid.GetColumn(b);
            if (SelectedNum != 0) {

                int sol = Convert.ToInt32(solution[row][col]);
                if (sol == SelectedNum)
                {

                    b.Content = sol;
                    b.Click -= PlaceNum;
                    b.Click += Find;
                    b.Foreground = new SolidColorBrush(Colors.Blue);
                    HighLight(b.Content.ToString());
                    HighLightWrong(Grid.GetRow(b), Grid.GetColumn(b), b.Content.ToString());
                    CountNums[sol - 1]++;
                    Count.Text = $"9/{CountNums[sol - 1]}";
                    Num(sol.ToString());
                }
                else {

                    b.Content = SelectedNum;
                    b.Foreground = new SolidColorBrush(Colors.Red);
                    b.Background = new SolidColorBrush(Colors.PaleVioletRed);
                    Mistakes++;
                    Mistake.Text = $"Mistakes: {Mistakes}";
                }
            }

            if (Check()) {
                MessageBox.Show("Win!");
            }
        }

        private bool Check() { 
        
            foreach (var childs in MainGrid.Children) {
                
                Button child = childs as Button;
                if (child.Content == "" || child.Foreground.ToString() != "#FFFF0000") {
                    return false;
                }
            }

            return true;
        }

        private void Find(object sender, RoutedEventArgs e) {

            Button b = sender as Button;
            HighLight(b.Content.ToString());
            HighLightWrong(Grid.GetRow(b), Grid.GetColumn(b), b.Content.ToString());

        }

        private void HighLight(string num) {

            foreach (var childs in MainGrid.Children)
            {

                Button child = childs as Button;
                //if (child.Foreground == new SolidColorBrush(Colors.Red)) { 

                //    child.Background = new SolidColorBrush(Colors.Gray);
                //    child.Content = "";
                //}
                if (child.Content.ToString() == num)
                {
                    if(child.Foreground.ToString() == "#FFFF0000") child.Background = new SolidColorBrush(Colors.PaleVioletRed);
                    else child.Background = new SolidColorBrush(Colors.LightBlue);
                }
                else if (child.Content != "")
                {
                    child.Background = new SolidColorBrush(Colors.White);
                }
                else {
                    child.Background = new SolidColorBrush(Colors.LightGray);
                }
            }
            Num(num);
       
        }

        private void HighLightWrong(int row, int col, string num) {

            foreach (var childs in MainGrid.Children)
            {

                Button child = childs as Button;
                if (child.Content.ToString() == "")
                {
                    child.Background = new SolidColorBrush(Colors.LightGray);
                }
                else if (child.Content.ToString() != num) {
                    child.Background = new SolidColorBrush(Colors.White);
                }
            }

            int cubeRow = row / 3; 
            int cubeCol = col / 3;
            for (int i = 0; i < 9; i++)
            {
                for (int f = 0; f < 9; f++)
                {
                    if (i == row && f == col) {
                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.LightBlue);
                    }
                    else if (i == row)
                    {
                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else if (f == col) {

                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                    else if (i>=cubeRow*3 && i< (cubeRow+1) * 3 && f>= cubeCol*3 && f<(cubeCol+1)*3) {

                        Button b = MainGrid.Children[9 * i + f] as Button;
                        b.Background = new SolidColorBrush(Colors.Gray);
                    }
                }
            }
        }

        private void Num(string num) {

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
                if (CountNums[Convert.ToInt32(child.Content) - 1] == 9) {
                    child.Visibility = Visibility.Collapsed;
                    //child.IsEnabled = false;
                    if (child.Content.ToString() == num)
                        SelectedNum = 0;
                }
                
            }
            Count.Text = $"9/{CountNums[Convert.ToInt32(num)-1]}";
            
        }

        //private bool CheckGrid(List<List<int>> grid) {
        //    for (int i = 0; i < 9; i++)
        //    {
        //        for (int f = 0; f < 9; f++)
        //        {
        //            if (grid[i][f] == 0) return false;
        //        }
        //    }


        //    return true;
        //}


        //private bool CretaeGrids(List<List<int>> grid) {

        //    for (int row = 0; row < 9; row++)
        //    {
        //        for (int col = 0; col < 9; col++)
        //        {
        //            if (grid[row][col] ==0) {
        //                List<int> InCol = new List<int>();
        //                for (int f = 0; f < 9; f++)
        //                {
        //                    InCol.Add(grid[f][col]);
        //                }

        //                int cubeRow = row / 3;
        //                int cubeCol = col / 3;
        //                List<int> square = new List<int>();

        //                for (int k = 0; k < 9; k++)
        //                {
        //                    for (int l = 0; l < 9; l++)
        //                    {
        //                        if (k >= cubeRow * 3 && k < (cubeRow + 1) * 3 && l >= cubeCol * 3 && l < (cubeCol + 1) * 3) {

        //                            square.Add(grid[k][l]);
        //                        }
        //                        if (square.Count > 8) {
        //                            break;
        //                        }
        //                    }
        //                    if (square.Count > 8)
        //                    {
        //                        break;
        //                    }

        //                }

        //                Numbers.Shuffle<int>();
        //                for (int i = 0; i < 9; i++)
        //                {
        //                    if (!grid[row].Contains(Numbers[i]) && !InCol.Contains(Numbers[i]) && !square.Contains(Numbers[i]))
        //                    {
        //                        grid[row][col] = Numbers[i];
        //                        if (CheckGrid(grid)) { return true; }
        //                        else {
        //                            if (CretaeGrids(grid)) {

        //                                return true;
        //                            }
        //                        }

        //                    }
        //                    break;
        //                }
        //            }
        //            grid[row][col] = 0;
        //        }
        //    }
        //    TheGrid = grid;
        //    return true;
        //}


        private void CreateGrid()
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
                    MainGrid.RowDefinitions.Add(new());
                    MainGrid.ColumnDefinitions.Add(new());
                    for (int j = 0; j < 9; j++)
                    {

                        Button b = new();
                        int val = Convert.ToInt32(vals[i][j]);
                        b.FontSize = 20;
                        if (val == 0) { 
                            b.Content = "";
                            b.Background = new SolidColorBrush(Colors.LightGray);
                            b.Click += PlaceNum;
                        }
                        else { 
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