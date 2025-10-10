using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
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
    ColoredText[,] terrainTiles;


    // Text box
    public int textBoxTop;

    //  Enemies 
    (int x, int y, ColoredText sprite)[] townEnemies;
    (int x, int y, ColoredText sprite)[] forestEnemies;
    (int x, int y, ColoredText sprite)[] castleEnemies;
    public int locationEnemyDamage = 1;
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
        SetupEnemies(width, height);
        Terminal.SetTitle("Tales from the Past");

        intro();

        // Wait for ENTER


        string input = Terminal.ReadLine(); 
        Terminal.Clear();
        startGame = true;
        map = new TerminalGridWithColor(width, height, new ColoredText("  ", ConsoleColor.Black, ConsoleColor.Black));
        backgroundTiles = new ColoredText[width, height];

        Terminal.SetCursorPosition(3, 23);
        textBoxTop = height + 1;

        playerX = 0;
        playerY = height - 1;

        oldplayerX = playerX; oldplayerY = playerY;
        DrawAreas(width, height);
        DrawEnemies();
        DrawTextBox();
        Terminal.SetCursorPosition(3, textBoxTop);
        Terminal.WriteLine("To Move type W,A,S,D then press ENTER");
        Terminal.SetCursorPosition(3, textBoxTop + 5);
        Terminal.ClearLine();
        Terminal.WriteLine("Kill Enough Enemies to Progress, 5 for the Town, 7 for the Forest, The defeat The Boss");
        DrawCharacter(playerX, playerY, playerChar);
        Terminal.SetCursorPosition(3, textBoxTop);

    }

    // Execute() runs based on Program.TerminalExecuteMode (assign to it in Setup).
    //  ExecuteOnce: runs only once. Once Execute() is done, program closes.
    //  ExecuteLoop: runs in infinite loop. Next iteration starts at the top of Execute().
    //  ExecuteTime: runs at timed intervals (eg. "FPS"). Code tries to run at Program.TargetFPS.
    //               Code must finish within the alloted time frame for this to work well.
    public void Execute()
    {
        // Update player on the map
        UpdateCharacter(ref oldplayerX, ref oldplayerY, playerX, playerY, playerChar);

        // Read player input
        string input = Terminal.ReadLine();

        if (!string.IsNullOrEmpty(input))
        {
            string trimmedInput = input.Trim().ToLower();

            if (!RandomCards.inCombatMode)
            {
                // Normal movement/commands
                command = trimmedInput;
                typedCommands(command);
            }
            else
            {
                // Combat input
                RandomCards.cardCommand = trimmedInput;
                RandomCards.cardMoves(RandomCards.cardCommand,textBoxTop);
            }
        }

        // Handle player death
        if (!RandomCards.hasGoldenIdol && RandomCards.playerHealth <= 0)
        {
            RandomCards.inCombatMode = false;
            intro();
            Setup();
            startGame = false;
            RandomCards.playerHealth = 40;
            return; // Exit Execute early after reset
        }
        else if (RandomCards.hasGoldenIdol && RandomCards.playerHealth <=0)
        {
            RandomCards.hasGoldenIdol = false;
            RandomCards.playerHealth = 100;
            Terminal.SetCursorPosition(3, textBoxTop + 4);
            Terminal.ClearLine();
            Terminal.WriteLine("Your Extra Life Was used");
        }

        // Clamp health and mana
        if (RandomCards.playerHealth > 100) RandomCards.playerHealth = 100;
        if (RandomCards.playerMana > 100) RandomCards.playerMana = 100;
        if (RandomCards.playerMana < 0) RandomCards.playerMana = 0;

        // Get current area enemies
        var currentEnemies = GetCurrentAreaEnemies(playerX);

        // Check if player is on an enemy tile
        var enemyHere = currentEnemies.FirstOrDefault(e => e.x == playerX && e.y == playerY);

        if (enemyHere != default)
        {
            // Player is on an enemy → enter combat
            if (!RandomCards.inCombatMode)
            {
                ClearTextBoxArea();
                RandomCards.inCombatMode = true;
                RandomCards.combatStartedThisEnemy = false; // reset message flag
                setEnemyHealth(playerX);
                RandomCards.enemyHealth = RandomCards.locationHealth;
            }

            if (!RandomCards.combatStartedThisEnemy)
            {
               
                Terminal.SetCursorPosition(3, textBoxTop);
                Console.WriteLine($"You are fighting an enemy! Mana: {RandomCards.playerMana}");
                RandomCards.combatStartedThisEnemy = true;
            }

            // Handle enemy defeat
            if (RandomCards.enemyHealth <= 0)
            {
                Console.WriteLine("Enemy Defeated! Mana restored by 25");
                RandomCards.playerMana += 25;

                // Remove enemy from appropriate array
                if (playerX < map.Width / 3)
                    enemyKilled(playerX, playerY, ref townEnemies);
                else if (playerX < 2 * map.Width / 3)
                    enemyKilled(playerX, playerY, ref forestEnemies);
                else
                    enemyKilled(playerX, playerY, ref castleEnemies);

                RandomCards.inCombatMode = false;
                RandomCards.combatStartedThisEnemy = false;
                DrawEnemies();
                playerY = Math.Min(height - 1, playerY + 1);
            }
        }
        else
        {
            // No enemy here  exit combat
            RandomCards.inCombatMode = false;
        }
        if (castleEnemies.Length == 0 && RandomCards.fightingBoss)
        {
            RandomCards.fightingBoss = false; // mark boss defeated
            RandomCards.inCombatMode = false;  // exit combat if still active
            ClearTextBoxArea();
            Terminal.SetCursorPosition(3, textBoxTop);
            Console.WriteLine("Congratulations! You have defeated the boss and completed the game!");
            Setup();
            // Optional: stop player movement or trigger end sequence
            startGame = false;
        }
        // Handle enemy attacks
        if (RandomCards.takenDamage)
        {
            RandomCards.playerHealth -= locationEnemyDamage;
            Terminal.SetCursorPosition(3, textBoxTop);
            Console.WriteLine($"The enemy has attacked you. You have {RandomCards.playerHealth} health left.");

            RandomCards.takenDamage = false;
        }

        // Update combat info in terminal if in combat
        if (RandomCards.inCombatMode)
        {
            ClearTextBoxArea();
            Terminal.SetCursorPosition(3, textBoxTop);
            Console.WriteLine($"Enemy has {RandomCards.enemyHealth} Health left and You have {RandomCards.playerMana} Mana left and {RandomCards.playerHealth} Health Left");
            Terminal.SetCursorPosition(3, textBoxTop+1);
            Console.WriteLine("Enter a code from a card!");
            Terminal.SetCursorPosition(3, textBoxTop + 3);
            Terminal.ClearLine();
            Terminal.SetCursorPosition(3, textBoxTop + 4);
            Terminal.ClearLine();
            RandomCards.combatText = false;
        }

        // Clear terminal if requested
        if (RandomCards.clearTerminal)
        {
            RandomCards.clearTerminal = false;
            ClearTextBoxArea();
            Terminal.SetCursorPosition(3, textBoxTop);
        }
    }
    public void typedCommands(string command)
    {
        ClearTextBoxArea();
        Terminal.SetCursorPosition(3, textBoxTop);
        switch (command)
        {
            case "":
            if (!startGame)
                {
                    intro();
                }
            break;

case "w":
            if (CanMoveTo(playerX, playerY - 1))
    {
        playerY = Math.Max(0, playerY - 1);
        Terminal.WriteLine("Player moved up");
    }
    else
    {
        Terminal.WriteLine("You can't move there yet! Kill the enemies first.");
    }
    break;

case "s":
    if (CanMoveTo(playerX, playerY + 1))
    {
        playerY = Math.Min(height - 1, playerY + 1);
        Terminal.WriteLine("Player moved down");
    }
    else
    {
        Terminal.WriteLine("You can't move there yet! Kill the enemies first.");
    }
    break;

case "a":
    if (CanMoveTo(playerX - 1, playerY))
    {
        playerX = Math.Max(0, playerX - 1);
        Terminal.WriteLine("Player moved left");
    }
    else
    {
        Terminal.WriteLine("You can't move there yet! Kill the enemies first.");
    }
    break;

case "d":
    if (CanMoveTo(playerX + 1, playerY))
    {
        playerX = Math.Min(width - 1, playerX + 1);
        Terminal.WriteLine("Player moved right");
    }
    else
    {
        Terminal.WriteLine("You can't move there yet! Kill the enemies first.");
    }
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
                    setEnemyHealth(playerX);
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
        // Redraw terrain
        for (int y = 0; y < map.Height; y++)
        {
            for (int x = 0; x < map.Width; x++)
            {
                // Only draw terrain tiles (ignore enemies)
                if (backgroundTiles[x, y].text != "😠" && backgroundTiles[x, y].text != "😡" && backgroundTiles[x, y].text != "😈")
                    map.Poke(x * 2, y, backgroundTiles[x, y]);
                else
                    map.Poke(x * 2, y, new ColoredText("  ", backgroundTiles[x, y].fgColor, backgroundTiles[x, y].bgColor));
            }
        }

        // Draw enemies
        foreach (var (x, y, sprite) in townEnemies) map.Poke(x * 2, y, sprite);
        foreach (var (x, y, sprite) in forestEnemies) map.Poke(x * 2, y, sprite);
        foreach (var (x, y, sprite) in castleEnemies) map.Poke(x * 2, y, sprite);
    }
    public void intro()
    {//Starting title screen
        string art = @"
                                      @@@@@@@                                                     
                                   @@@@@@@@@@@@@                                                  
                                  @@@@@@@@@@@@@@@@                                                
                                 @@@@@@@@@@@@@@@@@@                                               
                                @@@@@@@@@@@@@@@@@@@@                                              
                                @@@@@@@@@@@@@@@@@@@@@                                             
                             @@@@@@@@@@@@@@@@@@@@@@@@@@@@                                         
                           @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                                      
                           @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                                    
                               @@@@@@@@@@@@@@@@@@%%%%%%%@@@@@@@@                                  
                                   @@@@@@@@@@%%%%%%%%%%%@%%%%%%%@@@@@                             
                                   @@@@@@@%%%%%%%%%%%%@@@@@%%@@@@@@@@@@                           
                                     @@@%%%@%%%@@@@@@@@@@@@@@@@@@@@@@@@@                          
                                   @@%@%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                          
                                 @@%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@                           
                            @@@@@@@@@@@@@@@@@@@@@@@@@@%%%%%@%%%#-%@@@                             
                         @@@@@@@@@@@@@@@@@@@@@@@@@%%%%%@%%%%@#*:#@@                               
                        @@@@@@@@@@@@@@@@@@@@@@@@%%%%%%@@%%%%@=*@                                  
                         @@@@@@@@@@@@@@@@@%#%#%#+%%%%#**===*@@                                    
                          @@@@@@@@@@@%%%%%%+**#%%%%%%%%%%%%@@                                     
                                  @%%%%%#%%%%@%%#:..=%@@@@@@@@                                    
                                   @#**#@@@@@@@@@%%@@@@@@@@@@@@@                                  
                                      @@@@@@@@@@@@%#@@@@@@@@@@        @                           
                                         @@@@@+.-*.+@@@@@@@@  @*-+%@@*#%                          
                                        @@@@@@%##%@@@@@@@@@@ @+%@@@%%%@@                          
                                      @@@@@@@@##%+@@@@@@@@@@ @@@@#=:.:*                           
                                    @@@@@@@@@@@@@@@@@@@@@@@@@ @@*:.+%##@                          
                                  @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@*=%+:-+@                          
                                %%@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@  %%#%@                          
                              #-...:*@@@@@@@@@@@@@@@@@@@@@@@@@@@@@ %**#                           
                             %-.-%@#*@@@@@@@@@%%%@%@@@@@@@@@#+:::+%=:*@                           
                             %+%=@@@@@@@@@@@@%%%@%%%%@@@@@#=#%==%=:*%@                            
                             #=.-+%@@#*@@@@@%%%@@%%@@@@@%=%@@%*##%*%                              
                              @+::-*:.=%@@%%%%%@ @%%%%%@#*+:+:-%*#                                
                                @#===%@ @%%%%%@  @@%%%%@ @*===+%                                  
                                        @%%%%%@   @%%%%@                                          
                                        @%%%%%@   @%%%@                                           
                                        @*==*%@   @*=+%                                           
                                         **+=%     %+#%                                           
                                                                                                  
  ";                                                                                                
          Terminal.WriteLine(art);                                                                                      



        string logo = @"
 / $$$$$$$$        /$$                            /$$$$$$   /$$$$$$        /$$$$$$$$ /$$                       /$$$$$$$                       /$$          
|__  $$__/       | $$                           /$$__  $$ /$$__  $$      |__  $$__/| $$                      | $$__  $$                     | $$          
   | $$  /$$$$$$ | $$  /$$$$$$   /$$$$$$$      | $$  \ $$| $$  \__/         | $$   | $$$$$$$   /$$$$$$       | $$  \ $$ /$$$$$$   /$$$$$$$ /$$$$$$        
   | $$ |____  $$| $$ /$$__  $$ /$$_____/      | $$  | $$| $$$$             | $$   | $$__  $$ /$$__  $$      | $$$$$$$/|____  $$ /$$_____/|_  $$_/        
   | $$  /$$$$$$$| $$| $$$$$$$$|  $$$$$$       | $$  | $$| $$_/             | $$   | $$  \ $$| $$$$$$$$      | $$____/  /$$$$$$$|  $$$$$$   | $$          
   | $$ /$$__  $$| $$| $$_____/ \____  $$      | $$  | $$| $$               | $$   | $$  | $$| $$_____/      | $$      /$$__  $$ \____  $$  | $$ /$$      
   | $$|  $$$$$$$| $$|  $$$$$$$ /$$$$$$$/      |  $$$$$$/| $$               | $$   | $$  | $$|  $$$$$$$      | $$     |  $$$$$$$ /$$$$$$$/  |  $$$$/      
   |__/ \_______/|__/ \_______/|_______/        \______/ |__/               |__/   |__/  |__/ \_______/      |__/      \_______/|_______/    \___/        
                                                                                                                                                    
";

        Terminal.WriteLine(logo);
        Terminal.WriteLine("");
        Terminal.WriteLine("                      Press ENTER to Start");

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
    public void ClearTextBoxArea()
    {

        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;

        for (int i = textBoxTop; i < textBoxTop + 2; i++) // clear input + next line
        {
            Console.SetCursorPosition(0, i);
            Console.Write(new string(' ', Console.WindowWidth));
        }

        Console.ResetColor();
    }
    // 
    // Enemy & Barrier Logic
    // 
    public bool IsNearEnemy(int x, int y, (int x, int y, ColoredText sprite)[] enemies)
    {
        foreach (var (ex, ey, _) in enemies)
            if (Math.Abs(x - ex) <= 1 && Math.Abs(y - ey) <= 1) return true;
        return false;
    }

    public bool AreEnemiesDefeated((int x, int y, ColoredText sprite)[] enemies) => enemies.Length == 0;

    int requiredTownKills = 5;
    int requiredForestKills = 7;
    bool CanMoveTo(int x, int y)
    {
        int third = map.Width / 3;

        // Town → Forest boundary
        if (x == third && (townEnemies.Length > (7 - requiredTownKills))) return false;

        // Forest → Castle boundary
        if (x == 2 * third && (forestEnemies.Length > (9 - requiredForestKills))) return false;

        return true;
    }

    (int x, int y, ColoredText sprite)[] GetCurrentAreaEnemies(int x)
    {
        int third = map.Width / 3;
        if (x < third)
            return townEnemies;
        if (x < 2 * third) return forestEnemies;
        return castleEnemies;
    }
    public void enemyKilled(int x, int y, ref (int x, int y, ColoredText sprite)[] enemies)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i].x == x && enemies[i].y == y)
            {
                // Reset the background at this enemy's position
                ResetCell(enemies[i].x, enemies[i].y);

                // Remove the enemy from the array
                var temp = enemies.ToList();
                temp.RemoveAt(i);
                enemies = temp.ToArray();
                break;
            }
        }
    }
    public void setEnemyHealth(int playerX)//sets the enemys stats where the player is
    {
        int third = map.Width / 3;
        if (playerX < third)
        {
            RandomCards.locationHealth = 25;//town
            locationEnemyDamage = enemyDamage;
        }
        else if (playerX < 2 * third)
        {
            RandomCards.locationHealth = 40; //forest
            locationEnemyDamage = majorEnemyDamage;
            Terminal.SetCursorPosition(3, textBoxTop + 5);
            Terminal.ClearLine();
            Terminal.WriteLine("Enemies now Have 40 health");
        }
        else
        {
            RandomCards.fightingBoss = true;
            locationEnemyDamage = bossEnemyDamage;
            Terminal.SetCursorPosition(3, textBoxTop + 5);
            Terminal.ClearLine();
            Terminal.WriteLine("The Boss Has 200 health");
            RandomCards.locationHealth = 130; //castle
        }
    }
}
