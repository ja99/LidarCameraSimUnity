using System;
using System.Collections.Generic;
using System.Text;
using Car;
using Car.Camera;
using Car.Lidar;
using Communication.Messages;
using UnityEngine;

namespace Communication {
    public class InterfaceController : MonoBehaviour {
        /// <summary>
        /// The important topics
        /// </summary>
        private const int
            CAMERA = 42000,
            LIDAR = 42001,
            CAR_STATE = 42002,
            CONTROL_RESULT = 42003,
            PERCEIVED_CONES = 42004;

        /// <summary>
        /// The car controller
        /// </summary>
        public CarController carController;

        /// <summary>
        /// The car camera controller
        /// </summary>
        public CarCameraController carCameraController;

        /// <summary>
        /// The lidar controller
        /// </summary>
        public LidarController lidarController;

        /// <summary>
        /// The publishers
        /// </summary>
        private Publisher _cameraPublisher, _lidarPublisher, _carStatePublisher;

        /// <summary>
        /// The subscribers
        /// </summary>
        private Subscriber _controlTargetSubscriber, _perceivedConesSubscriber;

        /// <summary>
        /// The queue containing the actions that shall be executed on the main thread
        /// </summary>
        private readonly Queue<Action> _mainThreadQueue = new Queue<Action>();

        /// <summary>
        /// Initialize the server
        /// </summary>
        private void Start() {
            // Init the publishers
            _cameraPublisher = new Publisher(CAMERA);
            _lidarPublisher = new Publisher(LIDAR);
            _carStatePublisher = new Publisher(CAR_STATE);
            _controlTargetSubscriber = new Subscriber(CONTROL_RESULT, OnControlTarget);
            _perceivedConesSubscriber = new Subscriber(PERCEIVED_CONES, OnPerceivedCones);

            // Register the sensor events
            carCameraController.OnNewImage = imageData => _cameraPublisher.Publish(imageData);
            lidarController.OnNewPointCloud = pointCloudData => _lidarPublisher.Publish(pointCloudData);
            carController.OnNewCarState = message => {
                var json = JsonUtility.ToJson(message);
                _carStatePublisher.Publish(Encoding.UTF8.GetBytes(json));
            };
        }

        /// <summary>
        /// Execute the main thread actions
        /// </summary>
        private void FixedUpdate() {
            lock (_mainThreadQueue) {
                while (_mainThreadQueue.Count > 0) {
                    _mainThreadQueue.Dequeue()();
                }
            }
        }

        /// <summary>
        /// Adds a function to the queue that is executed
        /// on the main thread with every update
        /// </summary>
        /// <param name="action">The action</param>
        private void ExecuteOnMainThread(Action action) {
            lock (_mainThreadQueue) {
                _mainThreadQueue.Enqueue(action);
            }
        }

        /// <summary>
        /// Handle incoming control target
        /// </summary>
        private void OnControlTarget(byte[] msg) {
            var controlResult = JsonUtility.FromJson<ControlResultMessage>(Encoding.UTF8.GetString(msg));
            ExecuteOnMainThread(() => carController.ApplyControlResult(controlResult));
        }

        /// <summary>
        /// Handle incoming control target
        /// </summary>
        private void OnPerceivedCones(byte[] msg) {
            var perceivedCones = JsonUtility.FromJson<PerceivedConesMessage>(Encoding.UTF8.GetString(msg));
            ExecuteOnMainThread(() => carCameraController.DrawConeRays(perceivedCones));
        }

        /// <summary>
        /// Shut down the publishers and subscribers
        /// </summary>
        private void OnDestroy() {
            _cameraPublisher.Stop();
            _lidarPublisher.Stop();
            _carStatePublisher.Stop();
            _controlTargetSubscriber.Stop();
            _perceivedConesSubscriber.Stop();
        }
    }
}