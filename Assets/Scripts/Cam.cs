using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using RosMessageTypes.Std;
using Unity.Collections;
using Unity.Robotics.ROSTCPConnector;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using System;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;

public class Cam : MonoBehaviour
{
    private Camera _camera;

    private ROSConnection ros;

    private DateTime last;

    //ToDo: Write ComputeShader instead
    private RenderTexture renderTexture;
    private Texture2D texture;

    public ComputeShader computeShader;
    private ComputeBuffer pixelsBuffer;
    // private uint[] pixels;
    // private float[] pixels;
    private Dat[] pixels;
    private ComputeBuffer debBuffer;
    

    private int numOfThreads = 64;

    [Header("Variables")] public string topicName = "pylon_ros_camera/image_raw";

    [Range(5, 60)] public float hz = 5f;

    public Vector2Int resolution = new Vector2Int(1900, 1200);

    private uint sequenceCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        _camera = GetComponent<Camera>();

        ros = ROSConnection.instance;
        ros.RegisterPublisher(topicName, "sensor_msgs/Image");


        renderTexture = new RenderTexture(resolution.x, resolution.y, 16);
        texture = new Texture2D(resolution.x, resolution.y);

        // pixels = new uint[resolution.x * resolution.y * 3];
        pixels = new Dat[resolution.x * resolution.y * 3];
        // pixelsBuffer = new ComputeBuffer(pixels.Length, sizeof(uint));
        pixelsBuffer = new ComputeBuffer(pixels.Length, sizeof(float));
        computeShader.SetBuffer(0, "Pixels", pixelsBuffer);
        computeShader.SetTexture(0, "InputTexture", renderTexture);
        computeShader.SetFloat("xResolution", resolution.x);
        computeShader.SetFloat("yResolution", resolution.y);

        debBuffer = new ComputeBuffer(resolution.x * resolution.y, sizeof(int) * 2);
        computeShader.SetBuffer(0, "Deb", debBuffer);

        last = DateTime.Now;
    }


    private void Update()
    {
        var now = DateTime.Now;
        if ((now - last).TotalSeconds >= (1 / hz))
        {
            _camera.Render();
            last = now;
        }
    }

    private void OnPostRender()
    {
        SendOfToRosBurstCompiled();
        // SendOfToRosShader();
    }

    void SendOfToRosBurstCompiled()
    {
        float timeSinceStartup = Time.realtimeSinceStartup;
        uint secs = (uint) timeSinceStartup;
        uint ms = (uint) ((timeSinceStartup - secs) * 10e8);

        MTime time = new MTime(secs, ms);
        MHeader header = new MHeader(sequenceCounter, time, "map");

        texture.ReadPixels(new Rect(0, 0, resolution.x, resolution.y), 0, 0, false);
        texture.Apply();


        uint step = (uint) resolution.y * 3; //3 because there are 3 bytes in rgb8

        var data = ScheduleJob(texture.GetPixels(0, 0, resolution.x, resolution.y));

        var img = new RosMessageTypes.Sensor.MImage(header, (uint) resolution.y, (uint) resolution.x, "rgb8",
            1 /*no idea*/, step, data);


        ros.Send(topicName, img);

        sequenceCounter++;
    }

    //ToDo: fix the shader
    void SendOfToRosShader()
    {
        computeShader.Dispatch(0, Mathf.CeilToInt((resolution.x * resolution.y) / (float) numOfThreads), 1, 1);

        float timeSinceStartup = Time.realtimeSinceStartup;
        uint secs = (uint) timeSinceStartup;
        uint ms = (uint) ((timeSinceStartup - secs) * 10e8);

        MTime time = new MTime(secs, ms);
        MHeader header = new MHeader(sequenceCounter, time, "map");

        uint step = (uint) resolution.y * 3; //3 because there are 3 bytes in rgb8

        pixelsBuffer.GetData(pixels);

        Vector2Int[] deb = new Vector2Int[resolution.x* resolution.y];
        
        debBuffer.GetData(deb);
        
        
        
        

        byte[] data = new byte[pixels.Length];

        for (int i = 0; i < pixels.Length; i++)
        {
            data[i] = (byte) pixels[i].val;
        }


        var img = new RosMessageTypes.Sensor.MImage(header, (uint) resolution.y, (uint) resolution.x, "rgb8",
            1 /*no idea*/, step, data);


        ros.Send(topicName, img);

        sequenceCounter++;
    }

    
    
    byte[] ScheduleJob(Color[] colorArray)
    {
        NativeArray<Color> pixels = new NativeArray<Color>(colorArray.Length, Allocator.TempJob);

        NativeArray<byte> data = new NativeArray<byte>(colorArray.Length * 3, Allocator.TempJob);


        pixels.CopyFrom(colorArray);

        MyParallelJob jobData = new MyParallelJob();
        jobData.pixels = pixels;
        jobData.data = data;

// Schedule the job with one Execute per index in the results array and only 1 item per processing batch
        JobHandle handle = jobData.Schedule(pixels.Length, 20);

// Wait for the job to complete
        handle.Complete();

        byte[] returnData = new byte[data.Length];
        data.CopyTo(returnData);

// Free the memory allocated by the arrays
        pixels.Dispose();
        data.Dispose();

        return returnData;
    }


    [BurstCompile]
    public struct MyParallelJob : IJobParallelFor
    {
        [ReadOnly] public NativeArray<Color> pixels;

        [WriteOnly] [NativeDisableParallelForRestriction]
        public NativeArray<byte> data;


        public void Execute(int i)
        {
            data[i * 3] = (byte) (uint) Mathf.Lerp(0, 255, pixels[i].r);
            data[i * 3 + 1] = (byte) (uint) Mathf.Lerp(0, 255, pixels[i].g);
            data[i * 3 + 2] = (byte) (uint) Mathf.Lerp(0, 255, pixels[i].b);
        }
    }
}

public struct Dat
{
    public uint val;
}