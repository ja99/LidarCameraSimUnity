using System;

namespace Communication.Messages {
    [Serializable]
    public class CarStateMessage : BaseMessage {
        /// <summary>
        /// The current car speed
        /// </summary>
        public float speed_actual = 0;
        /// <summary>
        /// The yaw rate
        /// </summary>
        public float yaw_rate = 0;
    }
}