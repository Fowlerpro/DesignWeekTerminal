using DefaultNamespace;
using System;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        TerminalGridWithColor map;

        // Characters
        ColoredText knightChar = new(@"⚔", ConsoleColor.White, ConsoleColor.Black);
        int knightX, knightY, oldKnightX, oldKnightY;
        ColoredText witchChar = new(@"⚔", ConsoleColor.Magenta, ConsoleColor.Black);
        int witchX, witchY, oldWitchX, oldWitchY;
        ColoredText thiefChar = new(@"⚔", ConsoleColor.DarkGray, ConsoleColor.Black);
        int thiefX, thiefY, oldThiefX, oldThiefY;

        // Movement flags
        bool knightMovedThisFrame = false, witchMovedThisFrame = false, thiefMovedThisFrame = false;

        ColoredText[,] backgroundTiles;

        public void Setup()
        {
            Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
            Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
            Program.TargetFPS = 60;
            Terminal.SetTitle("Tales of the Past");
            Terminal.CursorVisible = false;

            int width = Console.WindowWidth / 2;
            int height = Console.WindowHeight;
            map = new TerminalGridWithColor(width, height, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
            backgroundTiles = new ColoredText[width, height];

            DrawAreas(width, height);

            // Start players at bottom-left
            knightX = witchX = thiefX = 0;
            knightY = witchY = thiefY = height - 1;
            oldKnightX = knightX; oldKnightY = knightY;
            oldWitchX = witchX; oldWitchY = witchY;
            oldThiefX = thiefX; oldThiefY = thiefY;

            DrawCharacter(knightX, knightY, knightChar);
            DrawCharacter(witchX, witchY, witchChar);
            DrawCharacter(thiefX, thiefY, thiefChar);
        }

        public void Execute()
        {
            knightMovedThisFrame = witchMovedThisFrame = thiefMovedThisFrame = false;
            Move(ref knightX, ref knightY, ref knightMovedThisFrame, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow);
            Move(ref witchX, ref witchY, ref witchMovedThisFrame, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
            Move(ref thiefX, ref thiefY, ref thiefMovedThisFrame, ConsoleKey.I, ConsoleKey.K, ConsoleKey.J, ConsoleKey.L);

            UpdateCharacter(ref oldKnightX, ref oldKnightY, knightX, knightY, knightChar);
            UpdateCharacter(ref oldWitchX, ref oldWitchY, witchX, witchY, witchChar);
            UpdateCharacter(ref oldThiefX, ref oldThiefY, thiefX, thiefY, thiefChar);
        }

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
                        if ((x % 10 == 2 || x % 10 == 7) && (y % 6 == 2))
                            tile = new ColoredText("🏠", ConsoleColor.Gray, ConsoleColor.Green);
                        if (y % 6 == 3)
                            tile = new ColoredText("~~", ConsoleColor.Blue, ConsoleColor.DarkBlue);
                        if (y % 6 == 3 && x % 10 == 5)
                            tile = new ColoredText("==", ConsoleColor.Yellow, ConsoleColor.Green);
                    }
                    else if (x < 2 * third) // Forest
                    {
                        tile = new ColoredText("  ", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                        if ((x % 6 == 1 || x % 6 == 4) && (y % 5 == 1 || y % 5 == 3))
                            tile = new ColoredText("🌲", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                        if ((x % 7 == 3 || x % 7 == 5) && y % 6 == 2)
                            tile = new ColoredText("🌸", ConsoleColor.Magenta, ConsoleColor.DarkGreen);
                    }
                    else // Castle
                    {
                        tile = new ColoredText("  ", ConsoleColor.Gray, ConsoleColor.DarkGray);
                        if (y == height - 6)
                            tile = new ColoredText("==", ConsoleColor.Gray, ConsoleColor.DarkGray);
                        if ((x % 7 == 3) && y % 6 == 1 && y < height - 6)
                            tile = new ColoredText("🪟", ConsoleColor.Cyan, ConsoleColor.DarkGray);
                        if ((x % 6 == 2 || x % 6 == 4) && y >= 0 && y <= height / 4)
                            tile = new ColoredText("🔶", ConsoleColor.Yellow, ConsoleColor.DarkGray);
                        if (y > height - 5)
                            tile = new ColoredText("~~", ConsoleColor.Red, ConsoleColor.DarkRed);
                    }

                    backgroundTiles[x, y] = tile;
                    map.Poke(x * 2, y, tile);
                }
            }
        }

        void Move(ref int x, ref int y, ref bool moved, ConsoleKey up, ConsoleKey down, ConsoleKey left, ConsoleKey right)
        {
            if (moved) return;
            int newX = x, newY = y;
            if (Input.IsKeyPressed(right)) newX++;
            if (Input.IsKeyPressed(left)) newX--;
            if (Input.IsKeyPressed(up)) newY--;
            if (Input.IsKeyPressed(down)) newY++;
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
            ColoredText tile = backgroundTiles[x, y];
            character.bgColor = tile.bgColor;
            map.Poke(x * 2, y, character);
        }

        void ResetCell(int x, int y)
        {
            map.Poke(x * 2, y, backgroundTiles[x, y]);
        }
    }
}
