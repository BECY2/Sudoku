using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Sudoku
{
    /// <summary>
    /// Interaction logic for First.xaml
    /// </summary>
    public partial class First : Window
    {
        public First()
        {
            InitializeComponent();
        }

        public void StartGame(object sender, RoutedEventArgs s) {

            if (Choose.Visibility == Visibility.Visible)
            {
                if (Choose.SelectedItem != null)
                {
                    Inner.difficulty = (Choose.SelectedItem as ComboBoxItem).Content.ToString();
                    MainWindow mw = new MainWindow();
                    mw.Show();
                    Close();
                }
                else {
                    MessageBox.Show("Choose a difficulty");
                }
            }
            else
            {
                MainWindow mw = new MainWindow();
                mw.Show();
                Close();
            }
        }

        public void Api(object sender, RoutedEventArgs s) {

            Start.Visibility = Visibility.Visible;
            Inner.api = true;
        }
        public void Generate(object sender, RoutedEventArgs s)
        {

            Start.Visibility = Visibility.Visible;
            Choose.Visibility = Visibility.Visible;
            Inner.api = false;
        }
    }
}
