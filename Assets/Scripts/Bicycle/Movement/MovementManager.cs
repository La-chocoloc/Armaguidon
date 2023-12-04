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
    [SerializeField] GameObject pivotDirection;
    #endregion

    private Rigidbody body;

    private float horizontalAccel = 0;

    private float verticalAccel = 0;

    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    private void OnValidate()
    {
        MoveHandleBar();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(handleBar.transform.position, pivotDirection.transform.position);
    }

    void FixedUpdate()
    {
        verticalAccel = Input.GetAxis("Vertical");
        horizontalAccel = Input.GetAxis("Horizontal");
        body.velocity = handleBar.transform.forward * verticalAccel * speed * Time.fixedDeltaTime;

        if (verticalAccel != 0)
        {
            frontTyre.transform.Rotate(new Vector3(verticalAccel, 0, 0));
            rearTyre.transform.Rotate(new Vector3(verticalAccel, 0, 0));
        }

        MoveHandleBar();
    }
    void MoveHandleBar()
    {
        handleBar.transform.rotation = Quaternion.Euler(0, 180, 0);
        handleBar.transform.Rotate(pivotDirection.transform.position - handleBar.transform.position, angle, Space.World);
    }
}
