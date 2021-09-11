using UnityEngine;

namespace Car.Camera {
    public class CameraExternalCalibration : MonoBehaviour {
        /// <summary>
        /// Calibrate the camera
        /// </summary>
        private void Start() {
            for (var x = 6; x <= 16; x += 5) {
                for (var y = -3; y <= 3; y += 3) {
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.localScale = Vector3.one * 0.1f;
                    var position = transform.position;
                    cube.transform.position = new Vector3(y + position.x, 0, x + position.z);
                }
            }
        }
    }
}