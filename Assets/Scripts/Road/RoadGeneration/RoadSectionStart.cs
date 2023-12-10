using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSectionStart : RoadSectionOneWay
    {
        public static new List<Type> possibleNeighbours = new List<Type>() {
            // typeof(RoadSectionOneWay),
            typeof(RoadSectionIntersection) ,
            typeof(RoadSectionOneWayTurn)
        };

        public override List<Type> GetPossibleNeighbours()
        {
            return possibleNeighbours;
        }
    }
}