using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

class Program {

    /*static double normalDistribution(double mean, double SD, double goals) {
        // a separate method for calculating the percentage chance of a win, draw or loss using the normal distribution formula
        return NormalDistribution((goals - mean) / SD);
    }*/

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

        // now to find all the goal differences between both sides and make a mean average of them
        int homeGoals = 0;
        int awayGoals = 0;
        double meanScoreDifference;
        foreach (string game in previousHeadToHeads) {
            Console.WriteLine(game);
            homeGoals += Convert.ToInt32(game[4].ToString());
            awayGoals += Convert.ToInt32(game[6].ToString());
        }
        if (homeGame) {
            meanScoreDifference = Convert.ToDouble((homeGoals/previousHeadToHeads.Count()) - (awayGoals/previousHeadToHeads.Count()));
        } else {
            meanScoreDifference = Convert.ToDouble((awayGoals/previousHeadToHeads.Count()) - (homeGoals/previousHeadToHeads.Count()));
        }
        
        // now we have the mean, time to calculate the standard deviation and distance to mean - these are needed for the normal distribution formula
        double distanceToMean = 0;
        foreach (string game in previousHeadToHeads) {
            if (homeGame) {
                distanceToMean += Math.Pow((Convert.ToInt32(game[4].ToString()) - Convert.ToInt32(game[6].ToString())) - meanScoreDifference, 2);
            } else {
                distanceToMean += Math.Pow((Convert.ToInt32(game[6].ToString()) - Convert.ToInt32(game[4].ToString())) - meanScoreDifference, 2);
            }
        }
        double standardDeviation = Math.Pow((distanceToMean/previousHeadToHeads.Count()), 0.5);

        // Final output of percentages
        Console.WriteLine("The average goal difference between the two sides is: " + meanScoreDifference + "\nwith a standard deviation of: " + standardDeviation);
        if (meanScoreDifference == 0 ) {
            Console.WriteLine("This indicates the game will likely be a draw");
        } if (meanScoreDifference > 0) {
            Console.WriteLine("This indicates the game will likely be a win");
        } else {
            Console.WriteLine("This indicates the game will likely be a loss");
        }


        

    }

    public static void Main(String[] args) {
        Arsenal(false, "MCI");
    }
}