using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is responsible for controlling the car with the WASD or Gamepad Controls.
/// </summary>
public class CarInputController : MonoBehaviour
{
    /// <summary>
    /// The colliders of the front wheels (for steering)
    /// </summary>
    public WheelCollider frontLeftCollider, frontRightCollider;
    public Transform frontLeftTransform, frontRightTransform;


    /// <summary>
    /// The colliders of the rear wheels (for thrust)
    /// </summary>
    public WheelCollider rearLeftCollider, rearRightCollider;
    public Transform rearLeftTransform, rearRightTransform;

    // Update is called once per frame
    void FixedUpdate()
    {
        // Get Steering
        var steering = Input.GetAxis("Horizontal");
        var gas = Input.GetAxis("Vertical");
        // Get Gas
        
        // Apply forces in NM 
        rearLeftCollider.motorTorque = gas * 200.0f;
        rearRightCollider.motorTorque = gas * 200.0f;
        
        // Steering
        frontLeftCollider.steerAngle = steering * 45.0f;
        frontRightCollider.steerAngle = steering * 45.0f;
        
        // Update Transforms
        //frontLeftTransform.rotation = frontLeftCollider.transform.rotation;
        //frontRightTransform.rotation = frontRightCollider.transform.rotation;
        //rearLeftTransform.rotation = rearLeftCollider.transform.rotation;
        //rearRightTransform.rotation = rearRightTransform.transform.rotation;
    }
    
}
