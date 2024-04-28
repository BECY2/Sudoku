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
        public MainWindow()
        {
            InitializeComponent();


            for (int i = 0; i < 9; i++)
            {
                NButtons.ColumnDefinitions.Add(new());
                Button b = new();
                b.Content = (i + 1).ToString();
                b.HorizontalAlignment = HorizontalAlignment.Stretch;
                b.VerticalAlignment = VerticalAlignment.Stretch;
                b.Background = new SolidColorBrush(Colors.White);
                b.Margin = new Thickness(0,10,0,10);
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
                    b.Background = new SolidColorBrush(Colors.LightBlue);
                    CountNums[sol - 1]++;
                    Count.Text = $"9/{CountNums[sol - 1]}";
                    Num(sol.ToString());
                }
                else {

                    b.Content = SelectedNum;
                    b.Foreground = new SolidColorBrush(Colors.Red);
                    b.Background = new SolidColorBrush(Colors.White);
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
                if (child.Content == "" || child.Foreground != new SolidColorBrush(Colors.Red)) {
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
}