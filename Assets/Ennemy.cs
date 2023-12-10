using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ennemy : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float accelForce;
    [SerializeField] float maxSpeed;

    private float currentAccel = 0;
    private float currentVelocity = 0;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 dir = target.position - transform.position;

        currentAccel = accelForce * Mathf.Clamp(Vector3.Dot(dir,transform.forward),0.1f,1);

        currentVelocity = Mathf.Clamp(
            currentVelocity * 0.8f + currentAccel * maxSpeed * Time.fixedDeltaTime,
            -maxSpeed,
            maxSpeed
        );

        rb.MovePosition(
            rb.position + transform.forward * currentVelocity * Mathf.Clamp(Mathf.Sin(Time.time)+0.3f,0.4f, 1.2f));

        float angle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);

        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles + new Vector3(
                0,
                angle,
                0
            ) * Time.fixedDeltaTime
        );
    }
}
