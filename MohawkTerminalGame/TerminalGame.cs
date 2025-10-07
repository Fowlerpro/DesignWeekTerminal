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

    }
}
