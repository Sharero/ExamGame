using System.Windows;
using System.Windows.Controls;

namespace ArkanoidGame
{
    public partial class HelpPage : Page
    {
        public HelpPage()
        {
            InitializeComponent();
        }
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new MenuPage());
        }

    }
}
