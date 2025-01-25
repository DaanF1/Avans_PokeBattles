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
        private Pokemon selectedPokemon = null;

        public CreateTeamWindow(Profile profile, PokemonLister lister)
        {
            InitializeComponent();

            this.playerProfile = profile;
            this.playerClient = profile.GetTcpClient();
            this.stream = profile.GetTcpClient().GetStream();
            this.pokemonLister = lister;
            this.selectedTeam = new List<Pokemon>();

            LoadAvailablePokemon();
            ConfigureTeamListBox();
            LoadPreviousTeam();
        }

        private void ConfigureTeamListBox()
        {
            listTeam.DisplayMemberPath = "Name"; // Display Name of Pokemon in list
            cmbSelectPokemon.SelectedIndex = 0;
        }

        private void LoadAvailablePokemon()
        {
            var availablePokemon = pokemonLister.GetAllPokemon(); // Load in all availible Pokemon in this game
            cmbSelectPokemon.ItemsSource = availablePokemon;
            cmbSelectPokemon.DisplayMemberPath = "Name";
        }

        private void cmbSelectPokemon_ItemSelected(object sender, EventArgs e)
        {
            if (selectedTeam.Count < 6)
            {
                Pokemon selPokemon = cmbSelectPokemon.SelectedItem as Pokemon;
                selectedTeam.Add(selPokemon); // Adds Pokemon to the list
                RefreshTeamList();
            }
            else
            {
                MessageBox.Show("There are no more spots availible on your team!", "Invalid Team", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            if (selectedPokemon == null)
            {
                MessageBox.Show("Please select a Pokemon from the list to proceed.", "Invalid Deletion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!selectedTeam.Contains(selectedPokemon))
            {
                MessageBox.Show("This Pokemon is not on your team!", "Invalid Deletion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (selectedTeam.Count > 0)
            {
                selectedTeam.Remove(selectedPokemon); // Deletes a selected Pokemon in the list from the list
                RefreshTeamList();
            }
        }

        private void btnDeleteTeam_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam.Count > 0)
            {
                selectedTeam.Clear(); // Clear the whole team
                RefreshTeamList();
            }
            else
            {
                MessageBox.Show("Cannot delete empty team!", "Invalid Deletion", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        private void btnSelectTeam_Click(object sender, RoutedEventArgs e)
        {
            if (selectedTeam.Count != 6)
            {
                MessageBox.Show("Please create a team of 6 Pokémon to proceed.", "Invalid Team", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var teamNames = string.Join(",", selectedTeam.ConvertAll(p => p.Name));
                // Check if the profile has the exact samen team as currently selected
                string trueNames = "";
                for (int i = 0; i < playerProfile.GetTeam().Count; i++) 
                { 
                    trueNames += playerProfile.GetTeam()[i].Name + ",";
                }
                trueNames = trueNames.Trim(',');

                // If the team is the same as the team in the profile, skip sending it to the server
                if (teamNames != trueNames)
                {
                    string message = $"create-team:{playerProfile.GetName()}:{teamNames}";
                    byte[] buffer = Encoding.UTF8.GetBytes(message);
                    stream.Write(buffer);
                    MessageBox.Show("Team selected and sent to the server!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send team to server: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            var lobbyWindow = new SelectLobbyWindow(playerProfile);
            lobbyWindow.Show();
            this.Close();
        }

        private void RefreshTeamList()
        {
            // Refreshes the list with the current team
            listTeam.ItemsSource = null;
            listTeam.ItemsSource = selectedTeam;
        }

        private void LoadPreviousTeam()
        {
            playerProfile.GetTeam().ForEach(pokemon=>selectedTeam.Add(pokemon)); // Loads in the previously selected team
            RefreshTeamList();
        }

        private void TeamPokemonSelected(object sender, SelectionChangedEventArgs e)
        {
            selectedPokemon = listTeam.SelectedItem as Pokemon; // Sets the selected Pokemon
        }
    }
}
