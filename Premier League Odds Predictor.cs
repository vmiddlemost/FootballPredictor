using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program {

    static void Arsenal(bool homeGame, string opposition) {
        // first create a string array of all arsenal results from the .txt file
        string[] fixtureList = File.ReadAllLines("arsenalResults.txt");

        // create previousHeadToHeads list so the code can add to the list
        List<string> previousHeadToHeads = new List<string>();

        /* find all games with opposition in fixtureList - seperated by if it's a home or away game -
        and add to previousHeadToHeads list */
        foreach (string game in fixtureList) {
            if (homeGame) {
                if (game[8] == opposition[0] && game[9] == opposition[1] && game[10] == opposition[2]) {
                    previousHeadToHeads.Add(game);
                }
            } else {
                if (game[0] == opposition[0] && game[1] == opposition[1] && game[2] == opposition[2]) {
                    previousHeadToHeads.Add(game);
                }
            }
        }

        // now to create an average scoreline from all previous head to head results with opposition
        double homeGoals = 0;
        double awayGoals = 0;
        foreach (string game in previousHeadToHeads) {
            homeGoals += Convert.ToDouble(Convert.ToInt32(game[4].ToString()));
            awayGoals += Convert.ToDouble(Convert.ToInt32(game[6].ToString()));
        }

        if (homeGame) {
            Console.WriteLine("Average scoreline: ARS " + (homeGoals/previousHeadToHeads.Count()) + "-" + (awayGoals/previousHeadToHeads.Count()) + " " + opposition);
        } else {
            Console.WriteLine("Average scoreline: " + opposition + " " + (homeGoals/previousHeadToHeads.Count()) + "-" + (awayGoals/previousHeadToHeads.Count()) + "  ARS");
        }
        

    }

    public static void Main(String[] args) {
        Arsenal(true, "TOT");
    }
}