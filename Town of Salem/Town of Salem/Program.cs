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
            public string role;
            public string personality;
            public bool alive;
            public int votes; //Number of votes their vote is worth
            public int[] suspicions; //Range from 0 to 100. 0 they trust someone with their lives. 100 they'll try to lynch them every round. 50 is neutral.
            public int[] roleThoughts; //What they think each players role is
        };

        public static string[] personalities = {"Timid", "Hostile", "Passive"};
        public static string[] roles = {"Innocent", "Mafia Killer"};        

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


            for (int currentPlayer = 0; currentPlayer < players.Length; currentPlayer++)
            {                
                players[currentPlayer].Name = $"{ReadLine("First Names.txt", rand.Next(FirstNamesTotal))} {ReadLine("Last Names.txt", rand.Next(LastNamesTotal))}";
                players[currentPlayer].role = roles[rand.Next(roles.Length)];
                players[currentPlayer].personality = personalities[rand.Next(personalities.Length)];
                players[currentPlayer].votes = 1;
                players[currentPlayer].alive = true;
                players[currentPlayer].suspicions = new int[playerCount];

                for (int targetPlayer = 0; targetPlayer < players[currentPlayer].suspicions.Length; targetPlayer++) //Set suspicions
                {
                    if (currentPlayer == targetPlayer)
                    {
                        players[currentPlayer].suspicions[targetPlayer] = 0;
                    }
                    else if (players[currentPlayer].role == "Mafia Killer" && players[targetPlayer].role == "Mafia Killer")
                    {
                        players[currentPlayer].suspicions[targetPlayer] = 0;
                    }
                    else
                    {
                        players[currentPlayer].suspicions[targetPlayer] = 50;
                    }

                }

            }

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
            Console.ReadLine();
        }
    }
}
