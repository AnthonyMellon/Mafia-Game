using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;

namespace Town_of_Salem
{
    class Program
    {
        public struct Player
        {
            public string Name;
            public string affiliation;
            public string role;
            public string personality;
            public bool alive;
            public string message;
            public bool somethingToSay;
            public int votes; //Number of votes their vote is worth
            public int[] suspicions; //Range from 0 to 100. 0 they trust someone with their lives. 100 they'll try to lynch them every round. 50 is neutral.
            public string[] roleThoughts; //What they think each players role is
        };

        public static string[] personalities = { "Timid", "Hostile", "Passive" };
        public static string[] Affiliations = {"Town", "Mafia", "Self"};
        public static string[] TownRoles = { "Innocent"};
        public static string[] MafiaRoles = {"Killer", "GodFather"};
        public static string[] SelfRoles = {"Serial Killer"};

        static string ReadLine(string filePath, int line)
        {
            string output;
            int count = 0;
            StreamReader reader = new StreamReader(filePath);
            do
            {
                output = reader.ReadLine();
                count++;
            } while (count < line && !reader.EndOfStream);
            reader.Close();
            return output;
        }
        static int GetFileLength(string filePath)
        {
            StreamReader reader = new StreamReader(filePath);

            int length = 0;
            do
            {
                reader.ReadLine();
                length++;

            } while (!reader.EndOfStream);

            reader.Close();
            return length;
        }
        static void WriteMessage(string name, string message, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(name + ": ");

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(message);
        }
        static int restrict(int number, int min, int max)
        {
            if (number < min)
            {
                number = min;
            }
            else if (number > max)
            {
                number = max;
            }
            return number;
        }

        static void Initialisation()
        {
            int playerCount = 13;
            Player[] players = new Player[playerCount];
            Random rand = new Random();
            int FirstNamesTotal = GetFileLength("First Names.txt");
            int LastNamesTotal = GetFileLength("Last Names.txt");            


            for (int currentPlayer = 0; currentPlayer < players.Length; currentPlayer++) //Set up all players settings that dont need knowledge of other players
            {       
                players[currentPlayer].Name = $"{ReadLine("First Names.txt", rand.Next(FirstNamesTotal))} {ReadLine("Last Names.txt", rand.Next(LastNamesTotal))}";
                players[currentPlayer].affiliation = Affiliations[rand.Next(Affiliations.Length)];                
                players[currentPlayer].personality = personalities[rand.Next(personalities.Length)];
                players[currentPlayer].votes = 1;
                players[currentPlayer].alive = true;
                players[currentPlayer].suspicions = new int[playerCount];
                players[currentPlayer].roleThoughts = new string[playerCount];                
                players[currentPlayer].somethingToSay = true;

                switch(players[currentPlayer].affiliation) //Set role based on affiliation
                {
                    case "Town":
                        players[currentPlayer].role = TownRoles[rand.Next(TownRoles.Length)]; 
                        break;
                    case "Mafia":
                        players[currentPlayer].role = MafiaRoles[rand.Next(MafiaRoles.Length)];
                        break;
                    case "Self":
                        players[currentPlayer].role = SelfRoles[rand.Next(SelfRoles.Length)];
                        break;
                }
                
                
            } //First character loop END

            for (int currentPlayer = 0; currentPlayer < players.Length; currentPlayer++) //Going through each player again to set stats that need knowledge of other players
            {
                for (int targetPlayer = 0; targetPlayer < players[currentPlayer].suspicions.Length; targetPlayer++) //Set suspicions
                {
                    if (currentPlayer == targetPlayer)
                    {
                        players[currentPlayer].suspicions[targetPlayer] = 0;
                        players[currentPlayer].roleThoughts[targetPlayer] = players[currentPlayer].role;
                    }
                    else
                    {
                        if (players[currentPlayer].affiliation == "Mafia" && (players[currentPlayer].affiliation == players[targetPlayer].affiliation))
                        {
                            players[currentPlayer].suspicions[targetPlayer] = 0;
                            players[currentPlayer].roleThoughts[targetPlayer] = players[targetPlayer].role;                                                        
                        }
                        else
                        {
                            players[currentPlayer].suspicions[targetPlayer] = rand.Next(100);
                        }
                        
                    }
                                    
                } //Set suspicions END

            } //Second charatcer loop END

            Console.WriteLine("What is your name?");
            players[0].Name = Console.ReadLine();
            Console.Clear();
            GameLoop(players);
        }

        static void GameLoop(Player[] playerList)
        {            
            const int SUSPICION_THRESHOLD_NORMAL = 75, SUSPICION_THRESHOLD_EXTREME = 90, TRUSTING_THRESHOLD_NORMAL = 25, TRUSTING_THRESHOLD_EXTREME = 10;
            bool game = true;
            Random rand = new Random();
            while(game == true)
            {
                int endDiscussionVotes;
                do //Go through each player to see if they have anything to say
                {
                    endDiscussionVotes = 0;
                    for (int currentPlayer = 0; currentPlayer < playerList.Length; currentPlayer++)
                    {
                        Console.Write("Do you have anything you'd like to add ");
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(playerList[currentPlayer].Name + "?");
                        Console.ForegroundColor = ConsoleColor.Gray;                        
                        if (currentPlayer == 0) //If it's the human controlled player
                        {
                            string playerMessage;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(playerList[currentPlayer].Name + ": ");
                            Console.ForegroundColor = ConsoleColor.Gray;
                            playerMessage = Console.ReadLine();
                            if (playerMessage.ToLower() == "no")
                            {
                                endDiscussionVotes++;
                            }
                        }
                        else //If it's a computer controlled player
                        {
                            string cpuMessage;
                            if (playerList[currentPlayer].somethingToSay == true)
                            {                                
                                cpuMessage = "I'm suspicious of ";
                                for (int target = 0; target < playerList.Length; target++)
                                {
                                    if (playerList[currentPlayer].suspicions[target] > SUSPICION_THRESHOLD_NORMAL)
                                    {                
                                        for (int i = 0; i < playerList.Length; i++) //Update each players suspicions based on what this player thinks
                                        {
                                            playerList[i].suspicions[target] = restrict((playerList[i].suspicions[target] + restrict((playerList[i].suspicions[target] / 10) - (playerList[i].suspicions[currentPlayer] / 10), 1, 10)), 0, 100);
                                            playerList[i].suspicions[currentPlayer] = restrict((playerList[i].suspicions[currentPlayer] + (playerList[i].suspicions[currentPlayer] / 10) - (playerList[i].suspicions[target] / 10)), 0, 100);
                                        }
                                        cpuMessage += (playerList[target].Name + " and ");
                                    }
                                }
                                playerList[currentPlayer].message = cpuMessage;
                            }
                            else
                            {
                                cpuMessage = "No";
                                endDiscussionVotes++;
                            }
                            WriteMessage(playerList[currentPlayer].Name, cpuMessage + "\n", ConsoleColor.Green);                            
                        }
                        Thread.Sleep(100);
                    }

                    Console.WriteLine(endDiscussionVotes + " of " + playerList.Length + " votes");
                    Console.ReadLine();
                    Console.Clear();

                } while (endDiscussionVotes < playerList.Length);
                game = false;
                Console.WriteLine("End of game");
                Console.ReadLine();
                
            }
        }

        static void Main()
        {
            Initialisation();            
        }
    }
}
