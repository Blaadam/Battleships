using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        Brush HitColour = Brushes.Red;
        Brush ShipColour = Brushes.LightGray;
        Brush MissedColour = Brushes.White;
        Brush WaterColour = Brushes.CadetBlue;

        public String CurrentTurn = "ShipSelect";

        // Used to stop the user from clicking too much
        public bool Debounce = false;

        public void GameOver(bool PlayerWon)
        {
            if (PlayerWon)
            {
                Label1.Content = "YOU WON!";
                EndScreenText.Text = "GAME OVER! YOU WON";
            }
            else
            { 
                Label1.Content = "YOU LOST!";
                EndScreenText.Text = "GAME OVER! YOU LOST";
            }
            this.MainView.Visibility = Visibility.Hidden;
            this.EndScreen.Visibility = Visibility.Visible;
        }

        public void SetCellColour(Button button, Brush colour)
        {
            button.Background = colour;
            button.BorderBrush = colour;
        }

        public void SetShips(Grid ShipGrid)
        {
            // Collumns
            for (int i = 0; i < NumberOfColumns; i++)
            {
                // Rows
                for (int j = 0; j < NumberOfRows; j++)
                {
                    Button ShipButtonControl = new Button();
                    ShipButtonControl.Content = NumberToLetterMap.GetValue(i) + j.ToString();
                    ShipButtonControl.Name = NumberToLetterMap.GetValue(i) + j.ToString();
                    ShipButtonControl.Click += Button_Click;

                    SetCellColour(ShipButtonControl, WaterColour);

                    Grid.SetColumn(ShipButtonControl, j);
                    Grid.SetRow(ShipButtonControl, i);
                    ShipGrid.Children.Add(ShipButtonControl);
                }
            }
        }

        /// <summary>
        /// Returns a list containing the ship slot and the ship button slot.
        /// Order: [0] = ShipSlot, [1] = ShipButtonSlot
        /// </summary>
        /// <param name="BattleShipLocations"></param>
        /// <returns></returns>
        public List<int> GetPointerToShip(string[,] BattleShipLocations, string? ShipCoordinate)
        {

            List<int> Response = new List<int>();

            int ShipSlot = -1;
            int ShipButtonSlot = -1;

            // Cycles through the max combination of ships
            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] == ShipCoordinate)
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
            
            Response.Add(ShipSlot);
            Response.Add(ShipButtonSlot);

            return Response;
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

        public int GetNumberOfShipsRemaining(string[,] BattleShipLocations)
        {
            int ShipsLeft = 0;

            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] != null && BattleShipLocations[Ship, Space] != "")
                    {
                        ShipsLeft++;
                        break;
                    }
                }
            }

            return ShipsLeft;
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
                    if (BattleShipLocations[Ship, Space] == null || BattleShipLocations[Ship, Space] == "")
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

        public bool CheckIfShipIsAlreadyPlaced(string[,] BattleShipLocations, string ShipPlacement)
        {
            bool ShipExists = false;

            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    // Checks if a ship is being spawned on another ship
                    if (BattleShipLocations[Ship, Space] == ShipPlacement)
                    {
                        ShipExists = true;
                        break;
                    }
                }

                // Breaks out the loop if a slot is found
                if (ShipExists)
                {
                    break;
                }
            }

            return ShipExists;
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

            if (CheckIfShipIsAlreadyPlaced(BattleShipLocations, ShipPlacement))
            {
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
        public Button? GetCellFromGrid(Grid grid, string CellName)
        {
            foreach (UIElement child in grid.Children)
            {
                if (child is Button button && button.Name == CellName)
                {
                    return (Button)child;
                }
            }
            return null;
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

        public bool CheckIfCellHasShip(string[,] BattleShipLocations, string ShipCell)
        {
            bool HasShip = false;
            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] == ShipCell)
                    {
                        HasShip = true;
                        break;
                    }
                }

                // Breaks out the loop if a slot is found
                if (HasShip)
                {
                    break;
                }

            }

            return HasShip;
        }

        public bool ShipsStillExist(string[,] BattleShipLocations)
        {
            bool HasShip = false;

            string? CellName = null;
            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] != null && BattleShipLocations[Ship, Space] != "")
                    {
                        CellName = BattleShipLocations[Ship, Space];
                        break;
                    }
                }

                // Breaks out the loop if a ship part is found
                if (HasShip)
                {
                    break;
                }

            }

            if (CellName == null)
            {
                return false;
            }

            return HasShip;
        }

        public bool RemoveShipEntry(string[,] BattleShipLocations, string ShipCell)
        {
            bool DeletedShip = false;
            for (int Ship = 0; Ship < NumberOfShips; Ship++)
            {
                // Cycles through the max number of spaces of ships
                for (int Space = 0; Space < NumberOfPartsToAShip; Space++)
                {
                    if (BattleShipLocations[Ship, Space] == null || BattleShipLocations[Ship, Space] == "")
                    {
                        continue;
                    }
                    if (BattleShipLocations[Ship, Space] == ShipCell)
                    {
                        DeletedShip = true;
                        BattleShipLocations[Ship, Space] = "";
                        break;
                    }
                }

                // Breaks out the loop if a ship part is found
                if (DeletedShip)
                {
                    break;
                }
            }
            return DeletedShip;
        }

        public void HandlePlayerShot(string CellName, Button CellShot)
        {
            bool RemovedPlayersShip = RemoveShipEntry(OpponentBattleshipLocations, CellName);

            if (RemovedPlayersShip)
            {
                SetCellColour(CellShot, HitColour);
            }
            else
            {
                SetCellColour(CellShot, MissedColour);
            }

        }

        public void GenerateOpponentShot()
        {
            string CellShot = "";

            Button? DownedShip = null;

            while (DownedShip == null) {
                CellShot = GetRandomSpace();
                DownedShip = GetCellFromGrid(this.PlayerShips, CellShot);
                if (DownedShip != null && DownedShip.Background != WaterColour && DownedShip.Background != ShipColour)
                {
                    DownedShip = null;
                }
            }

            if (DownedShip == null)
            {
                return;
            }

            SetCellColour(DownedShip, MissedColour);

            Label1.Content = "They shot: " + CellShot;

            if (!CheckIfCellHasShip(PlayerBattleshipLocations ,CellShot))
            {
                Label1.Content = "Their shot " + CellShot + " missed";
                return;
            }

            RemoveShipEntry(PlayerBattleshipLocations, CellShot);

            SetCellColour(DownedShip, HitColour);
            Label1.Content = "Your ship was hit! " + CellShot;
        }

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
                    SetCellColour(b, ShipColour);
                }

               if (!GetBlankSpace(PlayerBattleshipLocations))
                {
                    GenerateOpponentShips();

                    CurrentTurn = "ShotSelect";
                }

                Debounce = false;
                return;
            }
            else if (CurrentTurn == "ShotSelect" && PlayerShots.IsAncestorOf(b))
            {
                if (b.Background != WaterColour)
                {
                    Debounce = false;
                    return;
                }

                Label1.Content = "Shot fired!";
                HandlePlayerShot(b.Name, b);
                await Task.Delay(1000);
                Label1.Content = "The opponent shot back!";
                GenerateOpponentShot();
            }

            Label1.Content = GetNumberOfShipsRemaining(PlayerBattleshipLocations).ToString() + " " + GetNumberOfShipsRemaining(OpponentBattleshipLocations).ToString();

           if (GetNumberOfShipsRemaining(OpponentBattleshipLocations) <= 0)
            {
                GameOver(true);
            }
            else if (GetNumberOfShipsRemaining(PlayerBattleshipLocations) <= 0)
            {
                GameOver(false);
            }
            Debounce = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(Process.GetCurrentProcess().MainModule.FileName);
            Application.Current.Shutdown();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }
    }
}
