using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Xml.Linq;
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

            var profileManager = ProfileManager.Instance;
            var profile = profileManager.GetProfile(name);

            // Makes sure the name is not already chosen
            if (profile == null)
            {
                profile = profileManager.CreateProfile(name, tcpClient);
            }
            else
            {
                profile.SetTcpClient(tcpClient);
            }
            // Make sure that the name is valid in length
            if (string.IsNullOrWhiteSpace(name) || txtName.Text.Equals("Enter your name..."))
            {
                MessageBox.Show("Please enter a valid name.", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Make sure the name can fit in the next window
            if (name.ToString().Length > 9)
            {
                MessageBox.Show("Please enter a name shorter than 10 characters!", "Invalid Name", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            // Show the select lobby window and close the current window
            var selectLobbyWindow = new SelectLobbyWindow(profile);
            selectLobbyWindow.Show();
            this.Close();
        }
    }
}