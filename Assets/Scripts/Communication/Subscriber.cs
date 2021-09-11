using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Communication {
    public class Subscriber {
        /// <summary>
        /// True as long as the connection shall be maintained
        /// </summary>
        private bool _running = true;

        /// <summary>
        /// The client socket
        /// </summary>
        private Socket _clientSocket;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The desired port for the publisher</param>
        /// <param name="callback">The callback function for the incoming data</param>
        public Subscriber(int port, Action<byte[]> callback) {
            new Thread(() => Start(port, callback)).Start();
        }

        /// <summary>
        /// Start the socket server and accept incoming requests
        /// </summary>
        /// <param name="port">The desired port for the publisher</param>
        /// <param name="callback">The callback function for the incoming data</param>
        private void Start(int port, Action<byte[]> callback) {
            while (_running) {
                try {
                    // Connect with the server      
                    _clientSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    _clientSocket.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));

                    // Handle incoming data
                    while (_running) {
                        // Receive data size
                        var bytes = new byte[4];
                        _clientSocket.Receive(bytes);
                        var dataSize = BitConverter.ToInt32(bytes, 0);
                        
                        // Receive data
                        var data = new byte[dataSize];
                        var received = 0;
                        while (received < dataSize && _running) {
                            received += _clientSocket.Receive(data, received, dataSize - received, SocketFlags.None);
                        }

                        // Start subscriber routines
                        if (dataSize > 0) callback(data); 
                    }
                }
                catch (Exception) {
                    // Try again in 1s
                    Thread.Sleep(1000);
                }
            }
        }

        /// <summary>
        /// Stop the socket connection
        /// </summary>
        public void Stop() {
            _running = false;
            try {
                if (_clientSocket.Connected) _clientSocket.Shutdown(SocketShutdown.Both);
                _clientSocket.Close();
            }
            finally {
                _clientSocket = null;
            }
        }
    }
}