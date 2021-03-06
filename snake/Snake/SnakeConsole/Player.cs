﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SnakeConsole
{
    class Player
    {
        string playerName;
        int playerScore;

        public Player()
        {
        }

        public Player(string player, int score)
        {
            playerName = player;
            playerScore = score;
        }

        // reads each line from the .csv file and puts into a list of players
        public List<Player> GetPlayerValues(string filePath)
        {
            List<Player> values = new List<Player>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;

                while ((line = reader.ReadLine()) != null)
                {
                    string[] dataValues = line.Split(',');

                    string name = Convert.ToString(dataValues[0]);
                    int score = Convert.ToInt32(dataValues[1]);
                    
                    values.Add(new Player(name, score));
                }
            }
            return values;
        }
        
        // saves player name and score to the .csv file
        public void SavePlayerScore(string player, int score)
        {
            //"C:\\high_score_test.csv"
            //"high_score.csv"
            using (StreamWriter writer = new StreamWriter("high_score.csv", true))
            {
                writer.WriteLine("{0},{1}", player, score);
            }           
        }
        
        // Prints the top 10 players
        public void PrintScoreBoard()
        {
            List<Player> player = GetPlayerValues("high_score.csv");

            var rank =
                 (from s in player
                 orderby s.playerScore descending
                 select s).Take(10);

            foreach(var name in rank)
            {
                Console.WriteLine(name);
            }
        }

        // overide ToString method for printing out players
        public override string ToString()
        {
            return string.Format("player: {0,-9} Score: {1}", playerName, playerScore);
        }
    }
}
