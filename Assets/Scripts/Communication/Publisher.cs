using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Communication {
    public class Publisher {
        /// <summary>
        /// The list of clients
        /// </summary>
        private List<Socket> _clients = new List<Socket>();

        /// <summary>
        /// True as long as the connection shall be maintained
        /// </summary>
        private bool _running = true;

        /// <summary>
        /// The actual server
        /// </summary>
        private Socket _socketServer;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="port">The desired port for the publisher</param>
        public Publisher(int port) {
            new Thread(() => Start(port)).Start();
        }

        /// <summary>
        /// Start the socket server and accept incoming requests
        /// </summary>
        /// <param name="port">The desired port for the publisher</param>
        private void Start(int port) {
            while (_running)
                try {
                    // Start the server      
                    _socketServer = new Socket(SocketType.Stream, ProtocolType.Tcp);
                    _socketServer.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
                    _socketServer.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), port));
                    _socketServer.Listen(10);

                    // Accept incoming connections
                    while (_running) {
                        var clientSocket = _socketServer.Accept();
                        lock (_clients) {
                            _clients.Add(clientSocket);
                        }
                    }

                    // Close the server and the clients
                    _socketServer.Close();
                }
                catch (Exception) {
                    // Try again in 1s
                    Thread.Sleep(1000);
                }
        }

        /// <summary>
        /// Stop the socket connection
        /// </summary>
        public void Stop() {
            lock (_clients) {
                _running = false;
                try {
                    if (_socketServer.Connected) _socketServer.Shutdown(SocketShutdown.Both);
                    _socketServer.Close();
                    foreach (var client in _clients) {
                        client.Shutdown(SocketShutdown.Both);
                        client.Close();
                    }
                }
                finally {
                    _socketServer = null;
                    _clients = new List<Socket>();
                }
            }
        }

        /// <summary>
        /// Publish a message to the clients
        /// </summary>
        /// <param name="msg">The message that shall be published</param>
        public void Publish(byte[] msg) {
            new Thread(() => {
                lock (_clients) {
                    for (var i = _clients.Count - 1; i >= 0; i--) {
                        try {
                            // TODO: Also send timestamp
                            _clients[i].Send(BitConverter.GetBytes(msg.Length));
                            _clients[i].Send(msg);
                        }
                        catch (Exception) {
                            _clients[i].Close();
                            _clients.RemoveAt(i);
                        }
                    }
                }
            }).Start();
        }
    }
}