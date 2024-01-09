using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;


namespace ArkanoidGame
{

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            musicPlayer.Volume = 0.1;
            gridFrame.Navigate(new MenuPage());
        }
        public void changeColors(Button sender, string color) // при наведении на кнопку меняет ее картинку на белый вариант и наоборот
        {
            Button btn = sender as Button;
            Image img = (Image)btn.Content;

            string path = img.Source.ToString();
            if (path.Contains("white"))
                path = path.Replace("white", color);
            else
                path = path.Replace(color, "white");
            img.Source = new BitmapImage(new Uri(path));
        }
        private void Music_Ended(object sender, RoutedEventArgs e)
        {
            musicPlayer.Position = TimeSpan.Zero;
            musicPlayer.Play();
        }
    }
}
