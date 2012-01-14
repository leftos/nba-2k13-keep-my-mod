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
    /// Interaction logic for ODWindow.xaml
    /// </summary>
    public partial class ODWindow : Window
    {
        public ODWindow()
        {
            InitializeComponent();

            lstNew.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
            lstKeep.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
            lstUpdates.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));
            lstIgnored.Items.SortDescriptions.Add(new System.ComponentModel.SortDescription("Content", System.ComponentModel.ListSortDirection.Ascending));

            populateLists();
        }

        private void populateLists()
        {
            string[] odFiles = Directory.GetFiles(MainWindow.OnlineDataPath + @"downloads\");

            MainWindow.readModLists();
            
            foreach (string cur in odFiles)
            {
                string [] parts = cur.Split('\\');
                string curName = parts[parts.Length - 1];
                if (File.Exists(MainWindow.InstallationPath + curName))
                {
                    bool kept = MainWindow._keptmods.Contains(curName);
                    if (kept == true)
                    {
                        lstKeep.Items.Add(curName);
                    }
                    else
                    {
                        lstUpdates.Items.Add(curName);
                    }
                }
                else
                {
                    lstNew.Items.Add(curName);
                }
            }

            foreach (string cur in MainWindow._ignoredmods)
            {
                lstIgnored.Items.Add(MainWindow.getSafeFilename(cur));
            }
        }

        private void btnUseMod_Click(object sender, RoutedEventArgs e)
        {
            string[] _list = new string[lstUpdates.SelectedItems.Count];
            lstUpdates.SelectedItems.CopyTo(_list, 0);
            foreach (string item in _list)
            {
                lstKeep.Items.Add(item);
                lstUpdates.Items.Remove(item);
            }
        }

        private void btnKeepUpdate_Click(object sender, RoutedEventArgs e)
        {
            string[] _list = new string[lstKeep.SelectedItems.Count];
            lstKeep.SelectedItems.CopyTo(_list, 0);
            foreach (string item in _list)
            {
                lstUpdates.Items.Add(item);
                lstKeep.Items.Remove(item);
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            saveKeepMods();
            this.Close();
        }

        private void saveKeepMods()
        {
            try
            {
                Directory.CreateDirectory(MainWindow.KMMPath);
            }
            finally
            {
                string[] _list = new string[lstKeep.Items.Count];
                lstKeep.Items.CopyTo(_list, 0);
                File.WriteAllLines(MainWindow.KMMPath + "KeepMyMods.modlist", _list);

                string[] _updates = new string[lstUpdates.Items.Count];
                lstUpdates.Items.CopyTo(_updates, 0);
                File.WriteAllLines(MainWindow.KMMPath + "AllowUpdates.modlist", _updates);
            }
        }



    }
}
