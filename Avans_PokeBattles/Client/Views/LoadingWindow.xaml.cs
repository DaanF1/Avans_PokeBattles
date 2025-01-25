using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Avans_PokeBattles.Client.Views
{
    /// <summary>
    /// Interaction logic for LoadingWindow.xaml
    /// </summary>
    public partial class LoadingWindow : Window
    {
        private const int GWL_STYLE = -16;
        private const int WS_SYSMENU = 0x80000;
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        public LoadingWindow(string waitingText)
        {
            InitializeComponent();
            lblWaitingText.Content = waitingText;
        }

        private async void LoadingWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // Remove X button in Window
            // Source (StackOverflow): https://stackoverflow.com/questions/743906/how-to-hide-close-button-in-wpf-window 
            var hwnd = new WindowInteropHelper(this).Handle;
            SetWindowLong(hwnd, GWL_STYLE, GetWindowLong(hwnd, GWL_STYLE) & ~WS_SYSMENU);

            if (lblWaitingText.Content.ToString() == "Waiting for another player.")
            {
                while (true)
                {
                    switch (lblWaitingText.Content.ToString())
                    {
                        case "Waiting for another player.":
                            lblWaitingText.Content = "Waiting for another player..";
                            break;
                        case "Waiting for another player..":
                            lblWaitingText.Content = "Waiting for another player...";
                            break;
                        case "Waiting for another player...":
                            lblWaitingText.Content = "Waiting for another player.";
                            break;
                    }
                    await Task.Delay(1000);
                }
            } else if (lblWaitingText.Content.ToString() == "Waiting for pokemon to load in.")
            {
                while (true)
                {
                    switch (lblWaitingText.Content.ToString())
                    {
                        case "Waiting for pokemon to load in.":
                            lblWaitingText.Content = "Waiting for pokemon to load in..";
                            break;
                        case "Waiting for pokemon to load in..":
                            lblWaitingText.Content = "Waiting for pokemon to load in...";
                            break;
                        case "Waiting for pokemon to load in...":
                            lblWaitingText.Content = "Waiting for pokemon to load in.";
                            break;
                    }
                    await Task.Delay(1000);
                }
            }
        }
    }
}
