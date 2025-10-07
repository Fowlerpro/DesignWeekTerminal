using System;
using MohawkTerminalGame;

public static class RandomCards
{
	public static int enemyHealth = 20;// health of every single enemy
    public static int playerHealth = 40;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    
    public static void cardMoves()
	{

		string input = Terminal.ReadLine();
		if (input == "atk1")
		{

            
    enemyHealth -= 1;
            Terminal.WriteLine("you attacked the enemy")
			Terminal.WriteLine($"Enemy has {enemyHealth} Health");
            takenDamage = true;
            
        }
	}
}



