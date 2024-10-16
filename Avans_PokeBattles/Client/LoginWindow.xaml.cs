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
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginWindow_Loaded(object sender, RoutedEventArgs e)
        {
            
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

            // Hide the login window and show the game window
            this.Hide();
            var gameWindow = new LobbyWindow();
            gameWindow.Show();
        }
    }
}