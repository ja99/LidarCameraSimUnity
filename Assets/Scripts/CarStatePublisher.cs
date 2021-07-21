using System;
using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.MissionManagement;
using RosMessageTypes.Std;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class CarStatePublisher : MonoBehaviour
{
    [Range(5, 60)] public float hz = 5f;

    public ROSConnection ros;

    private DateTime last;

    private uint sequenceCounter = 0;

    private Vector3 lastPos = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private Vector3 angularVelocity = Vector3.zero;
    private Vector3 lastRot = Vector3.zero;
    private Vector3 lastVelocity = Vector3.zero;
    private Vector3 acceleration = Vector3.zero;
    private Vector3 lastAcceleration = Vector3.zero;


    //ToDo: send real values
    private float steeringAngle = 0;
    public float brakePercentage = 0;
    public float torque = 0;

    private String topic = "/as/car_state";


    // Start is called before the first frame update
    void Start()
    {
        ros.RegisterPublisher(topic, "mission_management/CarState");
        last = DateTime.Now;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = transform.position;
        lastVelocity = velocity;
        velocity = (pos - lastPos) / Time.fixedDeltaTime;
        lastPos = pos;

        var rot = transform.rotation.eulerAngles;
        angularVelocity = (rot - lastRot) / Time.fixedDeltaTime;
        lastRot = rot;

        lastAcceleration = acceleration;
        acceleration = (velocity - lastVelocity) / Time.fixedDeltaTime;


        var now = DateTime.Now;
        if ((now - last).TotalSeconds >= (1 / hz))
        {
            last = now;

            SendToRos();
        }
    }

    void SendToRos()
    {
        float timeSinceStartup = Time.realtimeSinceStartup;
        uint secs = (uint) timeSinceStartup;
        uint ms = (uint) ((timeSinceStartup - secs) * 10e8);

        MTime time = new MTime(secs, ms);
        MHeader header = new MHeader(sequenceCounter, time, "map");


        MCarState state = new MCarState(
            header,
            SByte.Parse("3"),
            SByte.Parse("3"),
            SByte.Parse("3"),
            true,
            SByte.Parse("1"),
            velocity.z,
            -velocity.x,
            velocity.magnitude,
            -angularVelocity.y * Mathf.Deg2Rad,
            steeringAngle,
            0,
            acceleration.magnitude,
            // Todo: Real values
            acceleration_longitudinal: acceleration.magnitude,
            acceleration_lateral: 0
            );
        
        
        ros.Send(topic, state);

        sequenceCounter++;
    }
}