using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    #region Serialize fields
    [SerializeField]
    private float speed = 200;
    [SerializeField] private float rotationSpeed = 150;

    [SerializeField][Range(-40f, 40f)] private float angle = 0;

    [SerializeField] GameObject frontTyre;    
    [SerializeField] GameObject rearTyre;
    [SerializeField] Transform frontTyreTransform;    
    [SerializeField] Transform rearTyreTransform;
    [SerializeField] GameObject handleBar;
    #endregion

    private Rigidbody body;

    private float horizontalAccel = 0;

    private float verticalAccel = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        verticalAccel = Input.GetAxis("Vertical");
        horizontalAccel = Input.GetAxis("Horizontal");
        body.velocity = handleBar.transform.forward * verticalAccel * speed * Time.fixedDeltaTime;
        body.AddForce(new Vector3(0, -980f, 0));

        if (verticalAccel != 0)
        {
            frontTyre.transform.Rotate(new Vector3(verticalAccel, 0, 0));
            rearTyre.transform.Rotate(new Vector3(verticalAccel, 0, 0));
        }

        MoveHandleBar();
    }
    void MoveHandleBar()
    {
        handleBar.transform.Rotate(new Vector3(0, horizontalAccel, 0));
    }
}
