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
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using LeftosCommonLibrary;
using Microsoft.Win32;

namespace NBA_2K13_Keep_My_Mod
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow mwInstance;
        public static bool bootSuccess = false;
        public static string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\";
        public static string SaveRootPath;
        public static string SavesPath;
        public static string OnlineDataPath;
        public static string CachePath;

        public static string InstallationPath;

        public static string UserDekstopPath = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\";

        public static string AppPath;

        public static bool restoring = false;
        public static bool noUpdate = false;

        public static List<string> _keptmods = new List<string>();
        public static List<string> _allowedmods = new List<string>();
        public static List<string> _allmods = new List<string>();
        public static List<string> _ignoredmods = new List<string>();

        private static bool paused;

        public static long oldsize = 0;
        private readonly Watcher cacheWatcher;
        private readonly Watcher odWatcher;
        private readonly Watcher savesWatcher;

        [PermissionSet(SecurityAction.Demand, Name = "FullTrust")]
        public MainWindow()
        {
            try
            {
                InitializeComponent();

            AppPath = Environment.CurrentDirectory + "\\";

            Tools.AppName = "NBA 2K13 Keep My Mod";
            Tools.AppRegistryKey = @"SOFTWARE\Lefteris Aslanoglou\NBA 2K13 Keep My Mod";

            /*
            if (IsProcessOpen(Process.GetCurrentProcess().ProcessName))
            {
                MessageBox.Show("You may only have one instance of this tool running at any time.");
                Environment.Exit(-1);
            }
            */

            bool gameRunning = isGameRunning();

            mwInstance = this;

            bootSuccess = true;

            insertInList("NBA 2K13 Keep My Mod - version " + Assembly.GetExecutingAssembly().GetName().Version);

            getPaths();

            try
            {
                if (!gameRunning)
                {
                    Directory.Delete(CachePath + "patches", true);
                }
            }
            catch
            {
            }

            cacheWatcher = new Watcher(CachePath, "patches");
            savesWatcher = new Watcher(SavesPath, "*");
            odWatcher = new Watcher(OnlineDataPath, "downloads");

            readModLists();

            if (!gameRunning)
            {
                insertInList("Game isn't running.");
                hideRosters();
                restoreOnlineData(onBoot: true);
            }
            else
            {
                insertInList("Game is already running.");
                noUpdate = true;
                keepMods();
            }

            // Create a Timer with a Normal Priority
            var _timer = new DispatcherTimer();

            // Set the Interval to 2 seconds
            _timer.Interval = TimeSpan.FromMilliseconds(3000);

            // Set the callback to just show the time ticking away
            // NOTE: We are using a control so this has to run on 
            // the UI thread
            _timer.Tick += delegate
                {
                    try
                    {
                        if (isGameRunning())
                        {
                            if (Directory.Exists(CachePath + "patches"))
                            {
                                long fullsize = -1;
                                int count = 0;
                                try
                                {
                                    var tr = new StreamReader(App.AppDocsPath + "2Konlinedata_crc.hist");
                                    while (tr.Peek() > -1)
                                    {
                                        tr.ReadLine();
                                        count++;
                                    }
                                    count--;
                                    tr.BaseStream.Seek(0, SeekOrigin.Begin);
                                    string[] lastline = textTail(tr, 1);
                                    string[] lparts = lastline[0].Split('\t');
                                    if (lparts.Length == 1)
                                    {
                                        fullsize = Convert.ToInt32(lparts[0]);
                                    }
                                    tr.Close();
                                }
                                catch
                                {
                                }
                                string[] patchfiles = Directory.GetFiles(CachePath + "patches");
                                long b = 0;
                                foreach (string f in patchfiles)
                                {
                                    var fi = new FileInfo(f);
                                    b += fi.Length;
                                }
                                var speed = (int) ((b - oldsize)/1024/3);
                                lblResyncStatus.Content = "Re-syncing... (" + patchfiles.Length;
                                if (count > 0)
                                {
                                    lblResyncStatus.Content += "/" + count.ToString();
                                }
                                lblResyncStatus.Content += " files, " + String.Format("{0:F1}", ((float) b/1024/1024));
                                if (fullsize > -1)
                                {
                                    lblResyncStatus.Content += "/" + String.Format("{0:F1}", ((float) fullsize/1024/1024));
                                }
                                lblResyncStatus.Content += "MB downloaded";
                                if (oldsize > 0)
                                {
                                    lblResyncStatus.Content += ", " + speed.ToString() + "KB/s";
                                    if (fullsize > 0)
                                    {
                                        if (b < fullsize)
                                        {
                                            if (speed > 0)
                                            {
                                                lblResyncStatus.Content += ", ";
                                                var minutes = (int) ((fullsize - b)/1024/speed/60);
                                                var seconds = (int) ((fullsize - b)/1024/speed%60);
                                                if (minutes > 0)
                                                {
                                                    lblResyncStatus.Content += minutes.ToString() + " minutes ";
                                                }
                                                lblResyncStatus.Content += seconds.ToString() + " seconds remaining";
                                            }
                                        }
                                    }
                                }
                                lblResyncStatus.Content += ")";
                                oldsize = b;
                            }
                            else
                            {
                                lblResyncStatus.Content = "";
                                oldsize = 0;
                            }
                        }
                        else
                        {
                            lblResyncStatus.Content = "";
                            if (Directory.Exists(CachePath + "patches"))
                            {
                                Directory.Delete(CachePath + "patches", true);
                            }
                        }
                    }
                    catch
                    {
                    }
                };

            // Start the timer
            _timer.Start();

            var w = new BackgroundWorker();
            w.DoWork += delegate
                {
                    checkForUpdates();
                };
            w.RunWorkerAsync();

                //App.errorReport(new Exception());
            }
            catch (Exception ex)
            {
                App.errorReport(ex, "MainWindow InitializeComponent");
            }
        }

        /// <summary>Returns the end of a text reader.</summary>
        /// <param name="reader">The reader to read from.</param>
        /// <param name="lineCount">The number of lines to return.</param>
        /// <returns>The last lneCount lines from the reader.</returns>
        public static string[] textTail(TextReader reader, int lineCount)
        {
            var buffer = new List<string>(lineCount);
            string line;
            for (int i = 0; i < lineCount; i++)
            {
                line = reader.ReadLine();
                if (line == null)
                {
                    return buffer.ToArray();
                }
                buffer.Add(line);
            }

            int lastLine = lineCount - 1;
            //The index of the last line read from the buffer.  Everything > this index was read earlier than everything <= this indes

            while (null != (line = reader.ReadLine()))
            {
                lastLine++;
                if (lastLine == lineCount)
                {
                    lastLine = 0;
                }
                buffer[lastLine] = line;
            }

            if (lastLine == lineCount - 1)
            {
                return buffer.ToArray();
            }
            var retVal = new string[lineCount];
            buffer.CopyTo(lastLine + 1, retVal, 0, lineCount - lastLine - 1);
            buffer.CopyTo(0, retVal, lineCount - lastLine - 1, lastLine + 1);
            return retVal;
        }

        private bool isGameRunning()
        {
            if (IsProcessOpen("nba2k13"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                if (clsProcess.ProcessName.ToLowerInvariant().Equals(name))
                {
                    //if the process is found to be running then we
                    //return a true
                    return true;
                }
            }
            //otherwise we return a false
            return false;
        }

        private void getPaths()
        {
            RegistryKey rk = null;

            try
            {
                rk = Registry.CurrentUser;
            }
            catch (Exception ex)
            {
                App.errorReport(ex, "Registry.CurrentUser");
            }
            rk = rk.OpenSubKey(@"SOFTWARE\2K Sports\NBA 2K13");
            if (rk == null)
            {
                MessageBox.Show(
                    "NBA 2K13 doesn't seem to be installed in this computer/for this user.\nThe required registry entries could not be found.");
                Environment.Exit(-1);
            }

            try
            {
                var installDir = rk.GetValue("Install Dir");
                if (installDir == null)
                {
                    throw new Exception("Registry value not found.");
                }
                InstallationPath = installDir + @"\";
            }
            catch (Exception ex)
            {
                bool needToSet = false;
                RegistryKey rk2 = null;

                try
                {
                    rk2 = Registry.CurrentUser;
                    rk2 = rk2.OpenSubKey(Tools.AppRegistryKey, true);
                    if (rk2 == null)
                    {
                        rk2 = Registry.CurrentUser;
                        rk2.CreateSubKey(Tools.AppRegistryKey);
                        needToSet = true;
                    }
                    else
                    {
                        try
                        {
                            InstallationPath = rk2.GetValue("NBA 2K13 Installation Path").ToString();
                        }
                        catch
                        {
                            needToSet = true;
                        }
                    }
                }
                catch (Exception ex2)
                {
                    App.errorReport(ex2, "Finding installation path of NBA 2K13 through KMM's own entry.");
                }

                if (needToSet)
                {
                    var ofd = new OpenFileDialog();
                    ofd.Filter = "NBA2K13.exe|nba2K13.exe";
                    ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) +
                                           @"\2K Sports\NBA 2K13";
                    if (ofd.InitialDirectory == "")
                    {
                        ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles) +
                                               @"\2K Sports\NBA 2K13";
                        if (ofd.InitialDirectory == "")
                        {
                            string[] parts = Environment.CurrentDirectory.Split('\\');
                            ofd.InitialDirectory = parts[0] + @"\Games\NBA 2K13";
                            if (ofd.InitialDirectory == "")
                            {
                                ofd.InitialDirectory = parts[0];
                            }
                        }
                    }
                    ofd.Title = "Please select the 'nba2K13.exe' in the game's installation folder.";
                    ofd.ShowDialog();

                    if (ofd.FileName == "")
                    {
                        MessageBox.Show(
                            "You need to point the tool to the folder you've installed NBA 2K13.\n\nIt can't work otherwise.\n\nPlease restart the tool and try again.");
                        rk.Close();
                        rk2.Close();
                        Environment.Exit(-1);
                    }

                    string fname = ofd.FileName;
                    string[] parts2 = fname.Split('\\');
                    fname = fname.Substring(0, fname.Length - parts2[parts2.Length - 1].Length);
                    InstallationPath = fname;
                    rk2.SetValue("NBA 2K13 Installation Path", InstallationPath);
                    rk2.Close();
                }
            }
            KMMPath = InstallationPath + @"KeepMyMods\";
            SavesPath = rk.GetValue("Saves").ToString();
            SaveRootPath = SavesPath.Substring(0, SavesPath.Length - 6);
            OnlineDataPath = rk.GetValue("Online Data").ToString();
            CachePath = rk.GetValue("Cache").ToString();

            if (Directory.Exists(KMMPath) == false)
            {
                try
                {
                    Directory.CreateDirectory(KMMPath);
                }
                catch (Exception ex)
                {
                    App.errorReport(ex, "CreateDirectory KMM");
                }
            }

            if (Directory.Exists(CachePath) == false)
            {
                try
                {
                    Directory.CreateDirectory(CachePath);
                }
                catch (Exception ex)
                {
                    App.errorReport(ex, "CreateDirectory cache");
                }
            }

            rk.Close();

            insertInList("Detected Paths");
            insertInList("Installation Path: " + InstallationPath);
            insertInList("KeepMyMods Path: " + KMMPath);
            insertInList("Saves Root Path: " + SaveRootPath);
            insertInList("Saves Path: " + SavesPath);
            insertInList("Cache Path: " + CachePath);
            insertInList("Online Data Path: " + OnlineDataPath);
        }

        private void hideRosters()
        {
            if (File.Exists(SavesPath + "Roster.ROS"))
            {
                insertInList("2K Original Roster found, backing up...");
                File.Delete(SavesPath + "Roster.ORB");
                File.Move(SavesPath + "Roster.ROS", SavesPath + "Roster.ORB");
            }

            changeExtension(SavesPath, "ROS", "CRB");
        }

        private void btnGetCustom_Click(object sender, RoutedEventArgs e)
        {
            int customCount = Directory.GetFiles(SavesPath, "*.CRB").Length;
            restoreCustomRosters();

            if (!isGameRunning())
            {
                if (customCount > 0)
                {
                    noUpdate = true;
                    paused = true;

                    disableAllWatchers();
                    insertInList("        (4/4): Alternatively you can also use \"Hide Rosters (Force Update)\" to restart them.");
                    insertInList("               and click on \"Force Keep My Mods\" to restart the tool's automatic capabilities.");
                    insertInList("        (3/4): You'll have to Alt-Tab back into the tool after you reach the Home screen,");
                    insertInList("        (2/4): This way, the tool has no way of knowing when the boot-up sequence has ended.");
                    insertInList("WARNING (1/4): You restored your custom rosters from their backup, before the game started.");
                }
            }
        }

        private void changeExtension(string path, string orige, string newe)
        {
            string[] fileList = Directory.GetFiles(path, "*." + orige);
            if (fileList.Length > 0)
            {
                if (orige.Equals("ROS") && newe.Equals("CRB"))
                {
                    insertInList("Backing up any additional custom rosters...");
                }
            }
            foreach (string cur in fileList)
            {
                string newFilename = cur.Substring(0, cur.Length - 3) + newe;
                if (File.Exists(newFilename))
                {
                    MessageBoxResult result = MessageBox.Show(newFilename + " already exists. Are you sure you want to overwrite it?",
                                                              "NBA 2K13 Keep My Mod", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No)
                    {
                        continue;
                    }
                    File.Delete(newFilename);
                }
                File.Move(cur, newFilename);
            }
        }

        private void btnStartSteam_Click(object sender, RoutedEventArgs e)
        {
            string steamUrl = "steam://rungameid/219600";
            var _processStartInfo = new ProcessStartInfo();
            _processStartInfo.WorkingDirectory = InstallationPath;

            if (InstallationPath.Contains("steamapps"))
            {
                _processStartInfo.FileName = steamUrl;
            }
            else
            {
                _processStartInfo.FileName = InstallationPath + "nba2K13.exe";
            }

            restoreOnlineData(onGameStart: true);

            if (File.Exists(SavesPath + "Roster.ROS") == false)
            {
                cacheWatcher.disableEvents();
                odWatcher.disableEvents();
            }

            Process myProcess = Process.Start(_processStartInfo);
        }

        private void btnRestoreOriginal_Click(object sender, RoutedEventArgs e)
        {
            restoring = true;
            changeExtension(SavesPath, "ORB", "ROS");
            if (!isGameRunning())
            {
                if (File.Exists(SavesPath + "Roster.ROS"))
                {
                    noUpdate = true;
                    paused = true;

                    disableAllWatchers();
                    insertInList("        (4/4): Alternatively you can also use \"Hide Rosters (Force Update)\" to restart them.");
                    insertInList("               and click on \"Force Keep My Mods\" to restart the tool's automatic capabilities.");
                    insertInList("        (3/4): You'll have to Alt-Tab back into the tool after you reach the Home screen,");
                    insertInList("        (2/4): This way, the tool has no way of knowing when the boot-up sequence has ended.");
                    insertInList("WARNING (1/4): You restored the game's original roster from its backup.");
                }
            }
        }

        public void restoreCustomRosters()
        {
            int customCount = Directory.GetFiles(SavesPath, "*.CRB").Length;
            if (customCount > 0)
            {
                insertInList("Re-enabling any additional custom rosters...");
                changeExtension(SavesPath, "CRB", "ROS");
            }
        }

        private void btnODShow_Click(object sender, RoutedEventArgs e)
        {
            if (Directory.Exists(OnlineDataPath + @"downloads\"))
            {
                var odw = new ODWindow();
                odw.ShowDialog();
            }
            else
            {
                MessageBox.Show("NBA 2K13 has no updates stored right now. Please wait for a re-sync to end before trying again.");
            }
        }

        public static void readModLists()
        {
            string[] _modlists = Directory.GetFiles(KMMPath, "*.modlist");
            insertInList("Found " + _modlists.Length + " modlists.");

            _keptmods.Clear();
            _allowedmods.Clear();
            _allmods.Clear();
            _ignoredmods.Clear();

            var odfiles = new[] {""};
            try
            {
                odfiles = Directory.GetFiles(OnlineDataPath + "downloads");
                for (int i = 0; i < odfiles.Length; i++)
                {
                    odfiles[i] = getSafeFilename(odfiles[i]);
                }
            }
            catch
            {
                insertInList("Online Data downloads folder wasn't found.");
            }

            foreach (string modlist in _modlists)
            {
                if (modlist.Contains("AllowUpdates.modlist"))
                {
                    continue;
                }
                ;

                insertInList("Parsing " + modlist + "...");
                string[] _temp = File.ReadAllLines(modlist);
                foreach (string mod in _temp)
                {
                    if (_allmods.Contains(mod) == false)
                    {
                        _allmods.Add(mod);
                        if (odfiles.Contains(mod))
                        {
                            _keptmods.Add(mod);
                        }
                        else
                        {
                            _ignoredmods.Add(mod);
                        }
                    }
                }
            }

            string[] _allowed;
            try
            {
                insertInList("Parsing allowed 2K updates (AllowUpdates.modlist)...");
                _allowed = File.ReadAllLines(KMMPath + "AllowUpdates.modlist");
            }
            catch
            {
                return;
            }

            foreach (string mod in _allowed)
            {
                if (_keptmods.Contains(mod))
                {
                    _keptmods.Remove(mod);
                    _allowedmods.Add(mod);
                }
            }
            insertInList(_allmods.Count.ToString() + " unique game files parsed, " + _keptmods.Count.ToString() +
                         " to be kept modded, " + _allowedmods.Count.ToString() + " to be replaced by 2K's updates, " +
                         _ignoredmods.Count.ToString() + " ignored because they haven't been updated by 2K.");
        }

        private List<string> findNew()
        {
            string[] odFiles = Directory.GetFiles(OnlineDataPath + @"downloads\");

            var newFiles = new List<string>();

            foreach (string cur in odFiles)
            {
                string[] parts = cur.Split('\\');
                string curName = parts[parts.Length - 1];
                if (File.Exists(InstallationPath + curName))
                {
                }
                else
                {
                    newFiles.Add(curName);
                }
            }

            return newFiles;
        }

        internal void keepMods()
        {
            readModLists();

            if (Directory.Exists(SaveRootPath + "ODBackup"))
            {
                restoreOnlineData();
                keepMods();
            }
            else
            {
                disableAllWatchers();

                backupOnlineData();

                insertInList("Overwriting conflicting updates with mods...");
                foreach (string item in _keptmods)
                {
                    try
                    {
                        File.Copy(InstallationPath + item, OnlineDataPath + @"downloads\" + item, true);
                    }
                    catch (Exception ex)
                    {
                        insertInList("Failed to keep mods, an error occured.");
                        string e = ex.ToString();
                        string[] earr = e.Split('\n');
                        foreach (string line in earr)
                        {
                            insertInList(line);
                        }
                        insertInList("Please use the Save Logs option and inform the developer using the NLSC thread.");
                        return;
                    }
                }
                if (UpdateMNF() != 0)
                {
                    insertInList("Failed to update downloads.mnf.");
                    restoreOnlineData();
                    return;
                }

                /*
                insertInList("Adding 2K's new files to installation folder...");
                List<string> newFiles = findNew();
                foreach (string file in newFiles)
                {
                    File.Copy(OnlineDataPath + @"downloads\" + file, InstallationPath + file, true);
                }
                */

                //Succesfully copied all mods

                if (Directory.Exists(OnlineDataPath + @"downloads\"))
                {
                    insertInList("Keeping backup of Online Data with Mods...");
                    if (Directory.Exists(SaveRootPath + "ODBackupMods"))
                    {
                        string[] odbfiles = Directory.GetFiles(SaveRootPath + "ODBackupMods");
                        foreach (string f in odbfiles)
                        {
                            File.Delete(f);
                        }
                        Directory.Delete(SaveRootPath + "ODBackupMods");
                    }
                    string srcDir = OnlineDataPath + @"downloads\";
                    string destDir = SaveRootPath + @"ODBackupMods\";
                    copyFolder(srcDir, destDir);

                    insertInList("Successfully kept mods!");
                }

                enableAllWatchers();
            }
        }

        private int UpdateMNF()
        {
            try
            {
                var fs = new FileStream(OnlineDataPath + @"downloads\downloads.mnf", FileMode.Open, FileAccess.ReadWrite,
                                        FileShare.ReadWrite);
                var br = new BinaryReader(fs, Encoding.ASCII);
                var bw = new BinaryWriter(fs, Encoding.ASCII);
                foreach (string mod in _keptmods)
                {
                    br.BaseStream.Position = 4; // Skip CRC
                    byte[] hex = Encoding.ASCII.GetBytes(mod);
                    bool done = false;
                    while (!done)
                    {
                        byte[] ba2 = br.ReadBytes(hex.Length);
                        if (ba2.Length < hex.Length)
                        {
                            insertInList("Error keeping " + mod);
                            break;
                        }
                        if (ba2.SequenceEqual(hex))
                        {
                            br.BaseStream.Position -= hex.Length + 6;
                            bw.BaseStream.Position = br.BaseStream.Position;
                            byte[] ba = BitConverter.GetBytes(Convert.ToInt32(new FileInfo(InstallationPath + mod).Length));
                            bw.Write(ba);
                            byte[] crc = Tools.ReverseByteOrder(Tools.HexStringToByteArray(getCRC(InstallationPath + mod)), 4);
                            bw.Write(crc);
                            insertInList("Kept " + mod);
                            done = true;
                        }
                        else
                        {
                            br.BaseStream.Position -= hex.Length - 2;
                        }
                    }
                }
                //bw.Close();
                var bw2 = new BinaryWriter(File.Open(App.AppDocsPath + "temp.mnf", FileMode.Create));
                br.BaseStream.Position = 4;
                byte[] buf = br.ReadBytes((int) br.BaseStream.Length - 4);
                bw2.Write(buf);
                bw2.Close();
                byte[] crc2 = Tools.ReverseByteOrder(Tools.HexStringToByteArray(getCRC(App.AppDocsPath + "temp.mnf")), 4);
                br.Close();
                bw.Close();
                fs.Close();
                bw = new BinaryWriter(File.Open(OnlineDataPath + @"downloads\downloads.mnf", FileMode.Open, FileAccess.Write),
                                      Encoding.ASCII);
                bw.Write(crc2);
                bw.Write(buf);
                bw.Close();
                insertInList("Updated downloads.mnf");
                return 0;
            }
            catch (Exception ex)
            {
                insertInList("Exception thrown in UpdateMNF: " + ex.Message);
                return -1;
            }
        }

        private static void enableAllWatchers()
        {
            mwInstance.odWatcher.enableEvents();
            mwInstance.savesWatcher.enableEvents();
            mwInstance.cacheWatcher.enableEvents();
            insertInList("All watchers enabled!");
            mwInstance.lblLog.Content = "Log (Automatic capabilities: ON)";
        }

        /// <summary>
        ///     Copy a folder (non-recursive, overwrites destination folder if it exists)
        /// </summary>
        /// <param name="srcDir">Source directory</param>
        /// <param name="destDir">Destination directory</param>
        public static void copyFolder(string srcDir, string destDir)
        {
            if (Directory.Exists(destDir))
            {
                Directory.Delete(destDir, true);
            }

            Directory.CreateDirectory(destDir);

            if (srcDir[srcDir.Length - 1] != '\\')
            {
                srcDir += '\\';
            }
            if (destDir[destDir.Length - 1] != '\\')
            {
                destDir += '\\';
            }

            string[] files = Directory.GetFiles(srcDir);
            foreach (string f in files)
            {
                string curName = getSafeFilename(f);
                File.Copy(f, destDir + curName);
            }
        }

        public static string getSafeFilename(string f)
        {
            string[] parts = f.Split('\\');
            string curName = parts[parts.Length - 1];
            return curName;
        }

        private static void backupOnlineData()
        {
            if (Directory.Exists(OnlineDataPath + @"downloads\"))
            {
                insertInList("Keeping backup of current Online Data...");
                Directory.CreateDirectory(SaveRootPath + "ODBackup");
                string[] odfiles = Directory.GetFiles(OnlineDataPath + @"downloads\");
                foreach (string f in odfiles)
                {
                    string[] parts = f.Split('\\');
                    string curName = parts[parts.Length - 1];
                    File.Copy(f, SaveRootPath + @"ODBackup\" + curName);
                }

                insertInList("Checking for changes in 2K's updates...");
                string log = App.AppDocsPath + "2Konlinedata_crc.hist";
                if (Directory.Exists(App.AppDocsPath) == false)
                {
                    Directory.CreateDirectory(App.AppDocsPath);
                }
                if (File.Exists(log) == false)
                {
                    saveOnlineDataInfo();
                    return;
                }
                var data = new Dictionary<string, string>();
                var sr = new StreamReader(log);
                while (sr.Peek() > -1)
                {
                    string line = sr.ReadLine();
                    string[] parts = line.Split('\t');
                    if (parts.Length == 2)
                    {
                        data.Add(parts[0], parts[1]);
                    }
                }
                sr.Close();
                bool found = false;
                int newFiles = 0;
                int updFiles = 0;
                foreach (string f in odfiles)
                {
                    if (data.ContainsKey(getSafeFilename(f)))
                    {
                        if (data[getSafeFilename(f)] != getCRC(f))
                        {
                            insertInList("UPDATE! " + getSafeFilename(f) + " has been updated by 2K!");
                            if (_keptmods.Contains(getSafeFilename(f)))
                            {
                                insertInList("You have opted to keep " + getSafeFilename(f) + " modded.");
                            }
                            found = true;
                            updFiles++;
                        }
                    }
                    else
                    {
                        insertInList("NEW FILE! " + getSafeFilename(f) + " is a new update by 2K!");
                        if (_keptmods.Contains(getSafeFilename(f)))
                        {
                            insertInList("You have opted to keep " + getSafeFilename(f) + " modded.");
                        }
                        found = true;
                        newFiles++;
                    }
                }
                if (found)
                {
                    insertInList("ONLINE DATA UPDATE: " + newFiles.ToString() + " new files, " + updFiles.ToString() +
                                 " files updated!");
                    SystemSounds.Beep.Play();
                }
                else
                {
                    insertInList("No changes to Online Data detected.");
                }
                saveOnlineDataInfo();
            }
        }

        private static void saveOnlineDataInfo()
        {
            try
            {
                string[] odfiles = Directory.GetFiles(SaveRootPath + @"ODBackup\");
                var sw = new StreamWriter(App.AppDocsPath + "2Konlinedata_crc.hist");
                long size = 0;
                foreach (string f in odfiles)
                {
                    long fsize = new FileInfo(f).Length;
                    string crc = getCRC(f);
                    sw.WriteLine(getSafeFilename(f) + "\t" + crc);
                    size += fsize;
                }
                sw.WriteLine(size);
                sw.Close();
            }
            catch (DirectoryNotFoundException e)
            {
                insertInList("No original Online Data backup found; won't save Online Data log this time.");
                return;
            }
        }

        private static String getCRC(string filename)
        {
            var crc32 = new Crc32();
            String hash = String.Empty;

            using (FileStream fs = File.Open(filename, FileMode.Open))
            {
                foreach (byte b in crc32.ComputeHash(fs))
                {
                    hash += b.ToString("x2").ToLower();
                }
            }
            return hash;
        }

        private void btnHide_Click(object sender, RoutedEventArgs e)
        {
            if (!isGameRunning())
            {
                hideRosters();
                noUpdate = false;
                paused = false;

                enableAllWatchers();
                insertInList("NOTE: Tool will watch for re-syncs and force your mods when needed.");
                /*
                }
                else
                {
                    insertInList("        (2/2): To restart them, use the \"Force Keep My Mods\" option.");
                    insertInList("WARNING (1/2): The tool's automatic capabilities have been paused by the \"Restore Online Data Backup\" option.");
                }
                */
            }
        }

        private void btnForceKeep_Click(object sender, RoutedEventArgs e)
        {
            keepMods();
            paused = false;
            insertInList("NOTE: Tool will watch for re-syncs and force your mods when needed.");
        }

        private void btnSaveLog_Click(object sender, RoutedEventArgs e)
        {
            readModLists();

            var log = new string[lstLog.Items.Count];
            lstLog.Items.CopyTo(log, 0);
            File.WriteAllLines(UserDekstopPath + "KeepMyMods_Log.txt", log);

            string[] temp = _keptmods.ToArray();
            File.WriteAllLines(UserDekstopPath + "KeepMyMods_Mods.txt", temp);

            MessageBox.Show("Logs saved; they're located on your desktop.");
        }

        public static void insertInList(string s)
        {
            mwInstance.lstLog.Items.Insert(0, DateTime.Now.ToLongTimeString() + " - " + s);
        }

        private void btnReadme_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(AppPath + @"\readme.txt");
        }

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            var fd = new OpenFileDialog();
            fd.Multiselect = true;
            fd.InitialDirectory = InstallationPath;
            fd.ShowDialog();

            if (fd.FileNames.Length == 0)
            {
                return;
            }

            var fs = new SaveFileDialog();
            fs.Filter = "Keep My Mod Modlist (*.modlist)|*.modlist";
            fs.InitialDirectory = KMMPath;
            fs.AddExtension = true;
            fs.ShowDialog();

            if (String.IsNullOrEmpty(fs.FileName))
            {
                return;
            }

            var sw = new StreamWriter(fs.FileName);
            foreach (string f in fd.SafeFileNames)
            {
                sw.WriteLine(f);
            }
            sw.Close();

            insertInList("Successfully created and saved " + fs.SafeFileName);
            Environment.CurrentDirectory = AppPath;
            readModLists();
        }

        private void btnRestoreOD_Click(object sender, RoutedEventArgs e)
        {
            restoreOnlineData();
            insertInList(
                "NOTE: Tool will not watch for re-syncs or force your mods until you use \"Force Keep My Mods\", or \"Hide Rosters\".");
            paused = true;
            if (Directory.Exists(CachePath + "patches"))
            {
                insertInList(
                    "NOTE: You may have to wait for the current re-sync to finish until you can access NBA Today, Online, or other features.");
                insertInList(
                    "NOTE: Watch for the \"Re-syncing...\" prompt under the Online Data browser button. When it disappears, you're good to go.");
            }
        }

        private static void restoreOnlineData(bool onBoot = false, bool onGameStart = false)
        {
            disableAllWatchers();

            if (Directory.Exists(SaveRootPath + "ODBackup"))
            {
                try
                {
                    Directory.Delete(OnlineDataPath + "downloads", true);
                }
                catch
                {
                }

                copyFolder(SaveRootPath + "ODBackup", OnlineDataPath + "downloads");
                Directory.Delete(SaveRootPath + "ODBackup", true);
                insertInList("Restored Online Data backup.");
            }
            else
            {
                insertInList("No original Online Data backup found. Online Data should be the original.");
            }

            if (onBoot)
            {
                enableAllWatchers();
            }
            else if (onGameStart && !paused)
            {
                enableAllWatchers();
            }
        }

        private static void disableAllWatchers()
        {
            mwInstance.odWatcher.disableEvents();
            mwInstance.savesWatcher.disableEvents();
            mwInstance.cacheWatcher.disableEvents();
            insertInList("All watchers disabled!");
            mwInstance.lblLog.Content = "Log (Automatic capabilities: OFF)";
        }

        private static void checkForUpdates()
        {
            //insertInList("Checking for updates...");
            try
            {
                var webClient = new WebClient();
                webClient.DownloadFileCompleted += Completed;
                //webClient.DownloadProgressChanged += new DownloadProgressChangedEventHandler(ProgressChanged);
                webClient.DownloadFileAsync(new Uri("http://users.tellas.gr/~aslan16/kmm13version.txt"), App.AppDocsPath + @"version.txt");
            }
            catch (Exception ex)
            {
                return;
                //insertInList("Check for updates failed. Exception was: " + ex.Message);
            }
        }

        /*
        private void ProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar.Value = e.ProgressPercentage;
        }
        */

        private static void Completed(object sender, AsyncCompletedEventArgs e)
        {
            string[] updateInfo;
            string[] versionParts;
            try
            {
                updateInfo = File.ReadAllLines(App.AppDocsPath + @"version.txt");
                versionParts = updateInfo[0].Split('.');
                string[] curVersionParts = Assembly.GetExecutingAssembly().GetName().Version.ToString().Split('.');
                var iVP = new int[versionParts.Length];
                var iCVP = new int[versionParts.Length];
                bool found = false;
                for (int i = 0; i < versionParts.Length; i++)
                {
                    iVP[i] = Convert.ToInt32(versionParts[i]);
                    iCVP[i] = Convert.ToInt32(curVersionParts[i]);
                    if (iCVP[i] > iVP[i])
                    {
                        break;
                    }
                    if (iVP[i] > iCVP[i])
                    {
                        MessageBoxResult mbr = MessageBox.Show("A new version is available! Would you like to download it?",
                                                               "NBA 2K13 Keep My Mod", MessageBoxButton.YesNo,
                                                               MessageBoxImage.Information);
                        found = true;
                        if (mbr == MessageBoxResult.Yes)
                        {
                            Process.Start(updateInfo[1]);
                            break;
                        }
                    }
                }
                //if (!found) insertInList("Check for updates finished, no new version found.");
            }
            catch
            {
                //insertInList("Check for updates failed.");
                return;
            }
        }

        private void btnLicense_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad", AppPath + @"\LICENSE");
        }

        public static string KMMPath;
    }
}