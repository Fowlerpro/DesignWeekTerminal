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


    public static void cardMoves(string cardCommand)
    {
        
        if (inCombatMode)
        {
            if (!enemyDead)
            {
                enemyHealth = 20;
                Console.WriteLine("Enter a code from a card!");
                enemyDead = true;
                mapClear = true;
            }
            else
            {
                Terminal.WriteLine($"Enemy has {enemyHealth} Health");
                Console.WriteLine("Enter a code from a card!");
                switch (cardCommand)//expandable switch statment for all card abilities
                {
                    case "atk1":
                        enemyHealth -= 1;
                        Terminal.WriteLine("you attacked the enemy");
                        takenDamage = true;
                        break;

                    default:
                        Terminal.WriteLine("Invalid Card Code");
                        Terminal.Beep();
                        break;
                }
                
            }
        }
    }
}



