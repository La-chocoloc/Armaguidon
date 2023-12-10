using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace JonathonOH.RoadGeneration
{
public class bikeDetector : MonoBehaviour
{
    [SerializeField] private RoadSection roadSection;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            if(other.name == "ArcadeBicycle")
            {
                roadSection.createNextRoad();
            }
        }
    }

}
