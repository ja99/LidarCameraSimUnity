using System;
using System.Collections;
using System.Collections.Generic;
using RosMessageTypes.MissionManagement;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public ROSConnection ros;

    private String topic = "/as/control_result";

    private MControlResult controlState;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        ros.RegisterSubscriber(topic, "mission_management/ControlResult");
        ros.Subscribe<MControlResult>(topic, Callback);
    }

    void Callback(MControlResult msg)
    {
        controlState = msg;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.Rotate(0,controlState.steering_angle_target,0);
        transform.position += transform.forward * (controlState.speed_target * Time.fixedDeltaTime);
    }
}


