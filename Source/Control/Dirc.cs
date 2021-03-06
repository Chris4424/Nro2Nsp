﻿using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MtX.Control.Properties;


namespace MtX.Control
{
    public class DircControl
    {
        public string TitleId { get; set; }
        public string AppName { get; set; }
        public static string current_directory = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                             user = System.IO.Path.GetPathRoot(Environment.CurrentDirectory),
                             root = user + @"/Nro2Nsp/",
                             buildpath = user + @"/Nro2Nsp/temp/",
                             sourcepath = @"source/",
                             exefspath = @"exefs/",
                             romfspath = @"romfs/",
                             controlpath = @"control/",
                             logopath = @"logo/",
                             RootDrive = System.IO.Path.GetPathRoot(Environment.CurrentDirectory);


        //Build working dirc
        public static void BuildTemp()
        {
            try
            {   // Build working paths and copy requried files
                System.IO.Directory.CreateDirectory(buildpath);
                System.IO.Directory.CreateDirectory(buildpath + sourcepath);
                System.IO.Directory.CreateDirectory(buildpath + exefspath);
                System.IO.Directory.CreateDirectory(buildpath + logopath);
                if (Directory.Exists(buildpath + romfspath)) { }
                else { System.IO.Directory.CreateDirectory(buildpath + romfspath); }
                if (Directory.Exists(buildpath + controlpath)) { }
                else { System.IO.Directory.CreateDirectory(buildpath + controlpath); }
                File.WriteAllBytes(buildpath + sourcepath + "trampoline.s", Properties.Resources.lind);
                if (File.Exists(@"./Resources/log.txt"))
                {
                    File.Delete(@"./Resources/log.txt");
                }

                switch (Tools.RunningPlatform())
                {
                    case Tools.Platform.Linux:
                        File.WriteAllBytes(buildpath + "hacbrewpack", Properties.Resources.hacbrewpackx86);
                        break;

                    case Tools.Platform.Mac:
                        File.WriteAllBytes(buildpath + "hacbrewpack", Properties.Resources.hacbrewpack_mac);
                        break;

                    case Tools.Platform.Windows:
                        if (Environment.Is64BitOperatingSystem) { File.WriteAllBytes(buildpath + "hacbrewpack.exe", Properties.Resources.hacbrewpackx64); }
                        else { File.WriteAllBytes(buildpath + "hacbrewpack.exe", Properties.Resources.hacbrewpackx86); }
                        break;
                    default:
                        MessageBox.Show("Couldnt Detect Operating System, This is probably a bug, please report it");
                        break;
                }
                File.Copy(@"Resources/logo/NintendoLogo.png", @buildpath + logopath + "NintendoLogo.png", true);
                File.Copy(@"Resources/logo/StartupMovie.gif", @buildpath + logopath + "StartupMovie.gif", true);
                File.Copy(@"Resources/keys.dat", @buildpath + "keys.dat", true);
            }
            catch (Exception e)
            { MessageBox.Show(e.Message + " -- File copying failed, make sure the file missing is int the \"Resources\" next to the Application"); }
        }


        //Clean Working Directory
        public static void CleanTemp()
        {
            try
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (Directory.Exists(root)) Directory.Delete(root, true);
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }


        //Clean Log at start
        public static void CleanLog()
        {
            try
            { 
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (File.Exists(System.IO.Path.Combine(current_directory, @"./Resources/log.txt"))) File.Delete(System.IO.Path.Combine(current_directory, @"./Resources/log.txt"));
            }
            catch (Exception e) { MessageBox.Show(e.Message); }
        }


        //Copy completed nsp to working dirc
        public void OutputCopy()
        { 
            string nspsource = @buildpath;
            String nsp = TitleId + ".nsp";
            string nspsourceFile = System.IO.Path.Combine(nspsource, nsp);
            string filename = Control.Tools.RemoveSpecialCharacters(AppName + "_" + TitleId + ".nsp");
            string nspdestFile = System.IO.Path.Combine(current_directory, filename);
            string RawNca = buildpath + "nca";
            string RawExefs = buildpath + "build/exefs";
            string RawControl = buildpath + controlpath;
            
            MessageBox.Show("Your Nsp \"" + AppName + "\" Was Successfully Created!");
            try {
                System.IO.File.Copy(nspsourceFile, nspdestFile, true);
                if (Convert.ToBoolean(Settings.Default["PerserveDataEnable"]) == true)
                {
                    PerseveCopy(RawControl, System.IO.Path.Combine(current_directory, "RawData/control/"));
                    PerseveCopy(RawExefs, System.IO.Path.Combine(current_directory, "RawData/exefs/"));
                    PerseveCopy(RawNca, System.IO.Path.Combine(current_directory, "RawData/nca/"));
                }
            }
            catch (Exception e)
            { MessageBox.Show(e.Message + System.Environment.NewLine + "It appears building the nsp failed, please verify the proper keys are in keys.dat file -- refer to Keys.dat template.txt");}
        }


        //Save data if set
        void PerseveCopy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);

            foreach (var file in Directory.GetFiles(sourceDir))
                File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)), true);

            foreach (var directory in Directory.GetDirectories(sourceDir))
                PerseveCopy(directory, Path.Combine(targetDir, Path.GetFileName(directory)));
        }
    }
}
