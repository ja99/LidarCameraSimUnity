using System;
using System.Text;
using Communication.Messages;
using UnityEngine;

namespace Car.Camera {

    public class CarCameraController : MonoBehaviour {
        /// <summary>
        /// The dimensions of the image
        /// </summary>
        private const int WIDTH = 1900, HEIGHT = 1200;

        /// <summary>
        /// The necessary buffer size
        /// </summary>
        private const int BUFFER_SIZE = 3 * WIDTH * HEIGHT;

        /// <summary>
        /// The number of images created per second
        /// </summary>
        private const float HZ = 50f;

        /// <summary>
        /// The duration of a ray
        /// </summary>
        [Range(0.01f, 1f)] public float rayDuration = 0.5f;

        /// <summary>
        /// The event that is triggered after a new image was generated
        /// </summary>
        public Action<byte[]> OnNewImage;

        /// <summary>
        /// The camera object
        /// </summary>
        public UnityEngine.Camera carCamera;

        /// <summary>
        /// The cam compute shader
        /// </summary>
        public ComputeShader computeShader;

        /// <summary>
        /// The buffer for generating images
        /// </summary>
        private ComputeBuffer _pixelsBuffer;

        /// <summary>
        /// Last time a picture was generated
        /// </summary>
        private DateTime _last;

        /// <summary>
        /// The number of threads used for the buffer
        /// </summary>
        private int _numOfThreads;

        /// <summary>
        /// Initialize the car camera
        /// </summary>
        private void Start() {
            // Connect the camera with the compute shader
            _pixelsBuffer = new ComputeBuffer(BUFFER_SIZE, sizeof(uint));
            var renderTexture = new RenderTexture(WIDTH, HEIGHT, 16);
            carCamera.targetTexture = renderTexture;
            computeShader.SetTexture(0, "InputTexture", renderTexture);

            // Set up the compute shader
            computeShader.SetBuffer(0, "Pixels", _pixelsBuffer);
            computeShader.SetFloat("xResolution", WIDTH);
            computeShader.SetFloat("yResolution", HEIGHT);
            computeShader.GetKernelThreadGroupSizes(0, out var x, out _, out _);
            _numOfThreads = (int) x;
            _last = DateTime.Now;
        }

        /// <summary>
        /// Clean up the pixel buffer
        /// </summary>
        private void OnDestroy() {
            _pixelsBuffer.Dispose();
        }

        /// <summary>
        /// Generate the images
        /// </summary>
        private void OnPostRender() {
            var now = DateTime.Now;
            if ((now - _last).TotalSeconds < 1f / HZ) return;
            _last = now;
            GenerateImageData();
        }

        /// <summary>
        /// Generate a new image
        /// </summary>
        private void GenerateImageData() {
            var imageData = new uint[BUFFER_SIZE];
            computeShader.Dispatch(0, Mathf.CeilToInt(WIDTH * HEIGHT / (float) _numOfThreads), 1, 1);
            _pixelsBuffer.GetData(imageData);
            var data = new byte[BUFFER_SIZE];
            for (var i = 0; i < BUFFER_SIZE; i++) data[i] = (byte) imageData[i];
            OnNewImage?.Invoke(data);
        }

        /// <summary>
        /// Draw rays for the perceived cones
        /// </summary>
        /// <param name="perceivedConesMessage">The perceived cones</param>
        public void DrawConeRays(PerceivedConesMessage perceivedConesMessage) {
            foreach (var cone in perceivedConesMessage.cones) {
                var ray = carCamera.ScreenPointToRay(new Vector2(
                    (float) cone.x,
                    YFlip((float) cone.y, carCamera.pixelHeight)
                ));
                var c = cone.cone_type switch {
                    "blue_cone" => Color.blue,
                    "yellow_cone" => Color.yellow,
                    _ => Color.red
                };
                Debug.DrawRay(ray.origin, ray.direction * 100, c, rayDuration);
            }
        }

        /// <summary>
        /// Y-Flip
        /// </summary>
        private static float YFlip(float y, float screenHeight) {
            var t = Mathf.InverseLerp(screenHeight, 0, y);
            return Mathf.Lerp(0, screenHeight, t);
        }
    }
}