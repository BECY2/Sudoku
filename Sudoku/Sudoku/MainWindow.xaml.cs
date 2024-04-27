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
                    b.Background = new SolidColorBrush(Colors.AliceBlue);
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
                    child.Background = new SolidColorBrush(Colors.AliceBlue);
                }
                else if (child.Content != "")
                {
                    child.Background = new SolidColorBrush(Colors.White);
                }
            }
            Num(num);
       
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
                    child.IsEnabled = false;
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