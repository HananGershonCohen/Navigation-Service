using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace Navigation_Service
{
    public  class UdpReceiver
    {
        private UdpClient _udpClient;
        private int _port;
        private IPAddress _IPAddress = IPAddress.Loopback;


        public event EventHandler<RawDataReceivedEventArgs> RawDataReceived;

        public UdpReceiver(int port)
        {
            _port = port;
            // socket bound to any IP address on the specified port
            _udpClient = new UdpClient(new IPEndPoint(_IPAddress, _port));
        }

        /// <summary>
        /// await : recuuve a one datagram !!
        /// while loop : keep receiving datagrams
        /// </summary>
        /// miss : care Exception handling
        /// <returns></returns>
        public async Task StartListening()
        {
            Console.WriteLine("[UDP Receiver] Listening loop started. Waiting for datagrams...");
            try
            {
                while (true)
                {
                    UdpReceiveResult result = await _udpClient.ReceiveAsync();  // UdpReceiveResult is Struct
                    byte[] receivedBytes = result.Buffer;
                   
                    // event
                    RawDataReceived?.Invoke(this, new RawDataReceivedEventArgs(receivedBytes));
                }
            }
            catch (Exception ex)
            {
               
                Console.WriteLine($"[UDP Receiver] Error: {ex.Message}");
                _udpClient.Close();
            }

        }


        public void StopListening()
        {
            _udpClient.Close();
        }

       


    }
}
