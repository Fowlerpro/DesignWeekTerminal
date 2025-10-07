using System;
using System.Data;

namespace MohawkTerminalGame;

public class TerminalGame
{
    // Place your variables here


    /// Run once before Execute begins
    //Varaibles
    public int enemyDamage = Random.Integer(1, 6);// Random generator from 1-5 to cause damage to the player
    public bool startGame = false;
    public bool inCombatMode = false;
    public int playerAbilites = 4;//number of ability points per round
    
    public void Setup()
    {

        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 60;

        Terminal.SetTitle("Tales from the Past");
        intro();
        RandomCards.cardMoves();

    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {//Starts the game if player hits the spacebar
        if (!startGame)
        {
            if (Input.IsKeyPressed(ConsoleKey.Spacebar))
            {
                startGame = true;
                startArea();
            }
        }
        if (RandomCards.playerHealth <= 0)
        {
            intro();
            startGame = false;
            RandomCards.playerHealth = 40;
        }// if player health falls to zero the title screen is displayed and player health is reset
        if (Input.IsKeyPressed(ConsoleKey.G))
        {
            RandomCards.playerHealth -= 40;
        }//debug Keybing to kill the player to test reset mechanic
        if (Input.IsKeyPressed(ConsoleKey.H) && !inCombatMode)
        {
            inCombatMode = true;
            inCombat();
        }//debug to enter combat mode
        if (RandomCards.enemyHealth <= 0) // once an enemy is killed it deactivates combat mode
        {
            inCombatMode = false;
            Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
            Console.WriteLine("Enemy Defeated");
        }
        if (RandomCards.takenDamage)//whenever an enemy attacks you it runs this command
        {
            RandomCards.playerHealth -= enemyDamage;
            Console.WriteLine("The enemy has attacked you");
            Console.WriteLine($"You have {RandomCards.playerHealth} health left");
            RandomCards.takenDamage = false;
        }
    }
    public void intro()
    {//Starting title screen
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
    {//first level
        // Set map to some values
        ColoredText tree = new(@"/\", ConsoleColor.Green, ConsoleColor.DarkGreen);
        ColoredText riverNS = new(@"||", ConsoleColor.Blue, ConsoleColor.DarkBlue);
        ColoredText riverEW = new(@"==", ConsoleColor.Blue, ConsoleColor.DarkBlue);
        TerminalGridWithColor map;
        map = new(10, 10, tree);
        map.SetCol(riverNS, 3);
        map.SetRow(riverEW, 8);

        // Clear window and draw map
        map.ClearWrite();
    }
    public void inCombat()//combat mode, swaps to typing so the player can input card codes
    {
        playerAbilites = 4;
        RandomCards.enemyHealth = 20;
        //resets default values and changes the mod to type
        Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;
        string input = Terminal.ReadLine();
        

    }
}
