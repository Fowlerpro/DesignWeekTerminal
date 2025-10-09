using System;
using MohawkTerminalGame;

public static class RandomCards
{
    public static int enemyHealth = 20;// health of every single enemy
    public static int playerHealth = 40;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    public static bool inCombatMode = false;
    public static int playerAbilites = 4;
    public static string cardCommand = "";
    public static bool mapClear = false;
    public static bool enemyDead = false;
    public static bool hasGoldenIdol = false;
    static bool goldenIdolUsed = false;

    public static void cardMoves(string cardCommand)
    {
        
            
            if (!enemyDead)
            {
                enemyHealth = 20;
                enemyDead = true;
                mapClear = true;
            }
            else
            {
                switch (cardCommand)//expandable switch statment for all card abilities
                {
                    case "atk1":
                        enemyHealth -= 1;
                        Terminal.WriteLine($"You attacked the enemy, Enemy has {enemyHealth} Health");
                        takenDamage = true;
                        break;
                case "gdl":
                    if (!goldenIdolUsed)
                    {
                        hasGoldenIdol = true;
                        Terminal.WriteLine("You have gained an extra life");
                        goldenIdolUsed = true;
                    }
                    else
                    {
                        Terminal.WriteLine("Card already Used");
                    }
                        break;

                default:
                        Terminal.WriteLine("Invalid Card Code");
                        Terminal.Beep();
                        break;
                }
                
            }
        }
    }




