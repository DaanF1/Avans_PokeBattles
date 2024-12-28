using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Avans_PokeBattles.Server;

namespace Avans_PokeBattles.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private readonly TcpClient tcpClient;
        private readonly int port = 8000;

        public LoginWindow()
        {
            InitializeComponent();

            this.tcpClient = new TcpClient("127.0.0.1", port);
        }

        private void txtName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Text = "";
        }

        private void btnLogin_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the name and check if it's valid
            string name = txtName.Text.ToString();
            if (string.IsNullOrWhiteSpace(name) || txtName.Text.Equals("Enter your name..."))
            {
                MessageBox.Show("Please enter a valid name.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Make sure the name can fit in the next window
            if (name.ToString().Length > 9)
            {
                MessageBox.Show("Please enter a name shorter than 11 characters!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            // Create ProfileManager
            Profile profile = ProfileManager.Instance.GetOrCreateProfile(name, tcpClient);

            // Show the select lobby window and close the current window
            var selectLobbyWindow = new SelectLobbyWindow(profile);
            selectLobbyWindow.Show();
            this.Close();
        }
    }
}