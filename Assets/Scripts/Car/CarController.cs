using System;
using Communication.Messages;
using UnityEngine;

namespace Car {
    public class CarController : MonoBehaviour {
        /// <summary>
        /// The event that is triggered after a new car state was generated
        /// </summary>
        public Action<CarStateMessage> OnNewCarState;
        
        /// <summary>
        /// The actual front wheels
        /// </summary>
        public Transform frontLeftWheel, frontRightWheel;

        /// <summary>
        /// The colliders of the front wheels (for steering)
        /// </summary>
        public WheelCollider frontLeftCollider, frontRightCollider;

        /// <summary>
        /// The colliders of the rear wheels (for thrust)
        /// </summary>
        public WheelCollider rearLeftCollider, rearRightCollider;
        
        /// <summary>
        /// Important data for the car state
        /// </summary>
        private Vector3 _pos = Vector3.zero, _velocity = Vector3.zero, _rot = Vector3.zero;
        
        /// <summary>
        /// Update the car state
        /// </summary>
        private void FixedUpdate() {
            var trans = transform;
            
            // Calculate the velocity
            var lastPos = _pos;
            _pos = trans.position;
            _velocity = (_pos - lastPos) / Time.fixedDeltaTime;

            // Calculate the angular velocity
            var lastRot = _rot;
            _rot = trans.rotation.eulerAngles;
            var angularVelocity = (_rot - lastRot) / Time.fixedDeltaTime;
            
            // Create the message
            var carState = new CarStateMessage {
                speed_actual = _velocity.magnitude,
                yaw_rate = -angularVelocity.y * Mathf.Deg2Rad
            };
            OnNewCarState?.Invoke(carState);
        }
        
        /// <summary>
        /// Update the car
        /// </summary>
        /// <param name="control">The control result from the as</param>
        public void ApplyControlResult(ControlResultMessage control) {
            SetFrontWheelAngle(frontLeftCollider, frontLeftWheel, control.steering_angle_target);
            SetFrontWheelAngle(frontRightCollider, frontRightWheel, control.steering_angle_target);
            // TODO: Set max motor torque (motor_moment_target is a relative value)
            rearLeftCollider.motorTorque = control.motor_moment_target * 50;
            rearRightCollider.motorTorque = control.motor_moment_target * 50;
        }

        /// <summary>
        /// Set the rotation of the wheels including the colliders 
        /// </summary>
        /// <param name="wheelCollider">The wheel collider</param>
        /// <param name="wheelTransform">The visuals of the wheel</param>
        /// <param name="steeringAngle">The steering angle</param>
        private static void SetFrontWheelAngle(WheelCollider wheelCollider, Transform wheelTransform,
            float steeringAngle) {
            wheelCollider.steerAngle = steeringAngle;
            wheelTransform.rotation = Quaternion.Euler(0, steeringAngle, 90);
        }
    }
}