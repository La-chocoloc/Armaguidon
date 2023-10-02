using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    #region Serialize fields
    [SerializeField]
    private float max_speed;
    [SerializeField]
    private float horizontal_accel;
    [SerializeField]
    private float vertical_accel;
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.forward*Time.deltaTime;
    }
}
