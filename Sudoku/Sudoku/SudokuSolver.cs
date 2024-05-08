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
                // If we've reached the end of the board, the puzzle is solved
                if (row == board.GetLength(0))
                {
                    return true;
                }

                // If the current cell is already filled, move to the next one
                if (board[row, col] != 0)
                {
                    return Solve(col == board.GetLength(1) - 1 ? row + 1 : row, col == board.GetLength(1) - 1 ? 0 : col + 1);
                }

                // Try all possible numbers (1-9) for the current cell
                for (int num = 1; num <= 9; num++)
                {
                    // Check if the number is valid in the current row, column, and sub-box
                    if (IsValidPlacement(row, col, num))
                    {
                        board[row, col] = num;

                        // If the placement is valid, try solving the rest of the board
                        if (Solve(col == board.GetLength(1) - 1 ? row + 1 : row, col == board.GetLength(1) - 1 ? 0 : col + 1))
                        {
                            return true;
                        }

                        // Backtrack if the placement leads to an invalid solution
                        board[row, col] = 0;
                    }
                }

                // If no valid number is found for the current cell, the puzzle is not solvable
                return false;
            }

            private bool IsValidPlacement(int row, int col, int num)
            {
                // Check row
                for (int i = 0; i < board.GetLength(1); i++)
                {
                    if (board[row, i] == num)
                    {
                        return false;
                    }
                }

                // Check column
                for (int i = 0; i < board.GetLength(0); i++)
                {
                    if (board[i, col] == num)
                    {
                        return false;
                    }
                }

                // Check sub-box (adjust logic for different sub-box sizes)
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
