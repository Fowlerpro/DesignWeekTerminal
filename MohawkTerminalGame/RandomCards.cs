using System;
using MohawkTerminalGame;

public static class RandomCards
{
    public static int locationHealth = 1;
    public static int enemyHealth = locationHealth;// health of every single enemy
    public static int playerHealth = 100;//this is the player health change this to make the game easier or harder
    public static bool takenDamage = false;
    public static bool inCombatMode = false;
    public static int playerMana = 100;//number of ability points per round
    public static string cardCommand = "";
    public static bool hasGoldenIdol = false;
    static bool goldenIdolUsed = false;
    public static bool fightingBoss = false;
    public static bool clearTerminal = false;
    public static bool combatStartedThisEnemy;
    public static bool combatText = true;

    public static void cardMoves(string cardCommand, int textBoxTop)
    {

        if (!inCombatMode) return;
        {
            { 
                takenDamage = true;

                switch (cardCommand)//expandable switch statment for all card abilities
                {
                    case "atk"://basic attack
                        enemyHealth -= 5;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Deal 5 damage");
                        break;
                    case "gdl"://golden idle
                        if (!goldenIdolUsed)
                        {
                            hasGoldenIdol = true;
                            Terminal.SetCursorPosition(3, textBoxTop + 2);
                            Terminal.ClearLine();
                            Terminal.WriteLine("You have gained an extra life");
                            goldenIdolUsed = true;
                        }
                        else
                        {
                            Terminal.SetCursorPosition(3, textBoxTop + 2);
                            Terminal.ClearLine();
                            Terminal.WriteLine("Card already Used");
                        }
                        break;
                    case "hpn"://basic healing potion
                        playerHealth += 5;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Heal 5");
                        break;
                    case "mpn"://basic mana potion
                        playerMana += 5;
                        break;
                    case "avk"://advanced attack
                        enemyHealth -= 10;
                        break;
                    case "mss"://missed
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("You Missed");
                        break;
                    case "flh"://flesh wound
                        playerHealth -= 5;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Its just a flesh wound, lose 5 health");
                        break;
                    case "hsw"://Knight
                        {//Holy sword
                            if (playerMana >= 15)
                            {


                                playerMana -= 15;
                                enemyHealth -= 15;
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 15 Mana, You Deal 15 Damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "gsw"://Glowing Sword
                        enemyHealth -= 15;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("ooooh Shiny, You deal 15 Damage");
                        break;
                    case "lck"://Lucky Bastard
                        playerHealth += 50;
                        playerMana += 30;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Why are you so Lucky? Gain 50 Health and 30 Mana");
                        break;
                    case "scr"://Sacred Ring
                        playerHealth += 15;
                        playerMana += 10;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Seems Pretty Special, Heal 15 health and restore 10 Mana");
                        break;
                    case "mga"://Magical Armor
                        playerHealth += 5;
                        playerMana += 15;
                        Terminal.WriteLine("You restored 5 health and 15 mana");
                        break;
                    case "ckm"://Cooked Meat
                        playerHealth += 10;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Damn that smells Good!, Heal 10 Health");
                        break;
                    case "soc"://Stylish Overcoat
                        enemyHealth -= 10;

                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Wow that looks good on you!, Deal 10 Damage");
                        break;
                    case "ahp"://Advanced Healing Potion
                        playerHealth += 25;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Its just that simple, gain 25hp");
                        break;
                    case "rst"://Full Rest
                        playerMana += 35;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("nighty Night Sleepy Head, Restore 35 Mana");
                        break;
                    case "rpn"://Recovery Potion
                        playerMana += 15;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("It looks funny Gain 15 mana");
                        break;
                    //negatives
                    case "cnr"://Dragon Cancer
                        playerMana -= 15;
                        playerHealth -= 15;
                        enemyHealth -= 10;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Im So Sorry, lose 15 Mana and 15 Health Deal 10 Damage");
                        break;
                    case "tch"://The Coughing Death
                        playerHealth -= 25;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("This is Pretty Bad, lose 25 health");
                        break;
                    case "tbd"://The Black Death
                        playerHealth -= 50;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("How did it get this Bad, lose 50 Health");
                        break;
                    case "whs"://Whoops
                        playerHealth -= 5;
                        playerMana -= 5;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("you break your leg, lose 5 Health and 5 Mana");
                        break;
                    case "lku"://Look Up
                        playerHealth -= 1;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Hey its a rock, lose 1 Health");
                        break;
                    case "amb"://Ambush
                        playerHealth -= 15;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("You get Jumped by Monsters, lose 15 Health");
                        break;
                    case "mug"://Mugged
                        playerHealth -= 10;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("A Cloaked Man Stabs you, lose 10 Health");
                        break;
                    case "brg"://Barrage of Arrows
                        playerHealth -= 15;
                        playerMana -= 10;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Its Kebab Time, lose 15 health and 10 Mana");
                        break;
                    case "drk"://Dark Angel
                        playerHealth -= 20;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("What is that doing here?, lose 20 health");
                        break;
                    case "wzd"://Wizzard of the South
                        playerMana -= 15;
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("But we are in the North did he get lost?, lose 15 Mana");
                        break;
                    case "sld"://shield Bash
                        {
                            if (playerMana >= 10)
                            {
                                playerMana -= 10;
                                enemyHealth -= 5;
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 10 Mana, and Dealt 5 Damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
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
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 25 Mana, and restored 15 Health");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
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
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 15 Mana, and dealt 10 Damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;
                    case "mrs":// mana restore 
                        {
                            playerMana += 30;
                            Terminal.SetCursorPosition(3, textBoxTop + 2);
                            Terminal.ClearLine();
                            Terminal.WriteLine("You have restored 30 Mana");
                        }
                        break;
                    case "idh"://instant Death
                        {
                            if (playerMana >= 60 && !fightingBoss)
                            {
                                playerMana -= 60;
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 60 Mana, and destroyed the enemy");
                            }
                            else if (playerMana >= 60 && fightingBoss)
                            {
                                playerMana -= 60;
                                enemyHealth -= 40;
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 60 Mana, and damaged the enemy");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
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
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 20 Mana, and Dealt 15 damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
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
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 15 Mana, and Dealt 10 Damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
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
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("You have used 25 Mana, and Dealt 20 damage");
                            }
                            else
                            {
                                Terminal.SetCursorPosition(3, textBoxTop + 2);
                                Terminal.ClearLine();
                                Terminal.WriteLine("Not enough Mana");
                            }
                        }
                        break;

                    default:
                        Terminal.SetCursorPosition(3, textBoxTop + 2);
                        Terminal.ClearLine();
                        Terminal.WriteLine("Invalid Card Code");
                        Terminal.Beep();
                        break;
                }
            }
        }
    }
}




