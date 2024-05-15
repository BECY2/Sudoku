using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    internal class SudokuSolver
    {
            public int[,] board;

            public SudokuSolver(int[,] board)
            {
                this.board = board;
            }

            public bool SolveSudoku()
            {
                return Solve(0, 0);
            }

            private bool Solve(int row, int col)
            {
               
                if (row == board.GetLength(0))
                {
                    return true;
                }

      
                if (board[row, col] != 0)
                {
                    return Solve(col == board.GetLength(1) - 1 ? row + 1 : row, col == board.GetLength(1) - 1 ? 0 : col + 1);
                }

                for (int num = 1; num <= 9; num++)
                {
             
                    if (IsValidPlacement(row, col, num))
                    {
                        board[row, col] = num;

                        if (Solve(col == board.GetLength(1) - 1 ? row + 1 : row, col == board.GetLength(1) - 1 ? 0 : col + 1))
                        {
                            return true;
                        }

                        board[row, col] = 0;
                    }
                }

                return false;
            }

            private bool IsValidPlacement(int row, int col, int num)
            {

                for (int i = 0; i < board.GetLength(1); i++)
                {
                    if (board[row, i] == num)
                    {
                        return false;
                    }
                }

                for (int i = 0; i < board.GetLength(0); i++)
                {
                    if (board[i, col] == num)
                    {
                        return false;
                    }
                }

                int subBoxRowStart = (row / 3) * 3;
                int subBoxColStart = (col / 3) * 3;
                for (int i = subBoxRowStart; i < subBoxRowStart + 3; i++)
                {
                    for (int j = subBoxColStart; j < subBoxColStart + 3; j++)
                    {
                        if (board[i, j] == num)
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
    }
}
