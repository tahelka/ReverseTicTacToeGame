using System;
using System.Text;
using System.Linq;

namespace game
{
    internal class ConsoleIO
    {
        internal const int k_Quit = -1;

        internal int WelcomeUserAndGetNumOfPlayers()
        {
            Console.WriteLine("Welcome to Reverse Tic Tac Toe!");

            return getNumOfPlayers();
        }

        internal Player GetPlayerDetailsFromUser(char i_Symbol)
        {
            string name = string.Empty;
            bool isValidName = false;

            while (!isValidName)
            {
                Console.Write("Enter player name: ");
                name = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(name) && name.All(c => Char.IsLetter(c) || Char.IsWhiteSpace(c)))
                {
                    isValidName = true;
                }
                else
                {
                    Console.WriteLine("Invalid name. Please enter a name that contains only letters and spaces and is not empty.");
                }
            }

            return new Player(name, i_Symbol);
        }

        internal void DisplayWhoseTurn(string i_NameOfCurrentPlayer, char i_Symbol)
        {
            Console.WriteLine($"It's {i_NameOfCurrentPlayer}'s turn ({i_Symbol})");
        }

        internal void DisplayBoard(BoardGame i_Board)
        {
            StringBuilder sb = new StringBuilder();

            Ex02.ConsoleUtils.Screen.Clear();
            for (int j = 0; j < i_Board.BoardSize; j++)
            {
                sb.Append($"  {j + 1} ");
            }

            sb.AppendLine();
            for (int i = 0; i < i_Board.BoardSize; i++)
            {
                sb.Append($"{i + 1}|");
                for (int j = 0; j < i_Board.BoardSize; j++)
                {
                    sb.Append($" {i_Board.Cells[i, j].Symbol} ");
                    if (j < i_Board.BoardSize - 1)
                    {
                        sb.Append("|");
                    }
                }

                sb.Append("|");
                sb.AppendLine();
                if (i <= i_Board.BoardSize)
                {
                    sb.Append(" ");
                    sb.Append(new string('=', i_Board.BoardSize * 4 + 1));
                    sb.AppendLine();
                }
            }

            Console.WriteLine(sb.ToString());
        }

        internal void DisplayCellIsOccupiedMsg()
        {
            Console.Write("Cell is already occupied! Please choose an empty cell.\n");
        }

        internal int GetBoardSizeFromPlayer()
        {
            bool isValidSize = false;
            int size;

            do
            {
                Console.Write($"Enter the size of the board ({BoardGame.k_MinSizeOfBoard}-{BoardGame.k_MaxSizeOfBoard}): ");
                string input = Console.ReadLine();
                if (!int.TryParse(input, out size) || size < BoardGame.k_MinSizeOfBoard || size > BoardGame.k_MaxSizeOfBoard)
                {
                    Console.Write("invalid input! try again\n");
                    continue;
                }

                isValidSize = true;
            } 
            while (isValidSize == false);

            return size;
        }

        internal bool GetMoveFromPlayer(int i_BoardSize, out int o_row, out int o_col)
        {
            bool doesPlayerWantToQuit = true;

            o_row = getFromUserBoardValueOfDimension(i_BoardSize, "row");
            if (o_row != k_Quit)
            {
                o_col = getFromUserBoardValueOfDimension(i_BoardSize, "column");
                if(o_col != k_Quit)
                {
                    doesPlayerWantToQuit = false;
                }
            }
            else
            {
                o_col = k_Quit;
            }

            return doesPlayerWantToQuit;
        }

        private int getFromUserBoardValueOfDimension(int i_BoardSize, string i_Dimension)
        {
            int valueOfDimension;
            bool isValidInput = false;

            do
            {
                Console.Write($"Enter the {i_Dimension} number of your move (1-{i_BoardSize}): ");
                string input = Console.ReadLine().Trim();
                if (checkInputForQuittingTheGame(input))
                {
                    valueOfDimension = k_Quit;
                    isValidInput = true;
                }
                else if (!int.TryParse(input, out valueOfDimension) || valueOfDimension < BoardGame.k_MinValOfDimension || valueOfDimension > i_BoardSize)
                {
                    Console.Write($"Invalid input! Please enter a valid {i_Dimension} number.\n");
                }
                else
                {
                    isValidInput = true;
                }
            } 
            while (!isValidInput);

            return valueOfDimension;
        }

        private void displaySummary(GameLogic i_Game)
        {
            if(i_Game.Player1.Forfeited)
            {
                Console.WriteLine($"{i_Game.Player1.Name} forfeits!");
            }
            else if(i_Game.Player2.Forfeited)
            {
                Console.WriteLine($"{i_Game.Player2.Name} forfeits!");
            }
            else if (i_Game.Winner == null)
            {
                Console.WriteLine("It's a tie!");
            }
            else
            {
                Console.WriteLine($"{i_Game.Winner.Name} ({i_Game.Winner.Symbol}) won!");
            }

            Console.WriteLine($@"Score summary:
{i_Game.Player1.Name} has {i_Game.Player1.Score} points.
{i_Game.Player2.Name} has {i_Game.Player2.Score} points.");
        }

        internal bool DoesPlayerWantToPlayAnotherRound()
        {
            bool result = true;

            Console.WriteLine("Press any key to play again or 'Q' to quit the game");
            string input = Console.ReadLine();
            if(checkInputForQuittingTheGame(input))
            {
                result = false;
            }

            return result;
        }

        private int getNumOfPlayers()
        {
            bool isValidInput = false;
            int numPlayers = 0;

            do
            {
                Console.Write("Enter number of players (1 or 2): ");
                string input = Console.ReadLine()?.Trim();
                if (int.TryParse(input, out numPlayers) && (numPlayers == 1 || numPlayers == 2))
                {
                    break;
                }

                Console.WriteLine("Invalid input. Please enter 1 or 2.");
            } 
            while (!isValidInput);

            return numPlayers;
        }

        private bool checkInputForQuittingTheGame(string i_Input)
        {
            return i_Input == "Q";
        }

        internal void DisplayTheFinalBoardAndSummary(GameLogic i_Game)
        {
            DisplayBoard(i_Game.Board);
            displaySummary(i_Game);
        }
    }
}