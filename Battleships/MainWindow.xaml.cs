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
        public const int NumberOfShips = 3;
        public const int NumberOfPartsToAShip = 2;  // Dont change

        public string[,] PlayerBattleshipLocations = new string[NumberOfShips, NumberOfPartsToAShip];
        public string[,] OpponentBattleshipLocations = new string[NumberOfShips, NumberOfPartsToAShip];
        string[] NumberToLetterMap = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "V", "W", "X", "Y", "Z" };

        public const int NumberOfRows = 6;
        public const int NumberOfColumns = 6;

        public void SetShips(Grid ShipGrid)
        {
            // Collumns
            for (int i = 0; i < NumberOfColumns; i++)
            {
                // Rows
                for (int j = 0; j < NumberOfRows; j++)
                {
                    Button MyControl1 = new Button();
                    MyControl1.Content = NumberToLetterMap.GetValue(i) + j.ToString();
                    MyControl1.Name = NumberToLetterMap.GetValue(i) + j.ToString();
                    MyControl1.Click += Button_Click;

                    Grid.SetColumn(MyControl1, j);
                    Grid.SetRow(MyControl1, i);
                    ShipGrid.Children.Add(MyControl1);
                }
            }
        }

        public bool AuthorisedShipPlacement(string ShipPlacement)
        {
            int ShipSlot = -1;
            int ShipButtonSlot = -1;

            // Get total number of ships to place
            //for (int i = 0; i < PlayerBattleshipLocations.Length-1; i++)
            for (int i = 0; i < NumberOfShips - 1; i++)
            {
                // Get number of spaces per ship
                for (int i_ = 0; i_ < NumberOfPartsToAShip - 1; i_++)
                {
                    // Check if its a new ship
                    if (i_ == 0 && PlayerBattleshipLocations[i, i_] == null)
                    {
                        ShipButtonSlot = 0;
                        ShipSlot = i;
                        break;
                    }
                    // Checks if its contributing to a current ship slot
                    else if (PlayerBattleshipLocations[i, i_] == null)
                    {
                        ShipButtonSlot = i_;
                        ShipSlot = i;
                    }
                }

                if (ShipSlot == -1 || ShipButtonSlot == -1)
                {
                    break;
                }

            }

            if (ShipSlot == -1 || ShipButtonSlot == -1)
            {
                return false;
            }

            PlayerBattleshipLocations[ShipSlot, ShipButtonSlot] = ShipPlacement;
            //Label1.Content = PlayerBattleshipLocations[ShipSlot, ShipButtonSlot];

            return true;
        }

        public String CurrentTurn = "ShipSelect";

        public MainWindow()
        {
            InitializeComponent();

            SetShips(PlayerShips);
            SetShips(PlayerShots);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;

            if (CurrentTurn == "ShipSelect" && PlayerShips.IsAncestorOf(b))
            {
                if (AuthorisedShipPlacement(b.Name))
                {
                    b.Background = Brushes.Red;
                }
            }
        }

    }
}
