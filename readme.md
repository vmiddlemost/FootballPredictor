About:
This is a project I am currently working on which allows me to predict the probability of either a win, draw or a loss of an upcoming premier league fixture.
The project currently only takes into account Arsenal games from an Arsenal perspective, but once I've added more to the core structure I plan to add more premier league teams.
Currently the prediction is based entirely off of previous head-to-head game results, dating back 3 seasons. But I plan to add in form among other factors into the formula.
I also plan to build UI functionality for this so it's easier to use, but at the moment it can be used by editing the code in the Main method.


How to use:
Simply edit the Main method in the code to use the method, Arsenal.
Arsenal takes in two arguments, the first being a bool for whether the fixture is at home or away for Arsenal (True or False).
The second argument being a string which indicates which team Arsenal will face in the fixture you want the probability for.
It will only take in specific string arguments which are all 3 letters long, for eg. Tottenham Hotspurs = "TOT", West Ham United = "WHU", Manchester City = "MCI".
Just edit the Main method to whatever you'd like and run the code in an IDE of your choice, and if Arsenal have faced them in the premier league in the past 3 seasons, the code will output probabilities for the result.