using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Queeens.Models;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Queeens.Extensions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;


namespace Queeens.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IActionResult SetBoard(int[][] board)
        {
            HttpContext.Session.SetObject("Board", board);
            return RedirectToAction("Index");
        }

        public IActionResult GetBoard()
        {
            var board = HttpContext.Session.GetObject<int[][]>("Board");
            return View(board);
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var board = CreateBoard();
            return View(board);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult GenerateBoard()
        {
            var board = CreateBoard();
            return Json(new { success = true, board = board }); 
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        void colorBoard(int[][] board)
        {
            Random rnd = new Random();
            int N = board.Length;

            HashSet<Tuple<int, int>> put = [];
            while (put.Count < N)
            {
                int i = rnd.Next(0, N);
                int j = rnd.Next(0, N);
                put.Add(new Tuple<int, int>(i, j));
            }

            Queue<Tuple<int, int, int>> queue = new Queue<Tuple<int, int, int>>();
            int color = 0;
            foreach (var (x, y) in put)
            {
                queue.Enqueue(new Tuple<int, int, int>(color++, x, y));
            }

            int[] r = { 0, 1, 0, -1 };
            int[] c = { 1, 0, -1, 0 };
            while (queue.Count > 0)
            {
                var front = queue.Dequeue();
                int k = front.Item1;
                int i = front.Item2;
                int j = front.Item3;

                if (board[i][j] != -1) { continue; }
                board[i][j] = k;

                for (int d = 0; d < 4; d++)
                {
                    int ii = i + r[d];
                    int jj = j + c[d];
                    if (ii >= 0 && ii < N && jj >= 0 && jj < N)
                    {
                        queue.Enqueue(new Tuple<int, int, int>(k, ii, jj));
                    }
                }
            }
        }
        public int[][] createBoardCandidate(int n)
        {
            Random rnd = new Random();
            int N = rnd.Next(Math.Max(n, 6), 12);
            int[][] board = new int[N][];
            for (int i = 0; i < N; ++i)
            {
                board[i] = new int[N];
                for (int j = 0; j < N; ++j)
                {
                    board[i][j] = -1;
                }
            }
            colorBoard(board);
            return board;
        }

        public int getSolutionCount(int[][] board, int row, int prev, int colorMask, int colMask)
        {
            int N = board.Length;
            if (row == N)
            {
                int filled = (1 << N) - 1;
                return Convert.ToInt32(colorMask == filled && colMask == filled);
            }
            int ans = 0;
            for (int col = 0; col < N; ++col)
            {
                int color = board[row][col];
                if (((colorMask >> color) & 1) == 1 || ((colMask >> col) & 1) == 1 || Math.Abs(prev - col) < 2) { continue; }
                ans += getSolutionCount(board, row + 1, col, colorMask | (1 << color), colMask | (1 << col));
            }
            return ans;
        }

        public bool fillQueens(int[][] board, int row, int prev, int colorMask, int colMask)
        {
            int N = board.Length;
            if (row == N)
            {
                int filled = (1 << N) - 1;
                return colorMask == filled && colMask == filled;
            }
            for (int col = 0; col < N; ++col)
            {
                int color = board[row][col];
                if (((colorMask >> color) & 1) == 1 || ((colMask >> col) & 1) == 1 || Math.Abs(prev - col) < 2) { continue; }
                if (fillQueens(board, row + 1, col, colorMask | (1 << color), colMask | (1 << col)))
                {
                    board[row][col] += 100;
                    return true;
                }
            }
            return false;
        }

        public int[][] CreateBoard()
        {
            int trial = 2000;
            while (trial-- > 0)
            {
                int[][] board = createBoardCandidate(trial / 200);
                int x = getSolutionCount(board, 0, -100, 0, 0);
                if (x == 1)
                {
                    fillQueens(board, 0, -100, 0, 0);
                    var model = new BoardModel(board);
                    HttpContext.Session.SetObject("Board", model);
                    return board;
                }
            }
            return Array.Empty<int[]>();
        }

        [HttpPost]
        public IActionResult PlaceQueen([FromBody] CellPosition position)
        {
            BoardModel model = HttpContext.Session.GetObject<BoardModel>("Board");
            int[][] board = model.Board;
            if (board[position.Row][position.Col] >= 100)
            {
                board[position.Row][position.Col] -= 100;
            }
            else
            {
                board[position.Row][position.Col] += 100;
            }
            return Json(new { success = true });
        }
        public class CellPosition
        {
            public int Row { get; set; }
            public int Col { get; set; }
        }

        [HttpPost]
        public IActionResult ValidateBoard([FromBody] List<CellPosition> placedQueens)
        {
            var model = HttpContext.Session.GetObject<BoardModel>("Board");

            bool isCorrect = model.AreQueensCorrect(placedQueens);

            return Json(new { correct = isCorrect });
        }
    }
}
