using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Threading;

namespace NBA_2K13_Keep_My_Mod
{
    class Watcher
    {
        public delegate void ControlUpdater(object sender, FileSystemEventArgs e);
        public delegate void ControlUpdaterRenamed(object sender, RenamedEventArgs e);

        public static bool resync = false;
        private static bool bootdone = false;
        public static bool disabled = false;

        private FileSystemWatcher myWatcher;

        public Watcher(string path, string filter)
        {
            myWatcher = new FileSystemWatcher();

            try
            {
                myWatcher.Path = path;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Requested folder wasn't found.\nI was looking in " + path + " for " + filter);
                App.errorReport(ex, "Watcher path trycatch");
            }
            //myWatcher.Path = @"C:\";
            myWatcher.Filter = filter;
            myWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;

            myWatcher.Deleted += new FileSystemEventHandler(myWatcher_Deleted);
            myWatcher.Created += new FileSystemEventHandler(myWatcher_Created);
            myWatcher.Renamed += new RenamedEventHandler(myWatcher_Renamed);

            myWatcher.EnableRaisingEvents = true;
        }

        public void disableEvents()
        {
            myWatcher.EnableRaisingEvents = false;
        }

        public void enableEvents()
        {
            myWatcher.EnableRaisingEvents = true;
        }

        void myWatcher_Created(object sender, FileSystemEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdater(updateList), System.Windows.Threading.DispatcherPriority.Send, new object[] { sender, e });
        }

        void myWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdater(updateList), System.Windows.Threading.DispatcherPriority.Send,  new object[] {sender, e});
        }

        void myWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdaterRenamed(updateListRenamed), System.Windows.Threading.DispatcherPriority.Send, new object[] { sender, e });
        }

        private static void updateList(object sender, FileSystemEventArgs e)
        {
            MainWindow.insertInList(e.FullPath + " was " + e.ChangeType.ToString().ToLower());

            if ((MainWindow.noUpdate == false) || (MainWindow.restoring == true))
            {
                // When the game recreates the official roster, we can safely restore the user's custom rosters.
                if (e.Name.Equals("Roster.ROS") && e.ChangeType.ToString().Equals("Created"))
                {
                    if ((MainWindow.restoring == false) && (bootdone == false))
                    {
                        MainWindow.insertInList("Roster updated/re-created, restoring custom rosters...");
                        MainWindow.mwInstance.restoreCustomRosters();
                        Thread.Sleep(5000);
                        MainWindow.insertInList("Trying to keep mods...");
                        MainWindow.mwInstance.keepMods();
                        bootdone = true;
                        MainWindow.noUpdate = true;
                    }
                    else
                    {
                        MainWindow.insertInList("Backup of Official 2K Roster restored.");
                        MainWindow.restoring = false;
                    }
                }
            }
            else
            {
                if (e.Name.Equals("downloads") && e.ChangeType.ToString().Equals("Deleted"))
                {
                    MainWindow.insertInList("Online Data deleted. Waiting for re-sync...");

                    //EXPERIMENTAL
                    MainWindow.insertInList("Trying to keep Modded Online Data during re-sync...");
                    MainWindow.copyFolder(MainWindow.SaveRootPath + "ODBackupMods", MainWindow.OnlineDataPath + "downloads");
                    MainWindow.insertInList("Modded Online Data backup restored!");
                }
                else if (e.Name.Equals("patches") && e.ChangeType.ToString().Equals("Created"))
                {
                    /*if ((Directory.Exists(MainWindow.OnlineDataPath + "downloads") == false)
                        || ((Directory.Exists(MainWindow.OnlineDataPath + "downloads") == true)
                            && (Directory.GetFiles(MainWindow.OnlineDataPath + "downloads").Length == 0)
                            )
                        )
                    {*/
                        //MainWindow.insertInList("Re-sync of Online Data has started...");
                        MainWindow.mwInstance.lblResyncStatus.Content = "Re-syncing...";
                        //MainWindow.mwInstance.btnRestoreOD.IsEnabled = false;
                    //}
                }
                else if (e.Name.Equals("patches") && e.ChangeType.ToString().Equals("Deleted")
                        && (resync == true)
                        && Directory.Exists(MainWindow.OnlineDataPath + "downloads")
                        && (Directory.GetFiles(MainWindow.OnlineDataPath + "downloads").Length > 0)
                    )
                {
                    MainWindow.insertInList("Re-sync finished. Trying to keep mods again...");
                    MainWindow.mwInstance.lblResyncStatus.Content = "";
                    resync = false;

                    try
                    {
                        Directory.Delete(MainWindow.SaveRootPath + "ODBackup", true);
                    }
                    catch (DirectoryNotFoundException ex)
                    {
                    }

                    MainWindow.mwInstance.keepMods();
                    //MainWindow.mwInstance.btnRestoreOD.IsEnabled = true;
                }
            }
            
            
        }

        private static void updateListRenamed(object sender, RenamedEventArgs e)
        {
            MainWindow.insertInList(e.OldFullPath + " was " + e.ChangeType.ToString().ToLower()
                + " to " + e.Name);

            if ((e.OldName == "latest") && (e.Name == "downloads"))
            {
                resync = true;
            }
        }
    }
}
