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
    ColoredText knightChar = new(@"⚔", ConsoleColor.DarkGray, ConsoleColor.DarkGray);
    public int knightX, knightY, oldKnightX, oldKnightY;
    ColoredText witchChar = new(@"⚔", ConsoleColor.Magenta, ConsoleColor.Magenta);
   public int witchX, witchY, oldWitchX, oldWitchY;
    ColoredText thiefChar = new(@"⚔", ConsoleColor.DarkRed, ConsoleColor.DarkRed);
   public int thiefX, thiefY, oldThiefX, oldThiefY;
    //Varaibles
    public bool knightMovedThisFrame = false;
    public bool witchMovedThisFrame = false;
   public bool thiefMovedThisFrame = false;

    //  Map 
    ColoredText[,] backgroundTiles;

    //  Enemies 
    (int x, int y, ColoredText sprite)[] townEnemies;
    (int x, int y, ColoredText sprite)[] forestEnemies;
    (int x, int y, ColoredText sprite)[] castleEnemies;
    public int enemyDamage = Random.Integer(1, 6);// Random generator from 1-5 to cause damage to the player
    public bool startGame = false;
    public int playerAbilites = 4;//number of ability points per round

    public void Setup()
    {

        Program.TerminalExecuteMode = TerminalExecuteMode.ExecuteTime;
        Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
        Program.TargetFPS = 60;
        Terminal.CursorVisible = false;

        Terminal.SetTitle("Tales from the Past");
        intro();
        RandomCards.cardMoves();

        int width = Console.WindowWidth / 2;
        int height = Console.WindowHeight;

        map = new TerminalGridWithColor(width, height, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
        backgroundTiles = new ColoredText[width, height];

        

        // Initialize players at bottom-left
        knightX = witchX = thiefX = 0;
        knightY = witchY = thiefY = height - 1;
        oldKnightX = knightX; oldKnightY = knightY;
        oldWitchX = witchX; oldWitchY = witchY;
        oldThiefX = thiefX; oldThiefY = thiefY;

        DrawCharacter(knightX, knightY, knightChar);
        DrawCharacter(witchX, witchY, witchChar);
        DrawCharacter(thiefX, thiefY, thiefChar);
        DrawAreas(width, height);
        SetupEnemies(width, height);
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
            if (Input.IsKeyPressed(ConsoleKey.M))
            {
                startGame = true;
                knightMovedThisFrame = witchMovedThisFrame = thiefMovedThisFrame = false;

                

                
                
                DrawEnemies();
            }

        }
        UpdateCharacter(ref oldKnightX, ref oldKnightY, knightX, knightY, knightChar);
        UpdateCharacter(ref oldWitchX, ref oldWitchY, witchX, witchY, witchChar);
        UpdateCharacter(ref oldThiefX, ref oldThiefY, thiefX, thiefY, thiefChar);
        Move(ref knightX, ref knightY, ref knightMovedThisFrame, ConsoleKey.UpArrow, ConsoleKey.DownArrow, ConsoleKey.LeftArrow, ConsoleKey.RightArrow);
        Move(ref witchX, ref witchY, ref witchMovedThisFrame, ConsoleKey.W, ConsoleKey.S, ConsoleKey.A, ConsoleKey.D);
        Move(ref thiefX, ref thiefY, ref thiefMovedThisFrame, ConsoleKey.I, ConsoleKey.K, ConsoleKey.J, ConsoleKey.L);
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
        if (Input.IsKeyPressed(ConsoleKey.H) && !RandomCards.inCombatMode)
        {
            RandomCards.inCombatMode = true;
        }//debug to enter combat mode
        if (RandomCards.enemyHealth <= 0) // once an enemy is killed it deactivates combat mode
        {
            RandomCards.inCombatMode = false;
            //Program.TerminalInputMode = TerminalInputMode.EnableInputDisableReadLine;
            Console.WriteLine("Enemy Defeated");
            playerAbilites = 4;
        }
        if (RandomCards.takenDamage)//whenever an enemy attacks you it runs this command
        {
            RandomCards.playerHealth -= enemyDamage;
            Console.WriteLine("The enemy has attacked you");
            Console.WriteLine($"You have {RandomCards.playerHealth} health left");
            RandomCards.takenDamage = false;
        }
    }
    // Player Movement
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
    public void startArea()
    {//first level
        // Set map to some values
      
        // Clear window and draw map
        map.ClearWrite();
    }
}
