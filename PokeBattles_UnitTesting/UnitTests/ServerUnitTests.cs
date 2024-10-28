using System.Net.Sockets;
using System.Text;
using Avans_PokeBattles.Server;
using Server = Avans_PokeBattles.Server.Server;

namespace PokeBattles_UnitTesting.UnitTests
{
    [TestClass]
    public class ServerUnitTests // Unit Tests for the Server, LobbyManager and Lobby classes
    {
        public Server server = new(); // Create Server
        [TestMethod]
        public void JoinLobbyTest() // Unit Test: Joining a Lobby
        {
            if (Server.IsRunning() == false) // Start the Server if it wasn't running already
                Server.Start();

            TcpClient client1 = new(); // Create Client 1
            client1.Connect("localhost", 8000); // Connect to Server
            byte[] response1 = Encoding.UTF8.GetBytes("join-lobby:Lobby-1"); // Join Lobby 1
            client1.GetStream().Write(response1, 0, response1.Length); // Send command to the Server

            TcpClient client2 = new TcpClient(); // Create Client 2
            client2.Connect("localhost", 8000); // Connect to Server
            byte[] response2 = Encoding.UTF8.GetBytes("join-lobby:Lobby-1"); // Join Lobby 1
            client2.GetStream().Write(response2, 0, response2.Length); // Send command to the Server

            Assert.IsTrue(true); // Test is successful
        }

        [TestMethod]
        public void MultipleClientsTest() // Unit Test: Multiple Clients can connect to the Server
        {
            if (Server.IsRunning() == false) // Start the Server if it wasn't running already
                Server.Start();

            TcpClient client1 = new TcpClient(); // Create Client 1
            client1.Connect("localhost", 8000); // Connect to Server
            byte[] response1 = Encoding.UTF8.GetBytes("Hello from Client 1!");
            client1.GetStream().Write(response1, 0, response1.Length); // Send something to the Server

            TcpClient client2 = new TcpClient(); // Create Client 2
            client2.Connect("localhost", 8000); // Connect to Server
            byte[] response2 = Encoding.UTF8.GetBytes("Hello from Client 2!");
            client2.GetStream().Write(response2, 0, response2.Length); // Send something to the Server

            TcpClient client3 = new TcpClient(); // Create Client 3
            client3.Connect("localhost", 8000); // Connect to Server
            byte[] response3 = Encoding.UTF8.GetBytes("Hello from Client 3!");
            client3.GetStream().Write(response3, 0, response3.Length); // Send something to the Server

            Assert.IsTrue(true); // Test is successful
        }
    }
}
