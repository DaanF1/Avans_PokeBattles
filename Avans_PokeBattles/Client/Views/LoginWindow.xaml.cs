using System.Net.Sockets;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Avans_PokeBattles.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private TcpClient tcpClient;
        private NetworkStream stream;
        private int port = 8000;
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            tcpClient = new TcpClient("127.0.0.1", port);
            stream = tcpClient.GetStream();
        }

        private void txtName_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtName.Text = "";
        }

        private void btnLogin_Clicked(object sender, RoutedEventArgs e)
        {
            // Get the name and check if it's valid
            string name = txtName.Text.ToString();
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("Please enter a valid name.", "", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            // Make sure the name can fit in the next window
            if (name.ToString().Count() > 9)
            {
                MessageBox.Show("Please enter a name shorter than 11 characters!", "", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            byte[] buffer = Encoding.ASCII.GetBytes(name);
            stream.Write(buffer, 0, buffer.Length);

            // Hide the login window and show the lobby window
            this.Hide();
            var selectLobbyWindow = new SelectLobbyWindow(tcpClient);
            selectLobbyWindow.Show();
        }
    }
}