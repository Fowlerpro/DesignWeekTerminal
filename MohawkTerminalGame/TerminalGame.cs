using DefaultNamespace;
using System;

namespace MohawkTerminalGame
{
    public class TerminalGame
    {
        TerminalGridWithColor map;

        // ---------------- Characters ----------------
        ColoredText knightChar = new(@"⚔", ConsoleColor.White, ConsoleColor.Black);
        int knightX = 0, knightY = 0;
        int oldKnightX = 0, oldKnightY = 0;

        ColoredText witchChar = new(@"⚔", ConsoleColor.Magenta, ConsoleColor.Black);
        int witchX = 2, witchY = 0;
        int oldWitchX = 2, oldWitchY = 0;

        ColoredText thiefChar = new(@"⚔", ConsoleColor.DarkGray, ConsoleColor.Black);
        int thiefX = 4, thiefY = 0;
        int oldThiefX = 4, oldThiefY = 0;

    

        // ---------------- Setup ----------------
        public void Setup()
        {
            Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
            Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
            Program.TargetFPS = 60;

            Terminal.SetTitle("Tales of the Past");
            Terminal.CursorVisible = false;
            Terminal.CursorVisible = true;
            // Dynamically size the map to fill the terminal
            int gridWidth = Console.WindowWidth / 2; // each tile is 2 columns wide
            int gridHeight = Console.WindowHeight;

            // Initialize map with base tiles (green field)
            map = new TerminalGridWithColor(gridWidth, gridHeight, new ColoredText("  ", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen));

            // Draw the map to terminal
            map.ClearWrite();

            // Draw initial characters
            DrawCharacter(knightX, knightY, knightChar);
            DrawCharacter(witchX, witchY, witchChar);
            DrawCharacter(thiefX, thiefY, thiefChar);
        }
        // ---------------- Main Loop ----------------
        public void Execute()
        {
            // Move characters based on input
            MoveKnight();
            MoveWitch();
            MoveThief();

            // Update characters on the map if they moved
            UpdateCharacter(ref oldKnightX, ref oldKnightY, knightX, knightY, knightChar);
            UpdateCharacter(ref oldWitchX, ref oldWitchY, witchX, witchY, witchChar);
            UpdateCharacter(ref oldThiefX, ref oldThiefY, thiefX, thiefY, thiefChar);
        }
        // ---------------- Movement ----------------
        void MoveKnight()
        {
            oldKnightX = knightX;
            oldKnightY = knightY;

            if (Input.IsKeyPressed(ConsoleKey.RightArrow)) knightX++;
            if (Input.IsKeyPressed(ConsoleKey.LeftArrow)) knightX--;
            if (Input.IsKeyPressed(ConsoleKey.UpArrow)) knightY--;
            if (Input.IsKeyPressed(ConsoleKey.DownArrow)) knightY++;

            knightX = Math.Clamp(knightX, 0, map.Width - 1);
            knightY = Math.Clamp(knightY, 0, map.Height - 1);
        }

        void MoveWitch()
        {
            oldWitchX = witchX;
            oldWitchY = witchY;

            if (Input.IsKeyPressed(ConsoleKey.D)) witchX++;
            if (Input.IsKeyPressed(ConsoleKey.A)) witchX--;
            if (Input.IsKeyPressed(ConsoleKey.W)) witchY--;
            if (Input.IsKeyPressed(ConsoleKey.S)) witchY++;

            witchX = Math.Clamp(witchX, 0, map.Width - 1);
            witchY = Math.Clamp(witchY, 0, map.Height - 1);
        }

        void MoveThief()
        {
            oldThiefX = thiefX;
            oldThiefY = thiefY;

            if (Input.IsKeyPressed(ConsoleKey.L)) thiefX++;
            if (Input.IsKeyPressed(ConsoleKey.J)) thiefX--;
            if (Input.IsKeyPressed(ConsoleKey.I)) thiefY--;
            if (Input.IsKeyPressed(ConsoleKey.K)) thiefY++;

            thiefX = Math.Clamp(thiefX, 0, map.Width - 1);
            thiefY = Math.Clamp(thiefY, 0, map.Height - 1);
        }

        // ---------------- Drawing ----------------
        void UpdateCharacter(ref int oldX, ref int oldY, int newX, int newY, ColoredText character)
        {
            if (oldX != newX || oldY != newY)
            {
                ResetCell(oldX, oldY);
                DrawCharacter(newX, newY, character);

                oldX = newX;
                oldY = newY;
            }
        }

        void DrawCharacter(int x, int y, ColoredText character)
        {
            ColoredText tile = map.Get(x, y);
            character.bgColor = tile.bgColor; // preserve background color
            map.Poke(x * 2, y, character);
        }

        void ResetCell(int x, int y)
        {
            ColoredText tile = map.Get(x, y);
            map.Poke(x * 2, y, tile); // restore base tile
        }
    }
}






