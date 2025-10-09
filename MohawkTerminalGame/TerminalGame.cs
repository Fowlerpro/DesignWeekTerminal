using System;
using System.Data;
using System.Diagnostics;

namespace MohawkTerminalGame;

public class TerminalGame
{
    // Place your variables here


    /// Run once before Execute begins
    TerminalGridWithColor map;

    // Characters
    ColoredText witchChar = new(@"⚔", ConsoleColor.Magenta, ConsoleColor.Magenta);
   public int witchX, witchY, oldWitchX, oldWitchY;
    //Varaibles
    public bool playerMovedThisFrame = false;

    //  Map 
    ColoredText[,] backgroundTiles;

    //  Enemies 
    (int x, int y, ColoredText sprite)[] townEnemies;
    (int x, int y, ColoredText sprite)[] forestEnemies;
    (int x, int y, ColoredText sprite)[] castleEnemies;
    public int enemyDamage = Random.Integer(1, 6);// Random generator from 1-5 to cause damage to the player
    public bool startGame = false;
    public int playerAbilites = 4;//number of ability points per round
    string command = "";// empty string variable to be used in the command function
    public int width = Console.WindowWidth / 2;
    public int height = Console.WindowHeight;
    public void Setup()
    {

        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;
        Program.TargetFPS = 60;
        Terminal.CursorVisible = false;
        Terminal.SetCursorPosition(0, 0);

        Terminal.SetTitle("Tales from the Past");
        intro();

        

        map = new TerminalGridWithColor(width, height, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
        backgroundTiles = new ColoredText[width, height];

        

        // Initialize players at bottom-left
        witchX = 0;
        witchY = height - 1;
        
        oldWitchX = witchX; oldWitchY = witchY;
        DrawAreas(width, height);
        SetupEnemies(width, height);
        Terminal.WriteLine("To Move type W,A,S,D then press ENTER");
    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {//Starts the game if player hits the spacebar
        DrawEnemies();
        Reload();
        DrawCharacter(witchX, witchY, witchChar);


        playerMovedThisFrame = false;
        UpdateCharacter(ref oldWitchX, ref oldWitchY, witchX, witchY, witchChar);


        string input = Terminal.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {//?.Trim gets rid of extra spaces and .ToLower makes all inputs automatically lowercase
            if (!RandomCards.inCombatMode)
            {
                command = input.Trim().ToLower();
                typedCommands(command);
            }
            else
            {
                RandomCards.cardCommand = input.Trim().ToLower();
                RandomCards.cardMoves(RandomCards.cardCommand);
            }
        }
        if (!RandomCards.hasGoldenIdol)
        {
            if (RandomCards.playerHealth <= 0)
            {
                RandomCards.inCombatMode = false;
                intro();
                Setup();
                startGame = false;
                RandomCards.playerHealth = 40;
            }
            else
            {
                if (RandomCards.playerHealth <= 0)
                {
                    RandomCards.playerHealth = 40;
                    RandomCards.hasGoldenIdol = false;
                }
            }// if player health falls to zero the title screen is displayed and player health is reset

            if (RandomCards.playerHealth >= 41)
            {
                RandomCards.playerHealth = 40;
                Console.WriteLine("Health is at Max");
            }
            if (RandomCards.enemyHealth <= 0) // once an enemy is killed it deactivates combat mode
            {
                RandomCards.inCombatMode = false;
                Console.WriteLine("Enemy Defeated");
                playerAbilites = 4;
            }
            if (RandomCards.takenDamage)//whenever an enemy attacks you it runs this command
            {
                RandomCards.playerHealth -= enemyDamage;
                Console.WriteLine($"The enemy has attacked you,You have {RandomCards.playerHealth} health left");
                RandomCards.takenDamage = false;
            }
            if (RandomCards.inCombatMode)
            {
                Console.WriteLine("Enter a code from a card!");

            }
        }
    }
    public void typedCommands(string command)
    {
        Terminal.Clear();
        DrawAreas(width, height);
        switch (command)
        {
            case "w":
                witchY = Math.Max(0, witchY - 1);
                Terminal.WriteLine("Player moved up");
                break;
            case "s":
                witchY = Math.Max(0, witchY + 1);
                Terminal.WriteLine("Player moved down");
                break;
            case "a":
                witchX = Math.Max(0, witchX - 1);
                Terminal.WriteLine("Player moved left");
                break;
            case "d":
                witchX = Math.Max(0, witchX +1);
                Terminal.WriteLine("Player moved right");
                break;
            case "kill":
                Terminal.WriteLine("Player was killed via command");
                RandomCards.playerHealth -= 40;
                Terminal.WriteLine($"player health is currently {RandomCards.playerHealth}");
                break;//debug command to kill the player to test reset mechanic
            case "heal":
                Terminal.WriteLine("Player was healed via command");
                RandomCards.playerHealth += 40;
                Terminal.WriteLine($"player health is currently {RandomCards.playerHealth}");
                break;//debug command to heal the player
            case "health":
                Terminal.WriteLine("checking player health");
                Terminal.WriteLine($"player health is currently {RandomCards.playerHealth}");
                break;//debug command to see the health of the player
            case "help":
                Terminal.WriteLine("Commands: w, a, s, d, heal, kill, health, help");
                break;//debug command to see all commands
            case "combat":
                if (!RandomCards.inCombatMode)
                {
                    Terminal.WriteLine("Player has entered combat mode");
                    RandomCards.inCombatMode = true;
                }//command to enter combat
                break;
                default:
                Terminal.WriteLine("Invalid Command");
                Terminal.Beep();
                break;

        }
    }
    public void Reload()
    {
        if (RandomCards.mapClear)
        {
            
            Terminal.Clear();
            DrawAreas(width, height);
        }
        RandomCards.mapClear = false;
    }
    // Character Drawing 
    public void UpdateCharacter(ref int oldX, ref int oldY, int newX, int newY, ColoredText character)
    {
        if (oldX != newX || oldY != newY)
        {
            ResetCell(oldX, oldY);
            DrawCharacter(newX, newY, character);
            oldX = newX; oldY = newY;
        }
    }

    public void DrawCharacter(int x, int y, ColoredText character)
    {
        ColoredText tile = backgroundTiles[x, y];
        character.bgColor = tile.bgColor;
        map.Poke(x * 2, y, character);
    }

    void ResetCell(int x, int y)
    {
        map.Poke(x * 2, y, backgroundTiles[x, y]);
    }

    //  Enemies Setup 
    public void SetupEnemies(int width, int height)
    {
        int townWidth = width / 3;
        int forestWidth = width / 3;
        int castleWidth = width / 3;

        // Town 😠
        townEnemies = new (int, int, ColoredText)[7]
        {
                (1, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (townWidth / 2, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (townWidth - 2, 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (2, height - 4, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (townWidth / 3, height - 3, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (townWidth - 3, height - 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green)),
                (townWidth / 2, height / 2, new ColoredText("😠", ConsoleColor.Red, ConsoleColor.Green))
        };

        // Forest 😡
        forestEnemies = new (int, int, ColoredText)[9]
        {
                (townWidth + 2, 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (townWidth + townWidth / 3, 3, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * townWidth - 2, 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (townWidth + 1, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (townWidth + townWidth / 2, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * townWidth - 2, height / 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (townWidth + 2, height - 4, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (townWidth + townWidth / 2, height - 3, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen)),
                (2 * townWidth - 3, height - 2, new ColoredText("😡", ConsoleColor.Red, ConsoleColor.DarkGreen))
        };

        // Castle 😈 (Boss)
        int castleStart = 2 * townWidth;
        castleEnemies = new (int, int, ColoredText)[1]
        {
                (castleStart + castleWidth / 2, height - 6, new ColoredText("😈", ConsoleColor.Red, ConsoleColor.DarkGray))
        };
    }
    // Map Drawing 
    public void DrawAreas(int width, int height)
    {
        int third = width / 3;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                ColoredText tile = null;

                // Town
                if (x < third)
                {
                    tile = new ColoredText("  ", ConsoleColor.Green, ConsoleColor.Green);
                    if ((x % 10 == 2 || x % 10 == 7) && (y % 6 == 2)) tile = new ColoredText("🏠", ConsoleColor.Gray, ConsoleColor.Green);
                    if (y % 6 == 3) tile = new ColoredText("~~", ConsoleColor.Blue, ConsoleColor.DarkBlue);
                    if (y % 6 == 3 && x % 10 == 5) tile = new ColoredText("==", ConsoleColor.Yellow, ConsoleColor.Green);
                }
                // Forest
                else if (x < 2 * third)
                {
                    tile = new ColoredText("  ", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                    if ((x % 6 == 1 || x % 6 == 4) && (y % 5 == 1 || y % 5 == 3)) tile = new ColoredText("🌲", ConsoleColor.DarkGreen, ConsoleColor.DarkGreen);
                    if ((x % 7 == 3 || x % 7 == 5) && y % 6 == 2) tile = new ColoredText("🌸", ConsoleColor.Magenta, ConsoleColor.DarkGreen);
                }
                // Castle
                else
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


   public void DrawEnemies()
    {
        foreach (var (x, y, sprite) in townEnemies) { backgroundTiles[x, y] = sprite; map.Poke(x * 2, y, sprite); }
        foreach (var (x, y, sprite) in forestEnemies) { backgroundTiles[x, y] = sprite; map.Poke(x * 2, y, sprite); }
        foreach (var (x, y, sprite) in castleEnemies) { backgroundTiles[x, y] = sprite; map.Poke(x * 2, y, sprite); }
    }
    public void intro()
    {//Starting title screen

    }
}
