﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace NBA_2K13_Keep_My_Mod
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // Add code to output the exception details to a message box/event log/log file,   etc.
            // Be sure to include details about any inner exceptions
            try
            {
                if (Directory.Exists(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath) == false)
                {
                    Directory.CreateDirectory(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath);
                }
                var f = new StreamWriter(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog_unh.txt");
                //StreamWriter f = new StreamWriter(NBA_2K13_Keep_My_Mod.MainWindow.SaveRootPath + @"\errorlog_unh.txt");

                f.Write(e.Exception.ToString());
                f.WriteLine();
                f.WriteLine();
                f.Write(e.Exception.InnerException == null ? "None" : e.Exception.InnerException.Message);
                f.WriteLine();
                f.WriteLine();
                f.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't create errorlog!\n\n" + ex + "\n\n" + ex.InnerException);
            }

            if (NBA_2K13_Keep_My_Mod.MainWindow.bootSuccess)
            {
                var log = new string[NBA_2K13_Keep_My_Mod.MainWindow.mwInstance.lstLog.Items.Count];
                NBA_2K13_Keep_My_Mod.MainWindow.mwInstance.lstLog.Items.CopyTo(log, 0);
                //File.WriteAllLines(NBA_2K13_Keep_My_Mod.MainWindow.SaveRootPath + @"\errorlog_unh.txt", log);
                File.AppendAllLines(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog_unh.txt", log);
            }
            MessageBox.Show(
                "NBA 2K13 Keep My Mod encountered a critical error and will be terminated.\n\nAn Error Log has been saved at " +
                NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog_unh.txt");

            Process.Start(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog_unh.txt");

            // Prevent default unhandled exception processing
            e.Handled = true;

            Environment.Exit(-1);
        }

        public static void errorReport(Exception e, string additional = "")
        {
            // Add code to output the exception details to a message box/event log/log file,   etc.
            // Be sure to include details about any inner exceptions
            try
            {
                //StreamWriter f = new StreamWriter(NBA_2K13_Keep_My_Mod.MainWindow.SaveRootPath + @"\errorlog.txt");
                if (Directory.Exists(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath) == false)
                {
                    Directory.CreateDirectory(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath);
                }
                var f = new StreamWriter(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog.txt");

                f.WriteLine("Additional: " + additional);
                f.WriteLine();
                f.Write(e.ToString());
                f.WriteLine();
                f.WriteLine();
                f.Write(e.InnerException == null ? "None" : e.InnerException.Message);
                f.WriteLine();
                f.WriteLine();
                f.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't create errorlog!\n\n" + ex + "\n\n" +
                                (e.InnerException == null ? "None" : e.InnerException.Message));
            }

            if (NBA_2K13_Keep_My_Mod.MainWindow.bootSuccess)
            {
                var log = new string[NBA_2K13_Keep_My_Mod.MainWindow.mwInstance.lstLog.Items.Count];
                NBA_2K13_Keep_My_Mod.MainWindow.mwInstance.lstLog.Items.CopyTo(log, 0);
                //File.AppendAllLines(NBA_2K13_Keep_My_Mod.MainWindow.SaveRootPath + @"\errorlog.txt", log);
                File.AppendAllLines(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog.txt", log);
            }
            MessageBox.Show(
                "NBA 2K13 Keep My Mod encountered a critical error and will be terminated.\n\nAn Error Log has been saved at " +
                NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog.txt");

            Process.Start(NBA_2K13_Keep_My_Mod.MainWindow.AppDocsPath + @"errorlog_unh.txt");

            Environment.Exit(-1);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            // Get Reference to the current Process
            Process thisProc = Process.GetCurrentProcess();
            // Check how many total processes have the same name as the current one
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                // If ther is more than one, than it is already running.
                MessageBox.Show("Application is already running.");
                Current.Shutdown();
                return;
            }

            base.OnStartup(e);
        }
    }
}