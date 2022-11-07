using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

public class Globals {
    public string[] globalDrawsArray = File.ReadAllLines("drawChances.txt");
}

class Program {

    static int normalDistribution(double mean, double SD, double goals) {
        // a separate method for calculating the percentage chance of a win, draw or loss using the normal distribution formula
        var normal = new Normal(mean, SD);
        double probability = normal.CumulativeDistribution(goals);
        //return 100 * probability;
        return Convert.ToInt32(100 * probability);
    }

    static double drawProbability(string oppositionTeam) {
        // initialize drawChances.txt array
        Globals drawsArray = new Globals();
        // initialize string arrays for the team of your choice (Can only be Arsenal at the moment) and the opposition team
        string[] teamDrawChance = new string[2];
        string[] oppositionDrawChance = new string[2];

        foreach (string line in drawsArray.globalDrawsArray) {
            // check if line is your team's draw chance (again, at the moment can only be Arsenal) and save that to teamDrawChance array
            if (line[0] == 'A' && line[1] == 'R' && line[2] == 'S') {
                teamDrawChance = line.Split(' ');
            // check if line is the opposition team's draw chance and save that to oppositionDrawChance array
            } if (line[0] == oppositionTeam[0] && line[1] == oppositionTeam[1] && line[2] == oppositionTeam[2]) {
                oppositionDrawChance = line.Split(' ');
            }
        }

        // find average of both teams draw chances and return it as a double
        return (Convert.ToDouble(teamDrawChance[1]) + Convert.ToDouble(oppositionDrawChance[1])) / 2;
    }

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
        /* if the difference between each game's goal difference and the overall average goal difference is 0 - likely to happen with some cases
        the chance to win becomes 100%, which is just not correct whatsoever so I'm adding this edge case in to make the probabilties more realistic*/
        double standardDeviation = 2 * Math.Sqrt((distanceToMean/previousHeadToHeads.Count()));
        if (distanceToMean == 0) {
            standardDeviation = 0.8;
        }
        
        // save the draw chance to a double as to not run method, drawProbability more than needed
        double drawChance = drawProbability(opposition);
        Console.WriteLine(drawChance);

        // final output of percentages
        Console.WriteLine("The average goal difference between the two sides is: " + meanScoreDifference + "\nwith a standard deviation of: " + standardDeviation);
        Console.WriteLine("The following numbers are the probability the game will be a:\nWin: " + (100 - normalDistribution(meanScoreDifference, standardDeviation, drawChance + 0.25)) + "%"
         + "\nDraw: " + (normalDistribution(meanScoreDifference, standardDeviation, drawChance + 0.25) - normalDistribution(meanScoreDifference, standardDeviation, -0.25 - drawChance)) + "%"
         + "\nLoss: " + (normalDistribution(meanScoreDifference, standardDeviation, -0.25 - drawChance)) + "%");

     

    }

    public static void Main(String[] args) {
        Arsenal(false, "WOL");
    }
}