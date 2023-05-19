using System;

namespace game
{
    internal class GameLogic
    {
        private const int k_FirstRound = 0;
        private const int k_ScorePointsForWinningOrLosing = 10;
        private const int k_MaxDepthForMiniMaxAlgorithm = 8;
        internal Player Player1 { get; set; }
        internal Player Player2 { get; set; }
        internal Player CurrentPlayer { get; set; }
        internal Player Winner { get; set; }
        internal BoardGame Board { get; set; }
        internal int NumOfRounds { get; set; }
        internal bool IsRoundOver { get; set; }

        internal GameLogic(Player i_Player1, Player i_Player2, int i_BoardSize)
        {
            Player1 = i_Player1;
            Player2 = i_Player2;
            CurrentPlayer = (NumOfRounds) % 2 == 0 ? Player1 : Player2;
            Board = new BoardGame(i_BoardSize);
        }

        private void updatePointsAfterPlayerForfeits()
        {
            if (Player1 == CurrentPlayer)
            {
                Player2.Score++;
            }
            else
            {
                Player1.Score++;
            }
        }

        internal void ApplyMove(int i_X, int i_Y)
        {
            Board.EmptyCells.Remove(Board.Cells[i_X, i_Y]);
            Board.SetCellSymbol(i_X, i_Y, CurrentPlayer.Symbol);
            checkGameStatusAndUpdate();
            if (!IsRoundOver)
            {
                switchPlayer();
            }
        }

        private void playComputerTurnWithoutMinimaxAlgorithm()
        {
            Random rnd = new Random();
            bool isRandomCellHasFound = false;

            while (!isRandomCellHasFound)
            {
                int row = rnd.Next(Board.BoardSize);
                int col = rnd.Next(Board.BoardSize);

                if (Board.IsCellOnBoardEmpty(row, col) && !isCurrentPlayerMightLose(row, col))
                {                  
                    ApplyMove(row, col);
                    isRandomCellHasFound = true;                   
                }
            }
        }

        private bool isCurrentPlayerMightLose(int i_Row, int i_Col)
        {
            Board.SetCellSymbol(i_Row, i_Col, CurrentPlayer.Symbol);
            bool didCurrentPlayerLose = checkForWinner();
            Board.CleanCell(i_Row, i_Col);

            return didCurrentPlayerLose;
        }

        internal void ApplyComputerPlayerTurn()
        {
            bool isNumOfCellsAboveMaxDepth = Board.EmptyCells.Count > k_MaxDepthForMiniMaxAlgorithm;

            if (isNumOfCellsAboveMaxDepth)
            {
                playComputerTurnWithoutMinimaxAlgorithm();
            }
            else
            {
                findNextMoveForComputerPlayer(out int row, out int col, 0);
                ApplyMove(row, col);
            }
        }

        private int minimaxAlgorithm(int i_Depth)
        {
            int scoreOfAlgorithm = 0;
            bool didCurrentPlayerLose = checkForWinner();

            if (didCurrentPlayerLose)
            {
                if (CurrentPlayer.IsComputer)
                {
                    scoreOfAlgorithm = -k_ScorePointsForWinningOrLosing; 
                }
                else
                {
                    scoreOfAlgorithm = k_ScorePointsForWinningOrLosing;
                }
            }
            else if (!Board.IsBoardFull())
            {
                switchPlayer();
                scoreOfAlgorithm = findNextMoveForComputerPlayer(out int row, out int col, i_Depth + 1);
                switchPlayer();
            }

            return scoreOfAlgorithm;
        }

        private int findNextMoveForComputerPlayer(out int o_Row, out int o_Col, int i_Depth)
        {
            int bestScore;

            o_Col = 0;
            o_Row = 0;
            if (CurrentPlayer.IsComputer)
            {
                bestScore = int.MinValue;
            }
            else
            {
                bestScore = int.MaxValue;
            }

            for (int i = 0; i < Board.EmptyCells.Count; i++)
            {
                Cell currCell = Board.EmptyCells[i];
                int scoreOfAlgorithm = checkTheScoreOfNextMove(currCell, i_Depth);
                bool isAlgorithmScoreBetter = (CurrentPlayer.IsComputer && scoreOfAlgorithm > bestScore) || (!CurrentPlayer.IsComputer && scoreOfAlgorithm < bestScore);
                if (isAlgorithmScoreBetter)
                {
                    bestScore = scoreOfAlgorithm;
                    o_Row = currCell.XDimension;
                    o_Col = currCell.YDimension;
                }
            }

            return bestScore;
        }

        private int checkTheScoreOfNextMove(Cell i_CellOfNextMove, int i_Depth)
        {
            Board.EmptyCells.Remove(i_CellOfNextMove);
            Board.SetCellSymbol(i_CellOfNextMove.XDimension, i_CellOfNextMove.YDimension, CurrentPlayer.Symbol);
            int scoreOfAlgorithm = minimaxAlgorithm(i_Depth);
            Board.CleanCell(i_CellOfNextMove.XDimension, i_CellOfNextMove.YDimension);
            Board.EmptyCells.Insert(0, i_CellOfNextMove);

            return scoreOfAlgorithm;
        }

        private void switchPlayer()
        {
            CurrentPlayer = CurrentPlayer == Player1 ? Player2 : Player1;
        }

        private void checkGameStatusAndUpdate()
        {
            if (checkForWinner())
            {
                IsRoundOver = true;
                switchPlayer();
                Winner = CurrentPlayer;
                Winner.Score++;
            }
            else if (Board.IsBoardFull())
            {
                IsRoundOver = true;
            }
        }

        private bool checkForWinner()
        {
            return (checkRowsForWinner() || checkColumnsForWinner() || checkDiagonalsForWinner());
        }

        private bool checkRowsForWinner()
        {
            bool isWinner = false;

            for (int i = 0; i < Board.BoardSize; i++)
            {
                bool isLosingRow = true;
                for (int j = 0; j < Board.BoardSize; j++)
                {
                    if (Board.GetCellSymbol(i, j) != CurrentPlayer.Symbol)
                    {
                        isLosingRow = false;
                        break;
                    }
                }

                if (isLosingRow)
                {
                    isWinner = true;
                    break;
                }
            }

            return isWinner;
        }

        private bool checkColumnsForWinner()
        {
            bool isWinner = false;

            for (int j = 0; j < Board.BoardSize; j++)
            {
                bool isLosingColumn = true;
                for (int i = 0; i < Board.BoardSize; i++)
                {
                    if (Board.GetCellSymbol(i, j) != CurrentPlayer.Symbol)
                    {
                        isLosingColumn = false;
                        break;
                    }
                }

                if (isLosingColumn)
                {
                    isWinner = true;
                    break;
                }
            }

            return isWinner;
        }

        private bool checkDiagonalsForWinner()
        {
            bool isLosingDiagonal1 = true;
            bool isLosingDiagonal2 = true;

            for (int i = 0, j = 0; i < Board.BoardSize; i++, j++)
            {
                if (Board.GetCellSymbol(i, j) != CurrentPlayer.Symbol)
                {
                    isLosingDiagonal1 = false;
                }

                if (Board.GetCellSymbol(i, Board.BoardSize - 1 - j) != CurrentPlayer.Symbol)
                {
                    isLosingDiagonal2 = false;
                }
            }

            return isLosingDiagonal1 || isLosingDiagonal2;
        }

        internal void SetGameForNewRound()
        {
            if (NumOfRounds++ != k_FirstRound)
            {
                CurrentPlayer.Forfeited = false;
                CurrentPlayer = (NumOfRounds) % 2 == 0 ? Player1 : Player2;
                Board = new BoardGame(Board.BoardSize);
                IsRoundOver = false;
                Winner = null;
            }
        }

        internal void PrepareGameForQuitting()
        {
            IsRoundOver = true;
            CurrentPlayer.Forfeited = true;
            updatePointsAfterPlayerForfeits();
        }
    }
}