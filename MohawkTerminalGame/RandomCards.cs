using System;
using MohawkTerminalGame;

public static class RandomCards
{
    public static int enemyHealth = 20;// health of every single enemy
    public static int majorEnemyHealth = 40;// health of every single enemy
    public static int bossEnemyHealth = 200;
    public static int playerHealth = 100;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    public static bool inCombatMode = false;
    public static int playerMana = 100;//number of ability points per round
    public static string cardCommand = "";
    public static bool mapClear = false;
    public static bool enemyDead = false;
    public static bool hasGoldenIdol = false;
    static bool goldenIdolUsed = false;
    public static bool fightingBoss = false;

    public static void cardMoves(string cardCommand)
    {

        if (inCombatMode)
        {
            if (!enemyDead)
            {
                enemyHealth = 25;
                enemyDead = true;
                mapClear = true;
            }
            else
            {
                takenDamage = true;

                switch (cardCommand)//expandable switch statment for all card abilities
                {
                    case "atk1"://basic attack
                        enemyHealth -= 5;
                        majorEnemyHealth -= 5;
                        bossEnemyHealth -= 5;
                        break;
                    case "gdl"://golden idle
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
                    case "hpn"://basic healing potion
                        playerHealth += 5;
                        break;
                    case "mpn"://basic mana potion
                        playerMana += 5;
                        break;
                    case "avk"://advanced attack
                        enemyHealth -= 10;
                        bossEnemyHealth -= 10;
                        majorEnemyHealth -= 10;
                        break;
                    case "mss"://missed
                        Terminal.WriteLine("You Missed");
                        break;
                    case "flh"://flesh wound
                        playerHealth -= 5;
                        Terminal.WriteLine("Its just a flesh wound, lose 5 health");
                        break;
                    case "hsw"://Knight
                        {//Holy sword
                            if (playerMana >= 15)
                            {


                                playerMana -= 15;
                                enemyHealth -= 15;
                                majorEnemyHealth -= 15;
                                bossEnemyHealth -= 15;
                                Terminal.WriteLine("You have used 15 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "gsw"://Glowing Sword
                        enemyHealth -= 15;
                        bossEnemyHealth -= 15;
                        majorEnemyHealth -= 15;
                        Terminal.WriteLine("ooooh Shiny");
                        break;
                    case "lck"://Lucky Bastard
                        playerHealth += 50;
                        playerMana += 30;
                        Terminal.WriteLine("Why are you so Lucky? Gain 50 Health and 30 Mana");
                        break;
                    case "scr"://Sacred Ring
                        playerHealth += 15;
                        playerMana += 10;
                        Terminal.WriteLine("Seems Pretty Special");
                        break;
                    case "mga"://Magical Armor
                        playerHealth += 5;
                        playerMana += 15;
                        break;
                    case "ckm"://Cooked Meat
                        playerHealth += 10;
                        Terminal.WriteLine("Damn that smells Good!");
                        break;
                    case "soc"://Stylish Overcoat
                        enemyHealth -= 10;
                        majorEnemyHealth -= 10;
                        bossEnemyHealth -= 10;
                        Terminal.WriteLine("Wow that looks good on you!");
                        break;
                    case "ahp"://Advanced Healing Potion
                        playerHealth += 25;
                        Terminal.WriteLine("Its just that simple, gain 25hp");
                        break;
                    case "rst"://Full Rest
                        playerMana += 35;
                        Terminal.WriteLine("nighty Night Sleepy Head");
                        break;
                    case "rpn"://Recovery Potion
                        playerMana += 15;
                        Terminal.WriteLine("It looks funny Gain 15 mana");
                        break;
                    //negatives
                    case "cnr"://Dragon Cancer
                        playerMana -= 15;
                        playerHealth -= 15;
                        enemyHealth -= 10;
                        majorEnemyHealth -= 10;
                        bossEnemyHealth -= 10;
                        Terminal.WriteLine("Im So Sorry");
                        break;
                    case "tch"://The Coughing Death
                        playerHealth -= 25;
                        Terminal.WriteLine("This is Pretty Bad");
                        break;
                    case "tbd"://The Black Death
                        playerHealth -= 50;
                        Terminal.WriteLine("How did it get this Bad");
                        break;
                    case "whs"://Whoops
                        playerHealth -= 5;
                        playerMana -= 5;
                        Terminal.WriteLine("you break your leg");
                        break;
                    case "lku"://Look Up
                        playerHealth -= 1;
                        Terminal.WriteLine("Hey its a rock");
                        break;
                    case "amb"://Ambush
                        playerHealth -= 15;
                        Terminal.WriteLine("You get Jumped by Monsters");
                        break;
                    case "mug"://Mugged
                        playerHealth -= 10;
                        Terminal.WriteLine("A Cloaked Man Stabs you");
                        break;
                    case "brg"://Barrage of Arrows
                        playerHealth -= 15;
                        playerMana -= 10;
                        Terminal.WriteLine("Its Kebab Time");
                        break;
                    case "drk"://Dark Angel
                        playerHealth -= 20;
                        Terminal.WriteLine("What is that doing here?");
                        break;
                    case "wzd"://Wizzard of the South
                        playerMana -= 15;
                        Terminal.WriteLine("But we are in the North did he get lost?");
                        break;
                    case "sld"://shield Bash
                        {
                            if (playerMana >= 10)
                            {
                                playerMana -= 10;
                                enemyHealth -= 5;
                                majorEnemyHealth -= 5;
                                bossEnemyHealth -= 5;
                                Terminal.WriteLine("You have used 10 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "pry"://holy Prayer
                        {
                            if (playerMana >= 25)
                            {
                                playerMana -= 25;
                                playerHealth += 15;
                                Terminal.WriteLine("You have used 25 Mana, and restored 15 Health");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "fbl"://fireball witch
                        {
                            if (playerMana >= 15)
                            {
                                playerMana -= 15;
                                enemyHealth -= 10;
                                majorEnemyHealth -= 10;
                                bossEnemyHealth -= 10;
                                Terminal.WriteLine("You have used 15 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "mrs":// mana restore 
                        {
                            playerMana += 30;
                            Terminal.WriteLine("You have restored 30 Mana");
                        }
                        break;
                    case "idh"://instant Death
                        {
                            if (playerMana >= 60 && !fightingBoss)
                            {
                                playerMana -= 60;
                                enemyHealth = 0;
                                majorEnemyHealth = 0;
                                Terminal.WriteLine("You have used 60 Mana, and destroyed the enemy");
                            }
                            else if (playerMana >= 60 && fightingBoss)
                            {
                                playerMana -= 60;
                                bossEnemyHealth -= 40;
                                Terminal.WriteLine("You have used 60 Mana, and damaged the enemy");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "clk"://cloak theif
                        {
                            if (playerMana >= 20)
                            {
                                playerMana -= 20;
                                enemyHealth -= 15;
                                majorEnemyHealth -= 15;
                                bossEnemyHealth -= 15;
                                Terminal.WriteLine("You have used 20 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "stl"://steal
                        {
                            if (playerMana >= 15)
                            {
                                playerMana -= 15;
                                enemyHealth -= 10;
                                majorEnemyHealth -= 10;
                                bossEnemyHealth -= 10;
                                Terminal.WriteLine("You have used 15 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "dtr"://distraction
                        {
                            if (playerMana >= 25)
                            {
                                playerMana -= 25;
                                enemyHealth -= 20;
                                majorEnemyHealth -= 20;
                                bossEnemyHealth -= 15;
                                Terminal.WriteLine("You have used 25 Mana");
                            }
                            else
                            {
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;

                    default:
                        Terminal.WriteLine("Invalid Card Code");
                        Terminal.Beep();
                        break;
                }
                Terminal.WriteLine($"Enemy has {enemyHealth} Health left ");

            }
        }
    }
}




