using System;

namespace Angeloid.Controllers
{   
    //Class for getting name of an object
    public static class SeasonNaming
    {
        // Return seasonName in text 
        public static string getSeasonInText(DateTime season) {

            if (1 <= season.Month && season.Month <= 3)
            {
                return "Winter";
            }
            if (4 <= season.Month && season.Month <= 6)
            {
                return "Spring";
            }
            if (7 <= season.Month && season.Month <= 9)
            {
                return "Summer";
            }
            
            return "Fall";
        }
    }
}