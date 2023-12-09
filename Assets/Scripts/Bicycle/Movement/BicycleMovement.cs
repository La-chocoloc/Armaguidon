using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(BoxCollider))]
public class BicycleMovement : MonoBehaviour
{ //MS Motorcycle test - Marcos Schultz (www.schultzgames.com)

    [SerializeField] WheelCollider frontWheel;
    [SerializeField] WheelCollider rearWheel;
    [SerializeField] GameObject meshFront;
    [SerializeField] GameObject handleBreakFront;
    [SerializeField] GameObject meshRear;
    [SerializeField] GameObject centerOfmassOBJ;
    Rigidbody ms_Rigidbody;

    float rbVelocityMagnitude;
    float horizontalInput;
    float verticalInput;
    float medRPM;

    void Awake()
    {
        transform.rotation = Quaternion.identity;
        ms_Rigidbody = GetComponent<Rigidbody>();

        //centerOfMass
        ms_Rigidbody.centerOfMass = transform.InverseTransformPoint(centerOfmassOBJ.transform.position);
        //
        //BoxCollider collider = GetComponent<BoxCollider>();
        //collider.size = new Vector3(0.5f, 1.0f, 3.0f);
    }

    void OnEnable()
    {
        WheelCollider WheelColliders = GetComponentInChildren<WheelCollider>();
        WheelColliders.ConfigureVehicleSubsteps(1000, 30, 30);
    }

    void FixedUpdate()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        medRPM = (frontWheel.rpm + rearWheel.rpm) / 2;
        rbVelocityMagnitude = ms_Rigidbody.velocity.magnitude;

        //motorTorque
        if (medRPM > 0)
        {
            rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 4.0f;
        }
        else
        {
            rearWheel.motorTorque = verticalInput * ms_Rigidbody.mass * 1.5f;
        }

        //steerAngle
        float nextAngle = horizontalInput * 45.0f;
        frontWheel.steerAngle = Mathf.Lerp(frontWheel.steerAngle, nextAngle, 0.125f);


        if (Mathf.Abs(rearWheel.rpm) > 50000)
        {
            rearWheel.motorTorque = 0.0f;
            rearWheel.brakeTorque = ms_Rigidbody.mass * 5;
        }

        if (rbVelocityMagnitude < 1.0f && Mathf.Abs(verticalInput) < 0.1f)
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = ms_Rigidbody.mass * 2.0f;
        }
        else
        {
            rearWheel.brakeTorque = frontWheel.brakeTorque = 0.0f;
        }

        Stabilizer();
    }

    void Update()
    {
        //update wheel meshes
        Vector3 temporaryVector;
        Quaternion temporaryQuaternion;
        
        frontWheel.GetWorldPose(out temporaryVector, out temporaryQuaternion);
        meshFront.transform.position = temporaryVector;
        meshFront.transform.rotation = temporaryQuaternion;
        
        rearWheel.GetWorldPose(out temporaryVector, out temporaryQuaternion);
        meshRear.transform.position = temporaryVector;
        meshRear.transform.rotation = temporaryQuaternion;
    }

    void Stabilizer()
    {
        Vector3 axisFromRotate = Vector3.Cross(transform.up, Vector3.up);
        Vector3 torqueForce = axisFromRotate.normalized * axisFromRotate.magnitude * 50;
        torqueForce.x = torqueForce.x * 0.4f;
        torqueForce -= ms_Rigidbody.angularVelocity;
        ms_Rigidbody.AddTorque(torqueForce * ms_Rigidbody.mass * 0.02f, ForceMode.Impulse);

        float rpmSign = Mathf.Sign(medRPM) * 0.02f;
        if (rbVelocityMagnitude > 1.0f && frontWheel.isGrounded && rearWheel.isGrounded)
        {
            ms_Rigidbody.angularVelocity += new Vector3(0, horizontalInput * rpmSign, 0);
        }
    }
}