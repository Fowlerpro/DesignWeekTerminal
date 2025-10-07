using System;
using MohawkTerminalGame;

public static class RandomCards
{
    public static int enemyHealth = 20;// health of every single enemy
    public static int playerHealth = 40;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    public static bool inCombatMode = false;
    public static bool waitingResponse = false;
    public static int playerAbilites = 4;

    public static void cardMoves()
    {
        if (inCombatMode)
        {
            enemyHealth = 20;
            Program.TerminalInputMode = TerminalInputMode.KeyboardReadAndReadLine;
            Console.WriteLine($"inCombatMode is {inCombatMode}");
            if (!waitingResponse)
            {

                waitingResponse = true;
                string input = Terminal.ReadLine();
                Console.WriteLine($"waitingResponse is {waitingResponse}");
                if (input == "atk1")
                {

                    enemyHealth -= 1;
                    Terminal.WriteLine("you attacked the enemy");
                    Terminal.WriteLine($"Enemy has {enemyHealth} Health");
                    takenDamage = true;
                    waitingResponse = false;

                }
            }
        }
    }
}



