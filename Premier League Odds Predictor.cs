using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MathNet.Numerics.Distributions;

public class Globals {
    public string[] globalDrawsArray = File.ReadAllLines("drawChances.txt");
    public string[] globalFormArray = File.ReadAllLines("formTable.txt");
}

class Program {

    static int normalDistribution(double mean, double SD, double goals) {
    // a separate method for calculating the percentage chance of a win, draw or loss using the normal distribution formula
    var normal = new Normal(mean, SD);
    double probability = normal.CumulativeDistribution(goals);
    //return 100 * probability;
    return Convert.ToInt32(100 * probability);
    }

    static double formFactor(string oppositionTeam) {
        // a method to determine the difference between both team's forms which will then get multiplied to the mean in the normal distribution formula
        // initialize formTable.txt
        Globals formArray = new Globals();
        // initialize string arrays for the team of your choice (Can only be Arsenal at the moment) and the opposition team
        string[] teamForm = new string[2];
        string[] oppositionForm = new string[2];

        foreach (string line in formArray.globalFormArray) {
            // check if line is your team's form (again, at the moment can only be Arsenal) and save that to teamForm array
            if (line[0] == 'A' && line[1] == 'R' && line[2] == 'S') {
                teamForm = line.Split(' ');
            // check if line is the opposition team's draw chance and save that to oppositionDrawChance array
            } if (line[0] == oppositionTeam[0] && line[1] == oppositionTeam[1] && line[2] == oppositionTeam[2]) {
                oppositionForm = line.Split(' ');
            }
        }

        // return 1/2 + the difference between both team's forms i.e if they're the same form, the form factor should halve the mean as to make the 
        // fixture more weighted towards a draw, as both teams are doing as well as each other
        return (0.5 + (Convert.ToDouble(teamForm[1]) - Convert.ToDouble(oppositionForm[1])));
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
                if (game[16] == opposition[0] && game[17] == opposition[1] && game[18] == opposition[2]) {
                    previousHeadToHeads.Add(game);
                }
            } else {
                if (game[0] == opposition[0] && game[1] == opposition[1] && game[2] == opposition[2]) {
                    previousHeadToHeads.Add(game);
                }
            }
        }

        // now to find all the goal differences between both sides and make a mean average of them
        // but first I need to create a double array with all the xG results contained inside so I can easily use the data
        double[] xGHeadToHeads = new double[previousHeadToHeads.Count * 2];
        int i = 0;
        double homeGoals = 0;
        double awayGoals = 0;
        double meanScoreDifference;
        foreach (string game in previousHeadToHeads) {
            Console.WriteLine(game);
            xGHeadToHeads[i] = Convert.ToDouble(game[4].ToString() + game[5].ToString() + game[6].ToString() + game[7].ToString());
            xGHeadToHeads[i+1] = Convert.ToDouble(game[11].ToString() + game[12].ToString() + game[13].ToString() + game[14].ToString());
            homeGoals += xGHeadToHeads[i];
            awayGoals += xGHeadToHeads[i+1];
            i+=2;
        }
        if (homeGame) {
            meanScoreDifference = (homeGoals/previousHeadToHeads.Count()) - (awayGoals/previousHeadToHeads.Count());
        } else {
            meanScoreDifference = (awayGoals/previousHeadToHeads.Count()) - (homeGoals/previousHeadToHeads.Count());
        }
        
        i = 0;
        // now we have the mean, time to calculate the standard deviation and distance to mean - these are needed for the normal distribution formula
        double distanceToMean = 0;
        foreach (string game in previousHeadToHeads) {
            if (homeGame) {
                distanceToMean += Math.Pow((xGHeadToHeads[i] - xGHeadToHeads[i+1]) - meanScoreDifference, 2);
            } else {
                distanceToMean += Math.Pow((xGHeadToHeads[i+1] - xGHeadToHeads[i]) - meanScoreDifference, 2);
            }
            i+=2;
        }
        // Here I calculate the standard deviation (and multiply by 2 to get "more fair" results)
        double standardDeviation = 2 * Math.Sqrt((distanceToMean/previousHeadToHeads.Count()));
        
        // save the draw chance to a double as to not run method, drawProbability more than needed - same with formFactor
        double drawChance = drawProbability(opposition);
        double formVariable = formFactor(opposition);

        // final output of percentages
        Console.WriteLine("The average expected goal difference between the two sides is: " + meanScoreDifference + "\nWith a standard deviation of: " + standardDeviation);
        Console.WriteLine("The following numbers are the probability the game will be a:\nWin: " + (100 - normalDistribution((meanScoreDifference * formVariable), standardDeviation, drawChance + 0.2)) + "%"
         + "\nDraw: " + (normalDistribution((meanScoreDifference * formVariable), standardDeviation, drawChance + 0.2) - normalDistribution((meanScoreDifference * formVariable), standardDeviation, -0.2 - drawChance)) + "%"
         + "\nLoss: " + (normalDistribution((meanScoreDifference * formVariable), standardDeviation, -0.2 - drawChance)) + "%");

     

    }

    public static void Main(String[] args) {
        Arsenal(true, "WOL");
        //Arsenal()
    }
}