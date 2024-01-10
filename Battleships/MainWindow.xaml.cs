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

        // Used to stop the user from clicking too much
        public bool Debounce = false;

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

        public bool GetBlankSpace(string[,] BattleShipLocations)
        {
            bool BlankSpace = false;

            // Cycles through the max combination of ships
            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] == null)
                    {
                        BlankSpace = true;
                        break;
                    }
                }

                if (BlankSpace)
                {
                    break;
                }
            }
            return BlankSpace;
        }

        public bool AuthorisedShipPlacement(string[,] BattleShipLocations, string ShipPlacement)
        {
            int ShipSlot = -1;
            int ShipButtonSlot = -1;

            // Cycles through the max combination of ships
            for (int Ship=0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space=0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] == null)
                    {
                        ShipSlot = Ship;
                        ShipButtonSlot = Space;
                        break;
                    }
                }

                // Breaks out the loop if a slot is found
                if (ShipSlot != -1 || ShipButtonSlot != -1)
                {
                    break;
                }

            }

            // If theres no available slots, deny the authorisation
            if (ShipSlot == -1 || ShipButtonSlot == -1)
            {
                Label1.Content = "No available slots";
                return false;
            }

            // Checks if theres already a point existing for that ship
            if (ShipButtonSlot != 0)
            {
                string PreviousSpace = BattleShipLocations[ShipSlot, ShipButtonSlot-1];
                string Prev_Column = PreviousSpace.Substring(0, 1);
                string Prev_Row = PreviousSpace.Substring(1);

                string Cur_Column = ShipPlacement.Substring(0, 1);
                string Cur_Row = ShipPlacement.Substring(1);

                if (PreviousSpace == ShipPlacement)
                {
                    Label1.Content = "Your points cannot be the same...";
                    return false;
                }

                // If the row or column doesn't match, reject the input
                if (Cur_Column != Prev_Column && Cur_Row != Prev_Row)
                {
                    Label1.Content = "You must choose a button in the same column or row!";
                    return false;
                }

                if ((GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) && GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) + 1 && GetIndexOfLetter(Cur_Column) != GetIndexOfLetter(Prev_Column) - 1) || (int.Parse(Cur_Row) != int.Parse(Prev_Row) && int.Parse(Cur_Row) != int.Parse(Prev_Row) - 1 && int.Parse(Cur_Row) != int.Parse(Prev_Row) + 1))
                {
                    Label1.Content = "Your points must be placed next to each other!";
                    return false;
                }

                Label1.Content = Prev_Column + " " + Prev_Row;
            }

            BattleShipLocations[ShipSlot, ShipButtonSlot] = ShipPlacement;
            return true;
        }

        public string GetRandomSpace()
        {
            Random RNG = new Random();
            int LetterIndex = RNG.Next(NumberOfColumns);
            string Letter = NumberToLetterMap[LetterIndex];

            int RandomRow = RNG.Next(NumberOfRows);

            string CellName = Letter + RandomRow.ToString();
            return CellName;
        }

        public void GenerateOpponentShips()
        {
            while (GetBlankSpace(OpponentBattleshipLocations))
            {
                string CellName = GetRandomSpace();
                AuthorisedShipPlacement(OpponentBattleshipLocations, CellName);
            }
            Label1.Content = "Enemy ships generated!";
        }

        public void GenerateOpponentShot()
        {
            string CellShot = GetRandomSpace();
        }

        public String CurrentTurn = "ShipSelect";

        public MainWindow()
        {
            InitializeComponent();

            SetShips(PlayerShips);
            SetShips(PlayerShots);
        }
        async private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Debounce)
            {
                return;
            }

            Debounce = true;
            Button b = (Button)sender;

            if (CurrentTurn == "ShipSelect" && PlayerShips.IsAncestorOf(b))
            {
                if (AuthorisedShipPlacement(PlayerBattleshipLocations, b.Name))
                {
                    b.Background = Brushes.Red;
                }

               if (!GetBlankSpace(PlayerBattleshipLocations))
                {
                    GenerateOpponentShips();

                    CurrentTurn = "ShotSelect";
                }
            }
            else if (CurrentTurn == "ShotSelect" && PlayerShots.IsAncestorOf(b))
            {
                Label1.Content = "Shot fired!";
                await Task.Delay(1000);
                Label1.Content = "The opponent shot back!";
            }
            Debounce = false;
        }
    }
}
