﻿using System.Net.Sockets;
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

            this.tcpClient = new TcpClient("127.0.0.1", port);
            this.stream = tcpClient.GetStream();
        }

        private void txtName_PreviewMouseDown(object sender, MouseButtonEventArgs e)
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
            // Send the players name to the server
            byte[] buffer = Encoding.UTF8.GetBytes("Player name: " + name);
            stream.Write(buffer, 0, buffer.Length);

            // Show the select lobby window and close the current window
            var selectLobbyWindow = new SelectLobbyWindow(name, tcpClient);
            selectLobbyWindow.Show();
            this.Close();
        }
    }
}