using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _6_TicTacToe
{
    public class Program
    {
        public static void Main()
        {
            var game = new Game();
            game.Start();
        }
    }

    public class Game
    {
        private readonly Regex inputTest = new Regex(@"\A\d{1}\Z");

        private readonly int[,] gameState = new int[,]
        {
            { 0, 0, 0 },
            { 0, 0, 0 },
            { 0, 0, 0 }
        };

        private const int stateCol1 = 4;
        private const int stateCol2 = 13;
        private const int stateCol3 = 22;
        private const int stateRow1 = 3;
        private const int stateRow2 = 11;
        private const int stateRow3 = 19;

        private readonly int gameFieldOffetLeft = 0;

        private readonly Point[,] gameStateCoord = new Point[,]
        {
            { new Point(stateCol1, stateRow1), new Point(stateCol2, stateRow1), new Point(stateCol3, stateRow1) },
            { new Point(stateCol1, stateRow2), new Point(stateCol2, stateRow2), new Point(stateCol3, stateRow2) },
            { new Point(stateCol1, stateRow3), new Point(stateCol2, stateRow3), new Point(stateCol3, stateRow3) }
        };

        private string[] log = new string[11];

        private readonly string[] xSign = new string[]
        {
            "█    █",
            " █  █ ",
            "  ██  ",
            " █  █ ",
            "█    █"
        };

        private readonly string[] oSign = new string[]
        {
            "  ██  ",
            " █  █ ",
            "█    █",
            " █  █ ",
            "  ██  "
        };

        private readonly string[] gameField = new string[]
        {
            "▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄",
            "█                              █",
            "█   1      ║ 2      ║ 3        █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█   ═══════╬════════╬════════  █",
            "█   4      ║ 5      ║ 6        █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█  ════════╬════════╬════════  █",
            "█   7      ║ 8      ║ 9        █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█          ║        ║          █",
            "█                              █",
            "▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀▀",
        };

        private int winner = 0;
        private int currentPlayer = 1;

        public Game()
        {
            gameFieldOffetLeft = stateRow3 + xSign[0].Length + 10;
        }

        public void Start()
        {
            while (!GameIsEnded())
            {
                DrawField();
                DrawHelp();
                DrawStatus();

                var actionRaw = Console.ReadLine();
                if (inputTest.IsMatch(actionRaw) && int.TryParse(actionRaw, out int action))
                {
                    DrawError(string.Empty);
                    SetCursorForInput();

                    var coord = action - 1;
                    var row = coord / 3;
                    var col = coord % 3;

                    if (gameState[row, col] > 0)
                    {
                        DrawError("Клетка занята");
                        SetCursorForInput();
                    }
                    else
                    {
                        gameState[row, col] = currentPlayer;
                        InsertIntoLog($"◄ {(currentPlayer == 1 ? "X" : "O")} ► походил в клетку {action}");
                        currentPlayer = currentPlayer == 1 ? 2 : 1;
                    }
                }
                else
                {
                    DrawError("Требуется число от 0 до 9");
                    SetCursorForInput();
                }

                DrawLog();
            }

            InsertIntoLog($"----------------");

            if (winner > 0)
            {
                InsertIntoLog($"Победил ◄ {(winner == 1 ? "X" : "O")} ► !");
            }
            else
            {
                InsertIntoLog($"Ничья !");
            }

            DrawField();
            DrawHelp();
            DrawStatus();
            DrawLog();
            SetCursorForInput();

            Console.ReadKey();
        }

        private void InsertIntoLog(string text)
        {
            for (var i = 0; i < log.Length; i++)
            {
                if (string.IsNullOrEmpty(log[i]))
                {
                    log[i] = text;
                    break;
                }
            }
        }

        private bool GameIsEnded()
        {
            for (var i = 0; i < 3; i++)
            {
                if (gameState[i, 0] == gameState[i, 1] && gameState[i, 1] == gameState[i, 2] && gameState[i, 0] > 0)
                {
                    winner = gameState[i, 0];
                    return true;
                }

                if (gameState[0, i] == gameState[1, i] && gameState[1, i] == gameState[2, i] && gameState[0, i] > 0)
                {
                    winner = gameState[0, i];
                    return true;
                }
            }

            if (gameState[0, 0] == gameState[1, 1] && gameState[1, 1] == gameState[2, 2] && gameState[0, 0] > 0)
            {
                winner = gameState[0, 0];
                return true;
            }

            if (gameState[0, 2] == gameState[1, 1] && gameState[1, 1] == gameState[2, 0] && gameState[0, 2] > 0)
            {
                winner = gameState[0, 2];
                return true;
            }

            var allFilled = true;
            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (gameState[i, j] < 1)
                    {
                        allFilled = false;
                        break;
                    }
                }
            }

            return allFilled;
        }

        private void SetCursorForInput()
        {
            Console.SetCursorPosition(gameFieldOffetLeft + 20, 4);
            Console.Write(new string(' ', Console.BufferWidth - gameFieldOffetLeft - 1 - 34));
            Console.SetCursorPosition(gameFieldOffetLeft + 20, 4);
        }

        private void DrawLog()
        {
            Console.SetCursorPosition(gameFieldOffetLeft, 10);
            Console.Write("Лог действий:");

            Console.SetCursorPosition(gameFieldOffetLeft, 12);
            log.ToList().ForEach(x =>
            {
                Console.Write(x);
                Console.SetCursorPosition(gameFieldOffetLeft, ++Console.CursorTop);
            });
        }

        private void DrawError(string text)
        {
            var prevBg = Console.BackgroundColor;
            var prevFg = Console.ForegroundColor;

            Console.SetCursorPosition(gameFieldOffetLeft, 5);

            if (!string.IsNullOrEmpty(text))
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" {text} ");
            }
            else
            {
                Console.Write(new string(' ', Console.BufferWidth - gameFieldOffetLeft - 1));
            }

            Console.BackgroundColor = prevBg;
            Console.ForegroundColor = prevFg;
        }

        private void DrawField()
        {
            Console.SetCursorPosition(0, 0);

            gameField.ToList().ForEach(x =>
            {
                Console.WriteLine(x);
            });

            for (var i = 0; i < 3; i++)
            {
                for (var j = 0; j < 3; j++)
                {
                    if (gameState[i, j] > 0)
                    {
                        var symbol = gameState[i, j] == 1 ? xSign : oSign;
                        DrawSymbol(symbol, gameStateCoord[i, j]);
                    }
                }
            }
        }

        private void DrawSymbol(string[] symbol, Point point)
        {
            Console.SetCursorPosition(point.X, point.Y);

            symbol.ToList().ForEach(x =>
            {
                Console.Write(x);
                Console.SetCursorPosition(point.X, ++Console.CursorTop);
            });
        }

        private void DrawHelp()
        {
            Console.SetCursorPosition(gameFieldOffetLeft, 0);
            Console.Write("▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄▄");

            Console.SetCursorPosition(gameFieldOffetLeft, 2);
            Console.Write("Используйте цифры от 1 до 9 для ходов");
        }

        private void DrawStatus()
        {
            Console.SetCursorPosition(gameFieldOffetLeft, 4);
            Console.Write($"Ходит игрок ◄ {(currentPlayer == 1 ? "X" : "O")} ► : ");
        }
    }
}
