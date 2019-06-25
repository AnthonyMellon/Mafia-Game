using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

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
            public int votes; //Number of votes their vote is worth
            public int[] suspicions; //Range from 0 to 100. 0 they trust someone with their lives. 100 they'll try to lynch them every round. 50 is neutral.
            public string[] roleThoughts; //What they think each players role is
        };

        public static string[] personalities = { "Timid", "Hostile", "Passive" };
        public static string[] Affiliations = {"Town", "Mafia", "Self"};
        public static string[] TownRoles = { "Innocent"};
        public static string[] MafiaRoles = {"Killer", "GodFather"};
        public static string[] SelfRoles = {"Serial Killer"};

        static void GameLoop()
        {

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
                            Console.WriteLine($"Im {players[currentPlayer].Name}, im affiliated with the {players[currentPlayer].affiliation} and my role is {players[currentPlayer].role}");
                            
                        }
                        else
                        {
                            players[currentPlayer].suspicions[targetPlayer] = 50;
                        }
                        
                    }
                                    
                } //Set suspicions END

            } //Second charatcer loop END
         
        }

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

        static void Main()
        {
            Initialisation();            
        }
    }
}
