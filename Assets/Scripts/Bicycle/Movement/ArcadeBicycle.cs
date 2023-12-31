using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class ArcadeBicycle : MonoBehaviour
{
    [SerializeField] GameObject handleBar;
    [SerializeField] GameObject bicycleVisuals;
    [SerializeField] float maxWheelTurningAngle;
    [SerializeField] float wheelTurningSpeed;
    [SerializeField] float maxBicycleTurningAngle;
    [SerializeField] float bicycleTurningSpeed;
    [SerializeField] float accelForce;
    [SerializeField] float maxSpeed;


    Rigidbody rb;

    float horizontalInput = 0;
    float verticalInput = 0;
    float currentAccel = 0;
    float currentVelocity = 0;

    float currentWheelAngle = 0;
    float currentBicycleAngle = 0;

    bool boosted = false;
    public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.identity;
        rb = GetComponent<Rigidbody>();
        Vector3 centerOfMass = GetComponent<BoxCollider>().center;
        rb.centerOfMass = centerOfMass;
        foreach (Rigidbody childRb in bicycleVisuals.GetComponentsInChildren<Rigidbody>())
        {
            childRb.centerOfMass = transform.position - childRb.transform.position + centerOfMass;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (dead)
            return;

        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // No use of deltaTime since we want arcade physic (so not realistic)
        currentAccel = Mathf.Clamp(
            accelForce * verticalInput, 
            0, 
            accelForce
        );

        // Use of deltaTime so bicycle's velocity changes are smooth
        currentVelocity = Mathf.Clamp(
            currentVelocity * 0.8f + currentAccel * maxSpeed * Time.fixedDeltaTime, 
            -maxSpeed, 
            maxSpeed
        );

        // Add force to move forward
        rb.MovePosition(transform.position + transform.forward * currentVelocity);
        bicycleVisuals.transform.position = rb.position;

        currentWheelAngle = Mathf.Clamp(
             currentWheelAngle * 0.8f + maxWheelTurningAngle * horizontalInput * Time.fixedDeltaTime * wheelTurningSpeed,
            -maxWheelTurningAngle,
            maxWheelTurningAngle
        );

        currentBicycleAngle = Mathf.Clamp(
            currentBicycleAngle * 0.8f + maxBicycleTurningAngle * horizontalInput * Time.fixedDeltaTime * bicycleTurningSpeed,
            -maxBicycleTurningAngle,
            maxBicycleTurningAngle
        );

        bicycleVisuals.transform.localRotation = Quaternion.Euler(0, 0, -currentBicycleAngle);
        handleBar.transform.localRotation = Quaternion.Euler(0,currentWheelAngle,0);



        transform.rotation = Quaternion.Euler(
            transform.rotation.eulerAngles + new Vector3(
                0,
                currentWheelAngle*currentAccel, 
                0
            ) * Time.fixedDeltaTime
         );
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ennemy")
        {
            Debug.Log("Should explode");
            Explode();
        }
        if (collision.gameObject.tag == "Boost")
        {
            setBoost(true);
        }
        if (collision.gameObject.tag == "Terrain")
        {
            if (boosted)
            {
                setBoost(false);
            }
        }
    }

    private void Explode()
    {
        dead = true;
        GetComponent<BoxCollider>().enabled = false;
        rb.isKinematic = true;
        foreach (TrailRenderer childTr in bicycleVisuals.GetComponentsInChildren<TrailRenderer>())
        {
            childTr.enabled = false;
        }
        foreach (Rigidbody childRb in bicycleVisuals.GetComponentsInChildren<Rigidbody>())
        {
            float randX = Random.value - 0.5f;
            float randY = Random.value - 0.5f;
            float randZ = Random.value - 0.5f;
            childRb.GetComponent<Collider>().isTrigger = false;
            childRb.centerOfMass = new Vector3();
            childRb.useGravity = true;
            childRb.isKinematic = false;
            childRb.AddForce(new Vector3(randX, randY, randZ).normalized * 1000f);
        }
    }

    void setBoost(bool isBoosted)
    {
        boosted = isBoosted;
        if(isBoosted)
        {
            maxSpeed *= 2.5f;
        }
        else
        {
            maxSpeed *= 0.4f;
        }
    }
}
