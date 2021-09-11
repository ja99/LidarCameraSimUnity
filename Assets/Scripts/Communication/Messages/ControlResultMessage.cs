using System;

namespace Communication.Messages {
    [Serializable]
    public class ControlResultMessage : BaseMessage {
        /**
         * Target velocity in driving direction (km/h) unsigned
         */
        public float speed_target = 0;

        /**
         * Target steering angle (°) signed
         */
        public float steering_angle_target = 0;

        /**
         *Target braking pressure (%) unsigned
         */
        public float brake_hydr_target = 0;

        /**
         * Target torque (%) signed
         */
        public float motor_moment_target = 0;

        /**
         * Finished rounds
         */
        public int lap_counter = 0;

        /**
         * Currently perceived cones
         */
        public int cones_count_actual = 0;

        /**
         * Whole amount of recognized cones on the track
         */
        public int cones_count_all = 0;
    }
}