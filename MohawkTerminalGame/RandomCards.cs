using System;
using MohawkTerminalGame;

public static class RandomCards
{
    public static int enemyHealth = 20;// health of every single enemy
    public static int playerHealth = 40;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    public static bool inCombatMode = false;
    public static int playerAbilites = 4;

    public static void cardMoves()
    {
        if (inCombatMode)
        {
            enemyHealth = 20;
            Console.WriteLine($"inCombatMode is {inCombatMode}");

            string input = Terminal.ReadLine()?.Trim().ToLower();
            switch (input)//expandable switch statment for all card abilities
            {
                case "atk1":
                    enemyHealth -= 1;
                    Terminal.WriteLine("you attacked the enemy");
                    Terminal.WriteLine($"Enemy has {enemyHealth} Health");
                    takenDamage = true;
                    break;

                default:
                    Terminal.WriteLine("Invalid Card Code");
                    break;
            }

        }
    }
}



