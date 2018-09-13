using System;
using System.Diagnostics;
using System.Windows;

namespace Rottweiler.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow()
        {
            InitializeComponent();
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("http://paypal.me/Scobalula");
        }

        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Scobalula/Rottweiler/");
        }
    }
}
