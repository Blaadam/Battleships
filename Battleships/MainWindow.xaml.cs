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

        // Stores the locations of the player & opponent battle ships
        public string[,] PlayerBattleshipLocations = new string[NumberOfShips, NumberOfPartsToAShip];
        public string[,] OpponentBattleshipLocations = new string[NumberOfShips, NumberOfPartsToAShip];

        // Stores the shots from the player and opponent
        public string[,] PlayerHits = new string[NumberOfShips, NumberOfPartsToAShip];
        public string[,] OpponentHits = new string[NumberOfShips, NumberOfPartsToAShip];

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

        public int GetIndexOfLetter(string Letter)
        {
            int ReturnedIndex = -1;

            for (int i = 0; i < NumberToLetterMap.Length; i++)
            {
                if (Letter == NumberToLetterMap[i])
                {
                    ReturnedIndex = i;
                    break;
                }
            }

            return ReturnedIndex;
        }

        public bool AuthorisedShipPlacement(string ShipPlacement)
        {
            int ShipSlot = -1;
            int ShipButtonSlot = -1;

            // Get total number of ships to place
            //for (int i = 0; i < PlayerBattleshipLocations.Length-1; i++)


            // Cycles through the max combination of ships
            for (int Ship=0; Ship < NumberOfShips; Ship++)
            {

                // Cycles through the max number of spaces of ships
                for (int Space=0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (Space == 0 && PlayerBattleshipLocations[Ship, Space] == null)
                    {
                        ShipSlot = Ship;
                        ShipButtonSlot = Space;
                        break;
                    }
                    else if (PlayerBattleshipLocations[Ship, Space] == null)
                    {
                        ShipSlot = Ship;
                        ShipButtonSlot = Space;
                        break;
                    }
                }

                if (ShipSlot != -1 || ShipButtonSlot != -1)
                {
                    break;
                }

            }

            if (ShipSlot == -1 || ShipButtonSlot == -1)
            {
                return false;
            }

            if (ShipButtonSlot != 0)
            {
                string PreviousSpace = PlayerBattleshipLocations[ShipSlot, ShipButtonSlot-1];
                string Prev_Column = PreviousSpace.Substring(0, 1);
                string Prev_Row = PreviousSpace.Substring(1);

                string Cur_Column = ShipPlacement.Substring(0, 1);
                string Cur_Row = ShipPlacement.Substring(1);

                // If the row or column doesn't match, reject the input
                if (Cur_Column != Prev_Column && Cur_Row != Prev_Row)
                {
                    return false;
                }

                if (GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) && GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) + 1 && GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) - 1)
                {
                    return false;
                }

                Label1.Content = Prev_Column + " " + Prev_Row;

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
