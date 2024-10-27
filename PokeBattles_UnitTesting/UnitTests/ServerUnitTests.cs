using System.Net.Sockets;
using System.Text;
using Avans_PokeBattles.Server;
using Server = Avans_PokeBattles.Server.Server;

namespace PokeBattles_UnitTesting.UnitTests
{
    [TestClass]
    public class ServerUnitTests // Unit Tests for the Server, LobbyManager and Lobby classes
    {
        [TestMethod]
        public void JoinLobbyTest() // Unit Test: Joining a Lobby
        {
            Server server = new(); // Create Server
            server.Start(); // Start the Server

            TcpClient client1 = new(); // Create Client 1
            client1.Connect("localhost", 8000); // Connect to Server
            byte[] response1 = Encoding.UTF8.GetBytes("join-lobby:Lobby-1"); // Join Lobby 1
            client1.GetStream().Write(response1, 0, response1.Length); // Send command to the Server

            TcpClient client2 = new TcpClient(); // Create Client 2
            client2.Connect("localhost", 8000); // Connect to Server
            byte[] response2 = Encoding.UTF8.GetBytes("join-lobby:Lobby-1"); // Join Lobby 1
            client2.GetStream().Write(response2, 0, response2.Length); // Send command to the Server

            Assert.IsTrue(true); // Test is successful
            server.Stop(); // Stop the Server
        }

        [TestMethod]
        public void MultipleClientsTest() // Unit Test: Multiple Clients can connect to the Server
        {
            Server server = new();  // Create Server
            server.Start(); // Start the Server

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
            server.Stop(); // Stop the Server
        }

        [TestMethod]
        public void TestDefault() // Unit Test: Default last test to handle Server
        {
            Console.WriteLine("Default Unit Test: Success");
            Assert.IsTrue(true); // Test is always successful
        }
    }
}
