using System;
using System.Data;
using System.Diagnostics;
using System.Text;

namespace MohawkTerminalGame;

public class TerminalGame
{
    // Place your variables here


    /// Run once before Execute begins
    TerminalGridWithColor map;

    // Characters
    ColoredText playerChar = new(@"⚔", ConsoleColor.Magenta, ConsoleColor.Magenta);
   public int playerX, playerY, oldplayerX, oldplayerY;
    //Varaibles
    public bool playerMovedThisFrame = false;

    //  Map 
    ColoredText[,] backgroundTiles;


    // Text box
    public int textBoxTop;

    //  Enemies 
    (int x, int y, ColoredText sprite)[] townEnemies;
    (int x, int y, ColoredText sprite)[] forestEnemies;
    (int x, int y, ColoredText sprite)[] castleEnemies;
    public int enemyDamage = Random.Integer(2, 8);// Random generator from 2-7 to cause damage to the player
    public int majorEnemyDamage = Random.Integer(5, 20);// Random generator from 5-19 to cause damage to the player
    public int bossEnemyDamage = Random.Integer(10, 26);//Random generator from 10-25 to cause damage to the player
    public bool startGame = false;

    string command = "";// empty string variable to be used in the command function
    public int width = Console.WindowWidth / 2;
    public int height = Console.WindowHeight - 8;

    public void Setup()
    {

        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;
        Program.TargetFPS = 60;
        Terminal.CursorVisible = false;
        Terminal.SetCursorPosition(0, 5);

        Terminal.SetTitle("Tales from the Past");
        intro();

        

        map = new TerminalGridWithColor(width, height, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
        backgroundTiles = new ColoredText[width, height];

        

        // Initialize players at bottom-left
        playerX = 0;
        playerY = height - 1;
        SetupEnemies(width, height);
        oldplayerX = playerX; oldplayerY = playerY;
        DrawAreas(width, height);
        DrawCharacter(playerX, playerY, playerChar);
        DrawEnemies();
        textBoxTop = height + 1;
        DrawTextBox();
        Terminal.SetCursorPosition(3, 23);
        Terminal.WriteLine("To Move type W,A,S,D then press ENTER");
        
    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {//Starts the game if player hits the spacebar
        playerMovedThisFrame = false;
        UpdateCharacter(ref oldplayerX, ref oldplayerY, playerX, playerY, playerChar);


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
                //Terminal.Clear();
                //DrawAreas(width, height);
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

            if (RandomCards.playerHealth >= 101)
            {
                RandomCards.playerHealth = 100;
                Console.WriteLine("Health is at Max");
            }
            if (RandomCards.playerMana >= 101)
            {
                RandomCards.playerHealth = 100;
                Console.WriteLine("Mana is at Max");
            }
            if (RandomCards.enemyHealth <= 0) // once an enemy is killed it deactivates combat mode
            {
                RandomCards.inCombatMode = false;
                Console.WriteLine("Enemy Defeated");
                RandomCards.playerMana += 25;
            }
            if (RandomCards.takenDamage)//whenever an enemy attacks you it runs this command
            {
                RandomCards.playerHealth -= enemyDamage;
                Console.WriteLine($"The enemy has attacked you,You have {RandomCards.playerHealth} health left");
                RandomCards.takenDamage = false;
            }
            if (RandomCards.inCombatMode)
            {
                Console.WriteLine($"you have {RandomCards.playerMana} mana left");
                Console.WriteLine("Enter a code from a card!");

            }
        }
    }
    public void typedCommands(string command)
    {
        Terminal.SetCursorPosition(3, 23);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 24);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 25);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 26);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 27);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 28);
        Terminal.ClearLine();
        Terminal.SetCursorPosition(3, 23);

        switch (command)
        {
            case "w":
                playerY = Math.Max(0, playerY - 1);
                Terminal.WriteLine("Player moved up");
                break;
            case "s":
                Terminal.WriteLine("Player moved down");
                if(playerY < height -1)
                {
                    Terminal.WriteLine("Player moved down");
                    playerY++;
                }
                break;
            case "a":
                playerX = Math.Max(0, playerX - 1);
                Terminal.WriteLine("Player moved left");
                break;
            case "d":
                playerX = Math.Max(0, playerX + 1);
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
                    RandomCards.enemyHealth = RandomCards.locationHealth;
                    Terminal.WriteLine("Player has entered combat mode");
                    RandomCards.inCombatMode = true;
                }//command to enter combat
                break;
            case "damage":
                {
                    RandomCards.playerHealth -= enemyDamage;
                    Console.WriteLine($"The enemy has attacked you,You have {RandomCards.playerHealth} health left");
                    RandomCards.takenDamage = false;//command to test the enemy damage function
                }
                break;
            default:
                Terminal.WriteLine("Invalid Command");
                Terminal.Beep();
                break;

        }
        var currentEnemies = GetCurrentAreaEnemies(playerX);
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
        Terminal.WriteLine("Type enter to start playing!");
    }
    void DrawTextBox()
    {

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        int width = Console.WindowWidth;
        int height = Console.WindowHeight - textBoxTop;

        for (int i = 0; i < height; i++)
        {
            Console.SetCursorPosition(0, textBoxTop + i);
            Console.Write(new string(' ', width), ConsoleColor.White, ConsoleColor.Black);
        }

        Console.SetCursorPosition(0, textBoxTop - 1);
        Console.WriteLine(new string('═', width));
        Console.Write(" > ");
        Console.ResetColor();
    }
    // 
    // Enemy & Barrier Logic
    // 
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
}
