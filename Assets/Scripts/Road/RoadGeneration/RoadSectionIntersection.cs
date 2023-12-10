using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSectionIntersection : RoadSection
    {
        public static new List<Type> possibleNeighbours = new List<Type>() { 
            typeof(RoadSectionStart),
            //typeof(RoadSectionDeadEnd),
        };

        protected new void Start()
        {
            base.Start();
            Debug.Log("Je suis une intersection");
            CreateNextRoad();
        }

        public override List<Type> GetPossibleNeighbours()
        {
            return possibleNeighbours;
        }
    }
}