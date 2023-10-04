using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildCity : MonoBehaviour
{
    [SerializeField]
    GameObject[] buildings;

    [SerializeField]
    GameObject cityObject;

    [SerializeField]
    private int mapWidth = 20;
    [SerializeField]
    private int mapHeight = 20;

    [SerializeField]
    float buildingFootprint = 1.01f;

    [SerializeField]
    float seed = 15.05f;

    void Start()
    {
        

        for (int h = 0; h < mapHeight;  h++)
        {
            for (int w = 0; w < mapWidth; w++)
            {
                int result = (int) (Mathf.PerlinNoise(w/15f + seed, h/15f + seed) * buildings.Length);
     
                Vector3 pos = new Vector3(w * buildingFootprint, 0, h * buildingFootprint);
                int index = (result / 2);
                Instantiate(buildings[index], pos, Quaternion.identity, cityObject.transform);    

            }
        }
    }
}
