using System;

namespace Communication.Messages {
    [Serializable]
    public class BaseMessage {
        /// <summary>
        /// The timestamp of creation
        /// </summary>
        public long time_stamp = 0;

        /// <summary>
        /// Create the timestamp
        /// </summary>
        public BaseMessage() {
            time_stamp = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}