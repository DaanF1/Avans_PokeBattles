using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Avans_PokeBattles.Server;

namespace Avans_PokeBattles.Client.Views
{
    /// <summary>
    /// Interaction logic for CreateTeamWindow.xaml
    /// </summary>
    public partial class CreateTeamWindow : Window
    {
        private readonly Profile playerProfile;
        private readonly TcpClient playerClient;
        private readonly NetworkStream stream;
        private readonly PokemonLister pokemonLister;
        private readonly List<Pokemon> selectedTeam;

        public CreateTeamWindow(Profile profile, TcpClient client, PokemonLister lister)
        {
            InitializeComponent();

            this.playerProfile = profile;
            this.playerClient = client;
            this.stream = client.GetStream();
            this.pokemonLister = lister;
            this.selectedTeam = new List<Pokemon>();

            LoadAvailablePokemon();
            ConfigureTeamListBox();
        }

        private void ConfigureTeamListBox()
        {
            listTeam.DisplayMemberPath = "Name";
        }

        private void LoadAvailablePokemon()
        {
            var availablePokemon = pokemonLister.GetAllPokemon();
            cmbSelectPokemon.ItemsSource = availablePokemon;
            cmbSelectPokemon.DisplayMemberPath = "Name";
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // Go to select lobby window
            var lobbyWindow = new SelectLobbyWindow(playerProfile, playerClient);
            lobbyWindow.Show();
            this.Close();
        }

        private void cmbSelectPokemon_ItemSelected(object sender, EventArgs e)
        {
            // Gets name of the Pokemon and adds it to the profile of this player
            Pokemon selPokemon = cmbSelectPokemon.SelectedItem as Pokemon;
            selectedTeam.Add(selPokemon);
            RefreshTeamList();
        }

        private void btnDeleteLast_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam.Count > 0)
            {
                selectedTeam.RemoveAt(selectedTeam.Count - 1);
                RefreshTeamList();
            }
        }

        private void btnDeleteTeam_Click(object sender, RoutedEventArgs e)
        {
            selectedTeam.Clear();
            RefreshTeamList();
        }

        private void btnSelectTeam_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam.Count != 6)
            {
                MessageBox.Show("Please select exactly 6 Pokémon to proceed.", "Invalid Team", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                var teamNames = string.Join(",", selectedTeam.ConvertAll(p => p.Name));
                string message = $"create team:{teamNames}";
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                stream.Write(buffer);

                MessageBox.Show("Team selected and sent to the server!", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                // Go back to selectlobbywindow
                var lobbyWindow = new SelectLobbyWindow(playerProfile, playerClient);
                lobbyWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send team to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshTeamList()
        {
            listTeam.ItemsSource = null;
            listTeam.ItemsSource = selectedTeam;
        }

    }
}
