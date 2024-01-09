using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ArkanoidGame
{
    public partial class MenuPage : Page
    {
        public MenuPage()
        {
            InitializeComponent();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            var message = MessageBox.Show("Are you sure you want to leave the game?", "Exit", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
            if (message == MessageBoxResult.OK)
            {
                Application.Current.Shutdown();
            }
        }
        private void Help_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new HelpPage());
        }

        private void PlayMusic(object sender, RoutedEventArgs e)
        {
            var wind = Application.Current.MainWindow as MainWindow;
            wind.musicPlayer.Source = new Uri("../../SoundtrackForArkanoid.mp3", UriKind.RelativeOrAbsolute);
            wind.musicPlayer.Play();
        }

        private void Game_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new GamePage());
        }
    }
}
