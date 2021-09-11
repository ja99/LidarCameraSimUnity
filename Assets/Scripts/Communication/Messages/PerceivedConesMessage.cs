using System;
using System.Collections.Generic;

namespace Communication.Messages {
    [Serializable]
    public class Cone {
        /// <summary>
        /// X-Position
        /// </summary>
        public double x = 0;
        /// <summary>
        /// Y-Position
        /// </summary>
        public double y = 0;
        /// <summary>
        /// The type of cone (yellow, blue or orange)
        /// </summary>
        public string cone_type = "blue_cone";
        /// <summary>
        /// The probability of the cone
        /// </summary>
        public double probability = 0;
    }
    
    [Serializable]
    public class PerceivedConesMessage : BaseMessage {
        /// <summary>
        /// The perceived cones
        /// </summary>
        public List<Cone> cones = new List<Cone>();
    }
}