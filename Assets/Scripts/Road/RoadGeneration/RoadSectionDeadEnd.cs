using System;
using System.Collections;
using System.Collections.Generic;
using Other;
using UnityEngine;

namespace JonathonOH.RoadGeneration
{
    public class RoadSectionDeadEnd : RoadSection
    {
        public static new List<Type> possibleNeighbours = new List<Type>() {};

        public override bool IsValidForNextSection(RoadSection section)
        {
            return false;
        }
    }
}