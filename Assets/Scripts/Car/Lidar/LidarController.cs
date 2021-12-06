using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Car.Lidar {
    public class LidarController : MonoBehaviour {
        /// <summary>
        /// The scan frequency of the lidar
        /// </summary>
        private const float HZ = 15f;

        /// <summary>
        /// The amount of layers the lidar shall have
        /// </summary>
        [Header("Variables")] public int layers = 16;

        /// <summary>
        /// The vertical angle range of the lidar
        /// </summary>
        [Tooltip("in degrees")] public float verticalAngleRange = 30f;

        /// <summary>
        /// The resolution for each layer
        /// </summary>
        [Range(0.1f, 0.4f)] public float horizontalResolution = 0.4f;

        /// <summary>
        /// The event that is triggered after a new point cloud was generated
        /// </summary>
        public Action<byte[]> OnNewPointCloud;

        /// <summary>
        /// The list of raycast commands
        /// </summary>
        private NativeArray<RaycastCommand> _commands;

        /// <summary>
        /// The list containing the results of the raycast
        /// </summary>
        private NativeArray<RaycastHit> _results;

        /// <summary>
        /// Last time a picture was generated
        /// </summary>
        private DateTime _last;
        
        // Generate the byte array (12 bytes = 3 * 4 bytes and 4 bytes = 1 float)
        private byte[] data;
        private float[] values = new float[3];

        /// <summary>
        /// Initialize the lidar
        /// </summary>
        private void Start() {
            _last = DateTime.Now;
            CreateCommands();
            _results = new NativeArray<RaycastHit>(_commands.Length, Allocator.TempJob);
            data = new byte[12 * _results.Length];
        }

        /// <summary>
        /// Generate the lidar data
        /// </summary>
        private void Update() {
            var now = DateTime.Now;
            if ((now - _last).TotalSeconds < 1f / HZ) return;
            _last = now;
            GenerateLidarData();
        }

        /// <summary>
        /// Clean up
        /// </summary>
        private void OnDestroy() {
            _commands.Dispose();
            _results.Dispose();
        }

        /// <summary>
        /// Create the raycast commands
        /// </summary>
        private void CreateCommands() {
            var result = new List<RaycastCommand>();
            
            
            for (float horizontal = 0; horizontal < 360f; horizontal += horizontalResolution) {
                for (var vertical = -(verticalAngleRange * 0.5f);
                    vertical <= verticalAngleRange * 0.5f;
                    vertical += verticalAngleRange / layers) {
                    // Calculate the direction vector
                    var r = transform.forward;
                    r = Quaternion.Euler(vertical, 0, 0) * r;
                    r = Quaternion.Euler(0, horizontal, 0) * r;

                    // Create the raycast command
                    var c = new RaycastCommand(transform.position, r);
                    result.Add(c);
                }
            }

            _commands = new NativeArray<RaycastCommand>(result.Count, Allocator.TempJob);
            _commands.CopyFrom(result.ToArray());
        }

        /// <summary>
        /// Generate a new point cloud
        /// </summary>
        private void GenerateLidarData() {
            // Update position information for each command
            for (var i = 0; i < _commands.Length; i++) {
                var command = _commands[i];
                command.from = transform.position;
                _commands[i] = command;
            }

            // Schedule the batch of raycasts
            var handle = RaycastCommand.ScheduleBatch(_commands, _results, 64);
            handle.Complete();

            
            for (var i = 0; i < _results.Length; i++) {
                var resPoint = _results[i].point - transform.position;
                values[0] = resPoint.z;
                values[1] = -resPoint.x;
                values[2] = resPoint.y;
                
                for (var j = 0; j < values.Length; j++) {
                    var bytes = BitConverter.GetBytes(values[j]);
                    for (var k = 0; k < bytes.Length; k++) data[12 * i + 4 * j + k] = bytes[k];
                }

            }
            OnNewPointCloud?.Invoke(data);
        }
    }
}