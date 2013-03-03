using System;
using System.IO;
using System.Windows;

namespace NBA_2K13_Keep_My_Mod
{
    /// <summary>
    ///     Interaction logic for Window1.xaml
    /// </summary>
    public partial class ReadmeW : Window
    {
        public ReadmeW()
        {
            InitializeComponent();
            txtReadme.Text = File.ReadAllText(Environment.CurrentDirectory + @"\readme.txt");
        }

        private void readmeW_Loaded(object sender, RoutedEventArgs e)
        {
        }
    }
}