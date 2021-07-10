using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RosMessageTypes.Std;
using Unity.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System.Threading;
using RosMessageTypes.Sensor;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEditor;
// using Newtonsoft.Json;

public class Cam : MonoBehaviour
{
    private Camera _camera;

    public ROSConnection ros;

    private DateTime last;

    //ToDo: Write ComputeShader instead
    private RenderTexture renderTexture;
    private Texture2D texture;

    public ComputeShader computeShader;
    private ComputeBuffer pixelsBuffer;
    private Dat[] pixels;


    private int numOfThreads;

    [Header("Variables")] public string topicName = "pylon_camera_node/image_raw";

    [Range(5, 60)] public float hz = 5f;

    public Vector2Int resolution = new Vector2Int(1900, 1200);

    private uint sequenceCounter = 0;

    private Stopwatch stopwatch;


    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        //ros = ROSConnection.instance;
        ros.RegisterPublisher(topicName, "sensor_msgs/Image");
        


        renderTexture = new RenderTexture(resolution.x, resolution.y, 16);
        texture = new Texture2D(resolution.x, resolution.y);
        _camera.targetTexture = renderTexture;


        pixels = new Dat[resolution.x * resolution.y * 3];
        pixelsBuffer = new ComputeBuffer(pixels.Length, sizeof(uint));
        computeShader.SetBuffer(0, "Pixels", pixelsBuffer);
        computeShader.SetTexture(0, "InputTexture", renderTexture);
        computeShader.SetFloat("xResolution", resolution.x);
        computeShader.SetFloat("yResolution", resolution.y);


        uint x = 0;
        uint throwaway = 0;
        computeShader.GetKernelThreadGroupSizes(0, out x, out throwaway, out throwaway);
        numOfThreads = (int) x;


        last = DateTime.Now;
    }

   

    private void OnPostRender()
    {
        var now = DateTime.Now;
        if ((now - last).TotalSeconds >= (1 / hz))
        {
            last = now;

            SendOfToRosShader();
        }
    }

    private void OnDestroy()
    {
        pixelsBuffer.Dispose();
    }


    //ToDo: fix the shader
    void SendOfToRosShader()
    {
        float timeSinceStartup = Time.realtimeSinceStartup;
        uint secs = (uint) timeSinceStartup;
        uint ms = (uint) ((timeSinceStartup - secs) * 10e8);

        MTime time = new MTime(secs, ms);
        MHeader header = new MHeader(sequenceCounter, time, "map");

        uint step = (uint) resolution.y * 3; //3 because there are 3 bytes in rgb8


        RunShader();


        byte[] data = new byte[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            data[i] = (byte) pixels[i].val;
        }


        var img = new RosMessageTypes.Sensor.MImage(header, (uint) resolution.y, (uint) resolution.x, "rgb8",
            1, step, data);


        // ros.Send(topicName, img);

        Thread sendThread = new Thread(() => SendThread(img));
        sendThread.Start();

        // SendThread(img);

        sequenceCounter++;
    }

    void SendThread(MImage img)
    {
        ros.Send(topicName, img);
    }

    void RunShader()
    {
        computeShader.Dispatch(0, Mathf.CeilToInt((resolution.x * resolution.y) / (float) numOfThreads), 1, 1);
        pixelsBuffer.GetData(pixels);
    }
}

public struct Dat
{
    public uint val;
}

[Serializable]
public struct ConePoint
{
    public double x { get; set; }
    public double y { get; set; }
    public string cone_type { get; set; }
    public double probability { get; set; }
}