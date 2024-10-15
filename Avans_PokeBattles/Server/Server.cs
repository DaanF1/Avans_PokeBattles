using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Avans_PokeBattles.Server
{
    public class Server
    {
        private static TcpListener listener;
        private static int port = 8000;
        private static int maxBytesInBuffer = 1500;
        public async void Start()
        {
            // Wait for Client connection
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            while (true)
            {
                var client = await listener.AcceptTcpClientAsync();
                HandleClientAsync(client);
            }
        }

        /// <summary>
        /// Handle a single Client's requests
        /// </summary>
        /// <param name="client"></param>
        public static async Task HandleClientAsync(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] readBuffer = new byte[maxBytesInBuffer];
            
            // Print incoming message
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(readBuffer, 0, maxBytesInBuffer);
                if (bytesRead == 0)
                {
                    break;
                }
                
                string message = Encoding.UTF8.GetString(readBuffer, 0, bytesRead);
                Console.WriteLine($"Received: {message}");
            }
            client.Dispose();
            client.Close();
        }
    }
}
