using DefaultNamespace;
using System;
using System.Text;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        TerminalGridWithColor map;

        // Characters
        ColoredText knightChar = new(@"⚔", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
        int knightX, knightY, oldKnightX, oldKnightY;
        ColoredText witchChar = new(@"♛", ConsoleColor.Magenta, ConsoleColor.Magenta);
        int witchX, witchY, oldWitchX, oldWitchY;
        ColoredText thiefChar = new(@"☠", ConsoleColor.DarkRed, ConsoleColor.DarkRed);
        int thiefX, thiefY, oldThiefX, oldThiefY;

        bool knightMovedThisFrame, witchMovedThisFrame, thiefMovedThisFrame;

        // Map
        ColoredText[,] backgroundTiles;

        // Enemies
        (int x, int y, ColoredText sprite)[] townEnemies;
        (int x, int y, ColoredText sprite)[] forestEnemies;
        (int x, int y, ColoredText sprite)[] castleEnemies;

        // Text box
        StringBuilder textBoxInput = new();
        int mapHeight;
        int textBoxTop;
        bool isTypingMode = false; // ✅ typing mode toggle

        public void Setup()
        {
            Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
            Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
            Program.TargetFPS = 60;
            Terminal.SetTitle("Tales of the Past");
            Terminal.CursorVisible = false;

            int width = Console.WindowWidth / 2;
            mapHeight = (int)(Console.WindowHeight * 0.75); // 75% map
            textBoxTop = mapHeight;

            map = new TerminalGridWithColor(width, mapHeight, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
            backgroundTiles = new ColoredText[width, mapHeight];

            DrawAreas(width, mapHeight);
            SetupEnemies(width, mapHeight);
            DrawEnemies();

            // Initialize players at middle-left
            int middleY = mapHeight / 2;
            knightX = witchX = thiefX = 0;
            knightY = middleY;
            witchY = middleY - 1;
            thiefY = middleY + 1;

            oldKnightX = knightX; oldKnightY = knightY;
            oldWitchX = witchX; oldWitchY = witchY;
            oldThiefX = thiefX; oldThiefY = thiefY;

            DrawCharacter(knightX, knightY, knightChar);
            DrawCharacter(witchX, witchY, witchChar);
            DrawCharacter(thiefX, thiefY, thiefChar);

            DrawTextBox();
        }

        public void Execute()
        {
            // ✅ Toggle typing mode with T key
            if (Input.IsKeyPressed(ConsoleKey.T))
            {
                isTypingMode = !isTypingMode;
                Console.SetCursorPosition(0, textBoxTop + 1);
                Console.WriteLine(isTypingMode ? "[Typing mode enabled]" : "[Typing mode disabled]");
                DrawTextBox();
            }

            if (isTypingMode)
            {
                HandleTextBoxInput();
                return; // stop movement while typing
            }

            // Normal movement/gameplay
            knightMovedThisFrame = witchMovedThisFrame = thiefMovedThisFrame = false;

            Move(ref knightX, ref knightY, ref knightMovedThisFrame, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow, GetCurrentAreaEnemies(knightX));
            Move(ref witchX, ref witchY, ref witchMovedThisFrame, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D, GetCurrentAreaEnemies(witchX));
            Move(ref thiefX, ref thiefY, ref thiefMovedThisFrame, ConsoleKey.I, ConsoleKey.K, ConsoleKey.J, ConsoleKey.L, GetCurrentAreaEnemies(thiefX));

            UpdateCharacter(ref oldKnightX, ref oldKnightY, knightX, knightY, knightChar);
            UpdateCharacter(ref oldWitchX, ref oldWitchY, witchX, witchY, witchChar);
            UpdateCharacter(ref oldThiefX, ref oldThiefY, thiefX, thiefY, thiefChar);
        }

        //  MAP DRAWING 
        void DrawAreas(int width, int height)
        {
            int third = width / 3;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    ColoredText tile;

                    if (x < third) // Town
                    {
                        tile = new ColoredText("  ", ConsoleColor.Green, ConsoleColor.Green);
                        if ((x % 10 == 2 || x % 10 == 7) && y % 6 == 2) tile = new ColoredText("🏠", ConsoleColor.Gray, ConsoleColor.Green);
                        if (y % 6 == 3) tile = new ColoredText("~~", ConsoleColor.Blue, ConsoleColor.DarkBlue);
                        if (y % 6 == 3 && x % 10 == 5) tile = new ColoredText("==", ConsoleColor.Yellow, ConsoleColor.Green);
                    }
                    else if (x < 2 * third) // Forest
                    {
                        tile = new ColoredText("  ", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                        if ((x % 6 == 1 || x % 6 == 4) && (y % 5 == 1 || y % 5 == 3)) tile = new ColoredText("🌲", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                        if ((x % 7 == 3 || x % 7 == 5) && y % 6 == 2) tile = new ColoredText("🌸", ConsoleColor.Magenta, ConsoleColor.DarkGreen);
                    }
                    else // Castle
                    {
                        tile = new ColoredText("  ", ConsoleColor.Gray, ConsoleColor.DarkGray);
                        if (y == height - 6) tile = new ColoredText("==", ConsoleColor.Gray, ConsoleColor.DarkGray);
                        if ((x % 7 == 3) && y % 6 == 1 && y < height - 6) tile = new ColoredText("🪟", ConsoleColor.Cyan, ConsoleColor.DarkGray);
                        if ((x % 6 == 2 || x % 6 == 4) && y >= 0 && y <= height / 4) tile = new ColoredText("🔶", ConsoleColor.Yellow, ConsoleColor.DarkGray);
                        if (y > height - 5) tile = new ColoredText("~~", ConsoleColor.Red, ConsoleColor.DarkRed);
                    }

                    backgroundTiles[x, y] = tile;
                    map.Poke(x * 2, y, tile);
                }
            }
        }

        // CHARACTER CONTROL 
        void Move(ref int x, ref int y, ref bool moved, ConsoleKey up, ConsoleKey down, ConsoleKey left, ConsoleKey right, (int x, int y, ColoredText sprite)[] areaEnemies)
        {
            if (moved) return;
            if (IsNearEnemy(x, y, areaEnemies)) return;

            int newX = x, newY = y;
            if (Input.IsKeyPressed(right)) newX++;
            if (Input.IsKeyPressed(left)) newX--;
            if (Input.IsKeyPressed(up)) newY--;
            if (Input.IsKeyPressed(down)) newY++;

            if (!CanMoveTo(newX, newY)) return;

            if (newX != x || newY != y)
            {
                x = Math.Clamp(newX, 0, map.Width - 1);
                y = Math.Clamp(newY, 0, map.Height - 1);
                moved = true;
            }
        }

        void UpdateCharacter(ref int oldX, ref int oldY, int newX, int newY, ColoredText character)
        {
            if (oldX != newX || oldY != newY)
            {
                ResetCell(oldX, oldY);
                DrawCharacter(newX, newY, character);
                oldX = newX; oldY = newY;
            }
        }

        void DrawCharacter(int x, int y, ColoredText character)
        {
            var tile = backgroundTiles[x, y];
            character.bgColor = tile.bgColor;
            map.Poke(x * 2, y, character);
        }

        void ResetCell(int x, int y) => map.Poke(x * 2, y, backgroundTiles[x, y]);

        // ENEMIES 
        void SetupEnemies(int width, int height)
        {
            int third = width / 3;

            townEnemies = new (int, int, ColoredText)[7]
            {
                (1, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (third / 2, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (third - 2, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (2, height - 4, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (third / 3, height - 3, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (third - 3, height - 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (third / 2, height / 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green))
            };

            forestEnemies = new (int, int, ColoredText)[9]
            {
                (third + 2, 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (third + third / 3, 3, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * third - 2, 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (third + 1, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (third + third / 2, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * third - 2, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (third + 2, height - 4, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (third + third / 2, height - 3, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * third - 3, height - 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen))
            };

            castleEnemies = new (int, int, ColoredText)[1]
            {
                (2 * third + third / 2, height - 6, new ColoredText("😈", ConsoleColor.Red, ConsoleColor.DarkGray))
            };
        }

        void DrawEnemies()
        {
            foreach (var (x, y, sprite) in townEnemies) backgroundTiles[x, y] = sprite;
            foreach (var (x, y, sprite) in forestEnemies) backgroundTiles[x, y] = sprite;
            foreach (var (x, y, sprite) in castleEnemies) backgroundTiles[x, y] = sprite;

            for (int y = 0; y < map.Height; y++)
                for (int x = 0; x < map.Width; x++)
                    map.Poke(x * 2, y, backgroundTiles[x, y]);
        }

        //  ENEMY & BARRIERS 
        bool IsNearEnemy(int x, int y, (int x, int y, ColoredText sprite)[] enemies)
        {
            foreach (var (ex, ey, _) in enemies)
                if (Math.Abs(x - ex) <= 1 && Math.Abs(y - ey) <= 1) return true;
            return false;
        }

        bool AreEnemiesDefeated((int x, int y, ColoredText sprite)[] enemies) => enemies.Length == 0;

        bool CanMoveTo(int x, int y)
        {
            int third = map.Width / 3;

            if (x == third && !AreEnemiesDefeated(townEnemies)) return false;
            if ((x == third - 1 && !AreEnemiesDefeated(townEnemies)) || (x == 2 * third && !AreEnemiesDefeated(forestEnemies))) return false;
            if (x == 2 * third - 1 && !AreEnemiesDefeated(forestEnemies)) return false;

            return true;
        }

        (int x, int y, ColoredText sprite)[] GetCurrentAreaEnemies(int x)
        {
            int third = map.Width / 3;
            if (x < third) return townEnemies;
            if (x < 2 * third) return forestEnemies;
            return castleEnemies;
        }

        // TEXT BOX 
        void DrawTextBox()
        {
            Console.SetCursorPosition(0, textBoxTop);
            Console.ForegroundColor = ConsoleColor.White;
            Console.BackgroundColor = ConsoleColor.Black;

            int width = Console.WindowWidth;
            int height = Console.WindowHeight - textBoxTop;

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(0, textBoxTop + i);
                Console.Write(new string(' ', width));
            }

            Console.SetCursorPosition(0, textBoxTop);
            Console.WriteLine(new string('═', width));
            Console.Write(" > ");
            Console.ResetColor();
        }

        void HandleTextBoxInput()
        {
            while (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                if (key.Key == ConsoleKey.Enter)
                {
                    string text = textBoxInput.ToString();
                    textBoxInput.Clear();

                    Console.SetCursorPosition(0, textBoxTop + 1);
                    Console.WriteLine("You entered: " + text.PadRight(Console.WindowWidth - 12));
                    Console.SetCursorPosition(3, textBoxTop);
                    Console.Write(new string(' ', Console.WindowWidth - 3));
                    Console.SetCursorPosition(3, textBoxTop);
                }
                else if (key.Key == ConsoleKey.Backspace && textBoxInput.Length > 0)
                {
                    textBoxInput.Remove(textBoxInput.Length - 1, 1);
                    Console.SetCursorPosition(3 + textBoxInput.Length, textBoxTop);
                    Console.Write(' ');
                    Console.SetCursorPosition(3 + textBoxInput.Length, textBoxTop);
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    textBoxInput.Append(key.KeyChar);
                    Console.SetCursorPosition(3 + textBoxInput.Length - 1, textBoxTop);
                    Console.Write(key.KeyChar);
                }
            }
        }
    }
}

// STRONGLY RECOMMENDED: press movement keys once, not hold them, to avoid input bugs.
