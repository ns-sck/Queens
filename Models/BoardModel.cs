using System;
using System.Collections.Generic;
using static Queeens.Controllers.HomeController;

namespace Queeens.Models
{
    public class BoardModel
    {
        public int[][] Board { get; private set; }
        public HashSet<Tuple<int, int>> ValidPositions { get; private set; }
        public BoardModel(int[][] board)
        {
            Board = board;
            UpdateValidPositions();
        }
        public void UpdateValidPositions()
        {
            ValidPositions = new HashSet<Tuple<int, int>>();
            int N = Board.Length;
            for (int i = 0; i < N; ++i)
            {
                for (int j = 0; j < N; ++j)
                {
                    if (Board[i][j] >= 100)
                    {
                        Board[i][j] -= 100;
                        ValidPositions.Add(new Tuple<int, int>(i, j));
                    }
                }
            }
        }
        public bool AreQueensCorrect(List<CellPosition> placedQueens)
        {
            Console.WriteLine(placedQueens.Count);
            Console.WriteLine(ValidPositions.Count);
            if (placedQueens.Count != ValidPositions.Count)
                return false;
            foreach (var (row, col) in ValidPositions)
            {
                if (!placedQueens.Any(q => q.Row == row && q.Col == col))
                    return false;
            }

            return true;
        }

    }
}
