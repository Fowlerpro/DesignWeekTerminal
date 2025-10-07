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

            // Dynamically size the map to fill the terminal
            int gridWidth = Console.WindowWidth / 2; // each tile is 2 columns wide
            int gridHeight = Console.WindowHeight;

            // Initialize map with base tiles (green field)
            map = new TerminalGridWithColor(gridWidth, gridHeight, new ColoredText("  ", ConsoleColor.DarkBlue, ConsoleColor.DarkBlue));

            // Draw the map to terminal
            map.ClearWrite();

            // Draw initial characters
            DrawCharacter(knightX, knightY, knightChar);
            DrawCharacter(witchX, witchY, witchChar);
            DrawCharacter(thiefX, thiefY, thiefChar);
        }






