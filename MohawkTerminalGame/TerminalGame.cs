using System;
using System.Data;

namespace MohawkTerminalGame;

public class TerminalGame
{
    // Place your variables here


    /// Run once before Execute begins
    //Varaibles
    public bool startGame = false;
    public void Setup()
    {

        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 60;

        Terminal.SetTitle("Tales from the Past");
        intro();

    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {
        if (!startGame)
        {
            if (Input.IsKeyPressed(ConsoleKey.Spacebar))
            {
                startGame = true;
                startArea();
                Console.WriteLine(startGame);
            }
        }
        if (Input.IsKeyPressed(ConsoleKey.RightArrow))
            Console.WriteLine(startGame);
    }
    public void Update()
    {
        if (Input.IsKeyPressed(ConsoleKey.RightArrow))
            Console.WriteLine(startGame);
    }
    public void intro()
    {
        ColoredText empty = new(@",", ConsoleColor.Green, ConsoleColor.DarkGreen);
        ColoredText borderL = new(@"||", ConsoleColor.White, ConsoleColor.DarkGreen);
        ColoredText borderR = new(@"||", ConsoleColor.White, ConsoleColor.DarkGreen);
        ColoredText borderB = new(@"==", ConsoleColor.White, ConsoleColor.DarkGreen);
        ColoredText borderT = new(@"==", ConsoleColor.White, ConsoleColor.DarkGreen);
        TerminalGridWithColor map;
        map = new(50, 50, empty);
        map.SetCol(borderL, 0);
        map.SetCol(borderR, 49);
        map.SetRow(borderB, 1);
        map.SetRow(borderT, 49);
        map.ClearWrite();
    }
    public void startArea()
    {
        // Set map to some values
        ColoredText tree = new(@"/\", ConsoleColor.Green, ConsoleColor.DarkGreen);
        ColoredText riverNS = new(@"||", ConsoleColor.Blue, ConsoleColor.DarkBlue);
        ColoredText riverEW = new(@"==", ConsoleColor.Blue, ConsoleColor.DarkBlue);
        ColoredText player = new(@"😎", ConsoleColor.White, ConsoleColor.Black);
        TerminalGridWithColor map;
        map = new(10, 10, tree);
        map.SetCol(riverNS, 3);
        map.SetRow(riverEW, 8);

        // Clear window and draw map
        map.ClearWrite();
    }
}
