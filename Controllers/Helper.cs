using System;
using System.Collections.Generic;
using System.Linq;
using Angeloid.Models;

namespace Angeloid.Controllers
{   
    //Class for getting name of an object
    public static class Helper
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

        public static bool isTheSameTag(List<Tag> l1, List<Tag> l2)
        {
            if (l1.Count() == l2.Count())
            {
                for (int i = 0; i < l1.Count(); i++)
                {
                    if (l1[i].TagId != l2[i].TagId)
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool isTheSameCharacter(List<Character> l1, List<Character> l2)
        {
            if (l1.Count() == l2.Count())
            {
                for (int i = 0; i < l1.Count(); i++)
                {
                    if (!l1[i].CharacterName.Equals(l2[i].CharacterName))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}