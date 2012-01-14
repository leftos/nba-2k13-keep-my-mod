using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace NBA_2K12_Keep_My_Mod
{
    /// <summary>
    /// Interaction logic for Window1.xaml
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
