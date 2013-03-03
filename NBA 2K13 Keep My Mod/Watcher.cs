#region Copyright Notice

//     Copyright 2011-2013 Eleftherios Aslanoglou
//  
//     Licensed under the Apache License, Version 2.0 (the "License");
//     you may not use this file except in compliance with the License.
//     You may obtain a copy of the License at
//  
//         http:www.apache.org/licenses/LICENSE-2.0
//  
//     Unless required by applicable law or agreed to in writing, software
//     distributed under the License is distributed on an "AS IS" BASIS,
//     WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//     See the License for the specific language governing permissions and
//     limitations under the License.

#endregion

using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace NBA_2K13_Keep_My_Mod
{
    internal class Watcher
    {
        public delegate void ControlUpdater(object sender, FileSystemEventArgs e);

        public delegate void ControlUpdaterRenamed(object sender, RenamedEventArgs e);

        public static bool resync = false;
        private static bool bootdone;
        public static bool disabled = false;

        private readonly FileSystemWatcher myWatcher;

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
            myWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite | NotifyFilters.FileName |
                                     NotifyFilters.DirectoryName;

            myWatcher.Deleted += myWatcher_Deleted;
            myWatcher.Created += myWatcher_Created;
            myWatcher.Renamed += myWatcher_Renamed;

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

        private void myWatcher_Created(object sender, FileSystemEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdater(updateList), DispatcherPriority.Send, new[] {sender, e});
        }

        private void myWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdater(updateList), DispatcherPriority.Send, new[] {sender, e});
        }

        private void myWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            MainWindow.mwInstance.Dispatcher.Invoke(new ControlUpdaterRenamed(updateListRenamed), DispatcherPriority.Send,
                                                    new[] {sender, e});
        }

        private static void updateList(object sender, FileSystemEventArgs e)
        {
            MainWindow.insertInList(e.FullPath + " was " + e.ChangeType.ToString().ToLower());

            if ((MainWindow.noUpdate == false) || MainWindow.restoring)
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
                else if (e.Name.Equals("patches") && e.ChangeType.ToString().Equals("Deleted") && resync &&
                         Directory.Exists(MainWindow.OnlineDataPath + "downloads") &&
                         (Directory.GetFiles(MainWindow.OnlineDataPath + "downloads").Length > 0))
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
            MainWindow.insertInList(e.OldFullPath + " was " + e.ChangeType.ToString().ToLower() + " to " + e.Name);

            if ((e.OldName == "latest") && (e.Name == "downloads"))
            {
                resync = true;
            }
        }
    }
}