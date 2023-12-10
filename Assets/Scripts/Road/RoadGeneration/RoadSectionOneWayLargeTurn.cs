using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSectionOneWayLargeTurn : RoadSectionOneWayTurn
    {
        public static new List<Type> possibleNeighbours = new List<Type>() {  
            typeof(RoadSectionStart), 
        };

        public override List<Type> GetPossibleNeighbours()
        {
            return possibleNeighbours;
        }

        public override bool IsValidForNextSection(RoadSection section)
        {
            if (base.IsValidForNextSection(section))
            {
                return true;
            }
            if(section.GetType() == typeof(RoadSectionOneWayTurn))
            {
                return section.IsFlipped != IsFlipped;
            }
            return false;
        }
    }
}