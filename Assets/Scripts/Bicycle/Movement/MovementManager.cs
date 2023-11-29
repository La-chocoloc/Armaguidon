using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementManager : MonoBehaviour
{
    #region Serialize fields
    [SerializeField]
    private float speed = 20;
    [SerializeField] private float rotationSpeed = 25;

    [SerializeField] private float maxSpeed = 50;
    [SerializeField][Range(-40f, 40f)] private float angle = 0;

    [SerializeField] GameObject frontTyre;    
    [SerializeField] GameObject rearTyre;
    [SerializeField] GameObject handleBar;
    [SerializeField] GameObject pivotDirection;
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
        body.velocity = transform.forward * verticalAccel * speed * Time.fixedDeltaTime;
        transform.Rotate(transform.up * horizontalAccel * rotationSpeed * Time.fixedDeltaTime);

        if (verticalAccel != 0)
        {
            frontTyre.transform.Rotate(new Vector3(0, 0, verticalAccel));
            rearTyre.transform.Rotate(new Vector3(0, 0, verticalAccel));
        }

        MoveHandleBar();
    }

    void MoveHandleBar()
    {
        handleBar.transform.rotation = Quaternion.Euler(0, 180, 0);
        handleBar.transform.Rotate(pivotDirection.transform.position - handleBar.transform.position, angle, Space.World);
    }
}
