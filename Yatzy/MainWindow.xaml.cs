using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Yatzy
{
    public partial class MainWindow : Window
    {
        public int DiceOne { get; set; }
        public int DiceTwo { get; set; }
        public int DiceThree { get; set; }
        public int DiceFour { get; set; }
        public int DiceFive { get; set; }
        public int RollsLeft { get; set; } = 3;
        public int CurrentPlayer { get; set; } = 1;
        public int TurnsPlayed { get; set; }
        public int POneTopTableOptions { get; set; }
        public int PTwoTopTableOptions { get; set; }
        public int POneTopTableScore { get; set; }
        public int PTwoTopTableScore { get; set; }
        public int POneTotalScore { get; set; }
        public int PTwoTotalScore { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void RollDice(object sender, RoutedEventArgs e)
        {
            if (RollsLeft == 0)
                MessageBox.Show("You've no rolls left.");
            else
            {
                Random dice = new Random();
                if (!(CheckBoxDiceOne.IsChecked ?? false)) { DiceOne = dice.Next(1, 7); }
                if (!(CheckBoxDiceTwo.IsChecked ?? false)) { DiceTwo = dice.Next(1, 7); }
                if (!(CheckBoxDiceThree.IsChecked ?? false)) { DiceThree = dice.Next(1, 7); }
                if (!(CheckBoxDiceFour.IsChecked ?? false)) { DiceFour = dice.Next(1, 7); }
                if (!(CheckBoxDiceFive.IsChecked ?? false)) { DiceFive = dice.Next(1, 7); }
                SetDiceImages();
                TextBoxRollsLeft.Text = "Rolls left: " + --RollsLeft;
                EnableCheckBoxes(true);
            }
        }

        public void NewTurn()
        {
            ResetDiceImages();
            EnableCheckBoxes(false);
            CheckCheckBoxes(false);
            if (++TurnsPlayed == 30) // A game of Yatzy is 30 turns, 15 per player, which means the game is now over.
                GameOver();
            else
            {
                RollsLeft = 3;
                TextBoxRollsLeft.Text = "Rolls left: 3";
                CurrentPlayer = CurrentPlayer == 1 ? 2 : 1;
                WhosTurn.Text = CurrentPlayer == 1 ? pOneName.Text : pTwoName.Text;
            }
        }

        public void GameOver()
        {
            pOneTotal.Text = POneTotalScore.ToString();
            pTwoTotal.Text = PTwoTotalScore.ToString();
            YourTurn.Text = "Game Over!";
            if (POneTotalScore == PTwoTotalScore)
            {
                WhosTurn.Text = "The game is";
                TextBoxRollsLeft.Text = "a draw.";
            }
            else
            {
                WhosTurn.Text = "The Winner is:";
                TextBoxRollsLeft.Text = POneTotalScore > PTwoTotalScore ? pOneName.Text : pTwoName.Text;
            }
            ButtonRollDice.IsEnabled = false;
        }

        public void POneScoreBoard_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsValidChoice(sender, 1))
                return;
            int score = CalculateScore(((TextBox)sender).Tag.ToString());
            POneTotalScore += score;
            ((TextBox)sender).Text = score.ToString();
            NewTurn();
        }

        public void PTwoScoreBoard_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!IsValidChoice(sender, 2))
                return;
            int score = CalculateScore(((TextBox)sender).Tag.ToString());
            PTwoTotalScore += score;
            ((TextBox)sender).Text = score.ToString();
            NewTurn();
        }

        public bool IsValidChoice(object sender, int player)
        {
            return HasRolled() && IsPlayersScoreBoard(player) && !HasOptionBeenChosen(sender);
        }

        public bool HasRolled()
        {
            if (RollsLeft == 3)
            {
                MessageBox.Show("You've to roll the dice first.");
                return false;
            }
            return true;
        }

        public bool IsPlayersScoreBoard(int player)
        {
            if (player != CurrentPlayer)
            {
                string opponent = CurrentPlayer == 1 ? pTwoName.Text : pOneName.Text;
                MessageBox.Show("This scoreboard belongs to " + opponent + ".");
                return false;
            }
            return true;
        }

        public bool HasOptionBeenChosen(object sender)
        {
            if (Int32.TryParse(((TextBox)sender).Text, out int temp)) // If you can parse it, it has already been assigned a number, and hence been chosen before.
            {
                MessageBox.Show("You've already chosen that option.");
                return true;
            }
            else
                return false;
        }

        public int CalculateScore(string option)
        {
            int score = 0;
            switch (option)
            {
                case "Ones":
                    score = TopTableResult(1);
                    break;
                case "Twos":
                    score = TopTableResult(2);
                    break;
                case "Threes":
                    score = TopTableResult(3);
                    break;
                case "Fours":
                    score = TopTableResult(4);
                    break;
                case "Fives":
                    score = TopTableResult(5);
                    break;
                case "Sixes":
                    score = TopTableResult(6);
                    break;
                case "OnePair":
                    score = OnePair();
                    break;
                case "TwoPair":
                    score = TwoPair();
                    break;
                case "ThreeOfAKind":
                    score = ThreeOfAKind();
                    break;
                case "FourOfAKind":
                    score = FourOfAKind();
                    break;
                case "SmallStraight":
                    score = SmallStraight();
                    break;
                case "LargeStraight":
                    score = LargeStraight();
                    break;
                case "FullHouse":
                    score = FullHouse();
                    break;
                case "Chance":
                    score = Chance();
                    break;
                case "Yatzy":
                    score = Yatzy();
                    break;
            }
            return score;
        }

        public int TopTableResult(int value)
        {
            int result = 0;
            if (DiceOne == value) { result += value; }
            if (DiceTwo == value) { result += value; }
            if (DiceThree == value) { result += value; }
            if (DiceFour == value) { result += value; }
            if (DiceFive == value) { result += value; }
            if (CurrentPlayer == 1) { POneTopTableOption(result); }
            if (CurrentPlayer == 2) { PTwoTopTableOption(result); }
            return result;
        }

        public void POneTopTableOption(int score)
        {
            POneTopTableScore += score;
            if (++POneTopTableOptions == 6)
            {
                pOneSum.Text = POneTopTableScore.ToString();
                POneTotalScore = POneTopTableScore >= 63 ? POneTotalScore + 50 : POneTotalScore;
                pOneBonus.Text = POneTopTableScore >= 63 ? "50" : "0";
            }
        }

        public void PTwoTopTableOption(int score)
        {
            PTwoTopTableScore += score;
            if (++PTwoTopTableOptions == 6)
            {
                pTwoSum.Text = PTwoTopTableScore.ToString();
                PTwoTotalScore = PTwoTopTableScore >= 63 ? PTwoTotalScore + 50 : PTwoTotalScore;
                pTwoBonus.Text = PTwoTopTableScore >= 63 ? "50" : "0";
            }
        }

        public int OnePair()
        {
            int result = 0;
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            diceList.Reverse();
            if (diceList[0] == diceList[1])
                result = diceList[0] * 2;
            else if (diceList[1] == diceList[2])
                result = diceList[1] * 2;
            else if (diceList[2] == diceList[3])
                result = diceList[2] * 2;
            else if (diceList[3] == diceList[4])
                result = diceList[3] * 2;
            return result;
        }

        public int TwoPair()
        {
            int result = 0;
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            diceList.Reverse();
            if (diceList[0] == diceList[1] && diceList[2] == diceList[3])
                result = diceList[0] * 2 + diceList[2] * 2;
            else if (diceList[0] == diceList[1] && diceList[3] == diceList[4])
                result = diceList[0] * 2 + diceList[3] * 2;
            else if (diceList[1] == diceList[2] && diceList[3] == diceList[4])
                result = diceList[1] * 2 + diceList[3] * 2;
            return result;
        }

        public int ThreeOfAKind()
        {
            int result = 0;
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            diceList.Reverse();
            if (diceList[0] == diceList[1] && diceList[1] == diceList[2])
                result = diceList[0] * 3;
            else if (diceList[1] == diceList[2] && diceList[2] == diceList[3])
                result = diceList[1] * 3;
            else if (diceList[2] == diceList[3] && diceList[3] == diceList[4])
                result = diceList[2] * 3;
            return result;
        }

        public int FourOfAKind()
        {
            int result = 0;
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            diceList.Reverse();
            if (diceList[0] == diceList[1] && diceList[1] == diceList[2] && diceList[2] == diceList[3])
                result = diceList[0] * 4;
            else if (diceList[1] == diceList[2] && diceList[2] == diceList[3] && diceList[3] == diceList[4])
                result = diceList[1] * 4;
            return result;
        }

        public int SmallStraight()
        {
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            if (diceList[0] == 1 && diceList[1] == 2 && diceList[2] == 3 && diceList[3] == 4 && diceList[4] == 5)
                return 15;
            else
                return 0;
        }

        public int LargeStraight()
        {
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            if (diceList[0] == 2 && diceList[1] == 3 && diceList[2] == 4 && diceList[3] == 5 && diceList[4] == 6)
                return 20;
            else
                return 0;
        }

        public int FullHouse()
        {
            int result = 0;
            List<int> diceList = new List<int> { DiceOne, DiceTwo, DiceThree, DiceFour, DiceFive };
            diceList.Sort();
            diceList.Reverse();
            if (diceList[0] == diceList[1] && diceList[1] == diceList[2] && diceList[3] == diceList[4])
                result = diceList.Sum();
            else if (diceList[0] == diceList[1] && diceList[2] == diceList[3] && diceList[3] == diceList[4])
                result = diceList.Sum();
            return result;
        }

        public int Chance()
        {
            return DiceOne + DiceTwo + DiceThree + DiceFour + DiceFive;
        }

        public int Yatzy()
        {
            if (DiceOne == DiceTwo && DiceTwo == DiceThree && DiceThree == DiceFour && DiceFour == DiceFive)
                return 50;
            else
                return 0;
        }

        public void SetDiceImages()
        {
            ImageDiceOne.Source = new BitmapImage(new Uri(@"/Images/d" + DiceOne + ".png", UriKind.Relative));
            ImageDiceTwo.Source = new BitmapImage(new Uri(@"/Images/d" + DiceTwo + ".png", UriKind.Relative));
            ImageDiceThree.Source = new BitmapImage(new Uri(@"/Images/d" + DiceThree + ".png", UriKind.Relative));
            ImageDiceFour.Source = new BitmapImage(new Uri(@"/Images/d" + DiceFour + ".png", UriKind.Relative));
            ImageDiceFive.Source = new BitmapImage(new Uri(@"/Images/d" + DiceFive + ".png", UriKind.Relative));
        }

        public void ResetDiceImages()
        {
            ImageDiceOne.Source = new BitmapImage(new Uri(@"/Images/d0.png", UriKind.Relative));
            ImageDiceTwo.Source = new BitmapImage(new Uri(@"/Images/d0.png", UriKind.Relative));
            ImageDiceThree.Source = new BitmapImage(new Uri(@"/Images/d0.png", UriKind.Relative));
            ImageDiceFour.Source = new BitmapImage(new Uri(@"/Images/d0.png", UriKind.Relative));
            ImageDiceFive.Source = new BitmapImage(new Uri(@"/Images/d0.png", UriKind.Relative));
        }

        public void EnableCheckBoxes(bool option)
        {
            CheckBoxDiceOne.IsEnabled = option;
            CheckBoxDiceTwo.IsEnabled = option;
            CheckBoxDiceThree.IsEnabled = option;
            CheckBoxDiceFour.IsEnabled = option;
            CheckBoxDiceFive.IsEnabled = option;
        }

        public void CheckCheckBoxes(bool option)
        {
            CheckBoxDiceOne.IsChecked = option;
            CheckBoxDiceTwo.IsChecked = option;
            CheckBoxDiceThree.IsChecked = option;
            CheckBoxDiceFour.IsChecked = option;
            CheckBoxDiceFive.IsChecked = option;
        }
    }
}