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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[,] BattleshipLocations = { { "1", "4", "2" } };

        public MainWindow()
        {
            InitializeComponent();

            int count = 1;
            
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    Button MyControl1 = new Button();
                    MyControl1.Content = count.ToString();
                    MyControl1.Name = "Button" + count.ToString();
                    MyControl1.Click += Button_Click;

                    Grid.SetColumn(MyControl1, j);
                    Grid.SetRow(MyControl1, i);
                    PlayerShips.Children.Add(MyControl1);
                    count = count + 1;
                }
            }
            

        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            b.Background = Brushes.Red;
        }

    }
}
