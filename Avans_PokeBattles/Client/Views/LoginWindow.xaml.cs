using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Avans_PokeBattles.Client
{
    public partial class LoginWindow : Window
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private int port = 8000;
        private bool isDefaultText = true;

        public LoginWindow()
        {
            InitializeComponent();
            this.tcpClient = new TcpClient("127.0.0.1", port);
            this.stream = tcpClient.GetStream();
        }

        private void btnLogin_Clicked(object sender, RoutedEventArgs e)
        {
            string name = txtName.Text.ToString();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a valid name.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Send the player's name to the server
            byte[] buffer = Encoding.ASCII.GetBytes("Player name: " + name);
            stream.Write(buffer, 0, buffer.Length);

            // Start listening for server messages before opening LobbyWindow
            Task.Run(() => ListenForGameStart());
        }

        private void txtName_GotFocus(object sender, RoutedEventArgs e)
        {
            if (isDefaultText)
            {
                txtName.Text = "";  // Clear the default text
                isDefaultText = false;
            }
        }

        private async Task ListenForGameStart()
        {
            byte[] buffer = new byte[4096];
            while (true)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    if (message.StartsWith("start-game:"))
                    {
                        string[] playerInfo = message.Substring("start-game:".Length).Split(',');

                        string player1Name = playerInfo[0].Split(':')[1].Trim(); 
                        string player2Name = playerInfo[1].Split(':')[1].Trim(); 

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            var gameWindow = new LobbyWindow(tcpClient);
                            gameWindow.lblPlayer1Name.Content = player1Name;
                            gameWindow.lblPlayer2Name.Content = player2Name;
                            gameWindow.Show();
                            this.Close();
                        });
                    }
                }
            }
        }
    }
}
