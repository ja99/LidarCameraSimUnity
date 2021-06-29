using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RosMessageTypes.Geometry;
using Unity.Collections;
using Unity.Jobs;
using Unity.Robotics.ROSTCPConnector;
using Unity.VisualScripting;
using UnityEditor.Experimental;
using UnityEngine;
using RosMessageTypes.Sensor;
using RosMessageTypes.Std;
using Unity.Burst;

public class Lidar : MonoBehaviour
{
    public ROSConnection ros;
    
    public string topicName = "velodyne";

    [Header("Variables")] public int layers = 16;

    [Tooltip("in degrees")] public float verticalAngleRange = 30f;

    [Range(5, 20)] public float hz = 5f;

    [Range(0.1f, 0.4f)] public float horizontalResolution = 0.4f;

    private DateTime last;

    private uint sequenceCounter = 0;

    private NativeArray<RaycastCommand> commands;
    private RaycastCommand[] commandsArray;

    private NativeArray<RaycastHit> results;

    // Start is called before the first frame update
    void Start()
    {
        ros = ROSConnection.instance;
        ros.RegisterPublisher(topicName, "sensor_msgs/PointCloud");
        last = DateTime.Now;


        commands = CreateCommands();
        commandsArray = new RaycastCommand[commands.Length];
        commands.CopyTo(commandsArray);
        results = new NativeArray<RaycastHit>(commands.Length, Allocator.TempJob);
    }

    // Update is called once per frame
    void Update()
    {
        var now = DateTime.Now;
        if ((now - last).TotalSeconds >= (1 / hz))
        {

            // var stopwatch = Stopwatch.StartNew();
            last = now;
            
            UpdateCommands();
           
            PerformRaycast();
            
            SendOfToRos();
            
            // print(stopwatch.ElapsedMilliseconds);

        }
    }
    
    private void OnDestroy()
    {
        commands.Dispose();
        results.Dispose();
    }

    void SendOfToRos()
    {
        float timeSinceStartup = Time.realtimeSinceStartup;
        uint secs = (uint) timeSinceStartup;
        uint ms = (uint) ((timeSinceStartup - secs) * 10e8);

        MTime time = new MTime(secs, ms);
        MHeader header = new MHeader(sequenceCounter, time, "map");

        MPoint32[] point32s = new MPoint32[results.Length];
        var channelVals = new float[results.Length];
       
        for (int i = 0; i < results.Length; i++)
        {
            var resPoint = results[i].point -transform.position;
            
            //ToDo: unmirror when you fixed the camera
            // var p = new MPoint32(resPoint.z, -resPoint.x, resPoint.y);
            var p = new MPoint32(resPoint.z, resPoint.x, resPoint.y);
            point32s[i] = p;
            channelVals[i] = 1;
        }

        var channel = new MChannelFloat32("intensity", channelVals);
        var channels = new MChannelFloat32[] {channel};
        
        
        MPointCloud pointCloud = new MPointCloud(header, point32s, channels);
        
        ros.Send(topicName, pointCloud);

        sequenceCounter++;
    }
    

    void PerformRaycast()
    {
        // Schedule the batch of raycasts
        JobHandle handle = RaycastCommand.ScheduleBatch(commands, results, 64, default(JobHandle));

        // Wait for the batch processing job to complete
        handle.Complete();
    }


    NativeArray<RaycastCommand> CreateCommands()
    {
        List<RaycastCommand> result = new List<RaycastCommand>();
        for (float horizontal = 0; horizontal < 360f; horizontal += horizontalResolution)
        {
            for (float vertical = -(verticalAngleRange * 0.5f);
                vertical <= (verticalAngleRange * 0.5f);
                vertical += (verticalAngleRange / layers))
            {
                var r = transform.forward;
                r = Quaternion.Euler(vertical, 0, 0) * r;
                r = Quaternion.Euler(0, horizontal, 0) * r;

                var c = new RaycastCommand(transform.position, r);
                result.Add(c);
            }
        }

        var returnArray = new NativeArray<RaycastCommand>(result.Count, Allocator.TempJob);
        returnArray.CopyFrom(result.ToArray());
        return returnArray;
    }

    void UpdateCommands()
    {
        for (int i = 0; i < commands.Length; i++)
        {
            commandsArray[i].from = transform.position;
        }
        commands.CopyFrom(commandsArray);
    }


}