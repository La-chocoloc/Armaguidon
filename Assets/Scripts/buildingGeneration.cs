using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class buildingGeneration : MonoBehaviour
{

    [SerializeField] GameObject[] buildings;
    [SerializeField] int rotation;
    [SerializeField] float spacing;

    MeshRenderer buildingSpaceRenderer;

    Bounds rendererbounds;

    //Bounds GizmoBounds;
    Bounds OgBounds;

    void Start()
    {
        buildingSpaceRenderer = GetComponent<MeshRenderer>();

        rendererbounds = buildingSpaceRenderer.bounds;

        selectBuilding(rendererbounds, rotation, spacing);
    }

    private void selectBuilding(Bounds spaceBounds, int rotation, float spacing)
    {
        Vector3 ogBoundsSize = spaceBounds.size;
        OgBounds = spaceBounds;
        List<GameObject> list = new List<GameObject>(buildings);
        
        while (spaceBounds.size.x > (ogBoundsSize * 0.1f).x && spaceBounds.size.z > (ogBoundsSize * 0.1f).z && list.Count!=0)
        {
            int randomIndex = Random.Range(0, list.Count);
            
            GameObject go = list[randomIndex];
             
            Bounds goBounds = go.GetComponent<MeshRenderer>().bounds;

            if (10*goBounds.size.x <spaceBounds.size.x && 10*goBounds.size.z < spaceBounds.size.z)
            {
                Vector3 max = spaceBounds.max;
                Vector3 center = spaceBounds.center;

                GameObject goInstance = Instantiate(go, new Vector3(center.x, 0, max.z - ((10 * goBounds.extents.z) + goBounds.extents.z*1.5f + spacing )), Quaternion.Euler(new Vector3(0,rotation)), transform.parent.transform);

                goInstance.transform.localScale *= 10;
                Vector3 newCenter = new Vector3(spaceBounds.center.x, spaceBounds.center.y, spaceBounds.center.z - (spacing + goBounds.extents.z * 10));
                Vector3 newSize = new Vector3(spaceBounds.size.x, spaceBounds.size.y, spaceBounds.size.z - (spacing + goBounds.size.z * 10));
                spaceBounds = new Bounds(newCenter, newSize); 
            } else {
                list.RemoveAt(randomIndex);
            }
        }
    }
}

