using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WheelManager : MonoBehaviour
{
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider frontWheelCollider;
        public WheelCollider rearWheelCollider;
        public GameObject frontWheelMesh;
        public GameObject rearWheelMesh;
        public bool motor;
        public bool steering;
    }

    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float brakeTorque;
    public float decelerationForce;

    public void ApplyLocalPositionToVisuals(AxleInfo axleInfo)
    {
        Vector3 position;
        Quaternion rotation;
        axleInfo.frontWheelCollider.GetWorldPose(out position, out rotation);
        axleInfo.frontWheelMesh.transform.position = position;
        axleInfo.frontWheelMesh.transform.rotation = rotation;
        axleInfo.rearWheelCollider.GetWorldPose(out position, out rotation);
        axleInfo.rearWheelMesh.transform.position = position;
        axleInfo.rearWheelMesh.transform.rotation = rotation;
    }

    void FixedUpdate()
    {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        for (int i = 0; i < axleInfos.Count; i++)
        {
            if (axleInfos[i].steering)
            {
                Steering(axleInfos[i], steering);
            }
            if (axleInfos[i].motor)
            {
                Acceleration(axleInfos[i], motor);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                Brake(axleInfos[i]);
            }
            ApplyLocalPositionToVisuals(axleInfos[i]);
        }
    }

    private void Acceleration(AxleInfo axleInfo, float motor)
    {
        if (motor != 0f)
        {
            axleInfo.frontWheelCollider.brakeTorque = 0;
            axleInfo.rearWheelCollider.brakeTorque = 0;
            axleInfo.frontWheelCollider.motorTorque = motor;
            axleInfo.rearWheelCollider.motorTorque = motor;
        }
        else
        {
            Deceleration(axleInfo);
        }
    }

    private void Deceleration(AxleInfo axleInfo)
    {
        axleInfo.frontWheelCollider.brakeTorque = decelerationForce;
        axleInfo.rearWheelCollider.brakeTorque = decelerationForce;
    }

    private void Steering(AxleInfo axleInfo, float steering)
    {
        axleInfo.frontWheelCollider.steerAngle = steering;
        axleInfo.rearWheelCollider.steerAngle = steering;
    }

    private void Brake(AxleInfo axleInfo)
    {
        axleInfo.frontWheelCollider.brakeTorque = brakeTorque;
        axleInfo.rearWheelCollider.brakeTorque = brakeTorque;
    }
}
