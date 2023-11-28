using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    #region Serialize fields
    [SerializeField]
    private float speed = 20;
    [SerializeField]
    private float rotationSpeed = 25;

    [SerializeField]
    private float maxSpeed = 50;

    [SerializeField]
    GameObject frontWheel;
    [SerializeField]
    GameObject rearWheel;
    #endregion

    private Rigidbody body;

    private float horizontalAccel = 0;
    private float horizontalVelocity = 0;

    private float verticalAccel = 0;
    private float verticalVelocity = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        verticalAccel = Input.GetAxis("Vertical");
        horizontalAccel = Input.GetAxis("Horizontal");
        body.velocity = transform.forward * verticalAccel * speed * Time.fixedDeltaTime;
        transform.Rotate(transform.up * horizontalAccel * rotationSpeed * Time.fixedDeltaTime);

        if (verticalAccel != 0)
        {
            frontWheel.transform.Rotate(new Vector3(0, 0, verticalAccel));
            rearWheel.transform.Rotate(new Vector3(0, 0, verticalAccel));
        }        
        
        if (horizontalAccel != 0)
        {
            frontWheel.transform.Rotate(new Vector3(0, horizontalAccel, 0));
        }
    }
}
