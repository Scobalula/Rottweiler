/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using PhilUtil;
using RottweilerLib;
using Rottweiler.Windows;
using Microsoft.Win32;

namespace Rottweiler
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Currently Loaded Fast File
        /// </summary>
        public static FastFile ActiveFastFile { get; set; }

        /// <summary>
        /// List View (For Filtering)
        /// </summary>
        public static CollectionView View { get; set; }

        /// <summary>
        /// Version as float
        /// </summary>
        public float CurrentVersion = 0.80f;

        /// <summary>
        /// Version as a string
        /// </summary>
        public static string Version = "v1.0-release";

        /// <summary>
        /// Main Sausages
        /// </summary>
        public MainWindow()
        {
            new Thread(delegate () { Updater.CheckForUpdates(CurrentVersion, this); }).Start();
            Logger.ActiveLogger = new Logger("Rottweiler Log - Doggo is active", "Rottweiler-Log.txt");
            InitializeComponent();
            Game.RegisterGames();
        }

        /// <summary>
        /// Displays File Dialog to load a Fast File
        /// </summary>
        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Filter = "Fast File (.ff)|*.ff"
            };

            if (dlg.ShowDialog() == true)
            {
                ClearLoadedData();

                string extension = Path.GetExtension(dlg.FileName);

                if (extension == ".ff")
                    LoadFastFile(dlg.FileName);
            }
        }

        /// <summary>
        /// Filters Sounds by text in Search Box
        /// </summary>
        public bool ViewFilter(object obj)
        {
            return string.IsNullOrEmpty(SearchBox.Text) ? true : (obj.ToString().IndexOf(SearchBox.Text, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        public void LoadFastFile(string fileName)
        {
            if(!File.Exists(fileName))
            {
                FastFile.LogError(String.Format("Cannot find file {0}.", Path.GetFileNameWithoutExtension(fileName)));
            }
            else if(!FileUtil.CanAccessFile(fileName))
            {
                FastFile.LogError(String.Format("Cannot access file {0}, file in use or permissions denied.", Path.GetFileNameWithoutExtension(fileName)));
            }
            else
            {
                ProgressWindow progressWindow = new ProgressWindow
                {
                    Owner = this
                };

                progressWindow.label.Content = "Decompressing Fast File....";
                Dispatcher.BeginInvoke(new Action(() => progressWindow.ShowDialog()));

                DimBox.Visibility = Visibility.Visible;
                new Thread(
                    delegate ()
                    {
                        ActiveFastFile = FastFile.Load(fileName, progressWindow.UpdateProgress);
                        progressWindow.isWorking = false;
                        Dispatcher.BeginInvoke(new Action(() => progressWindow.Close()));
                        Logger.ActiveLogger.CloseStream();
                        Dispatcher.Invoke(
                            () =>
                            {
                                if (ActiveFastFile != null)
                                {
                                    Sounds.ItemsSource = ActiveFastFile.Sounds;

                                    View = CollectionViewSource.GetDefaultView(Sounds.ItemsSource) as CollectionView;
                                    View.Filter = ViewFilter;

                                    SoundsLoadedLabel.Content = String.Format("{0} Sounds Loaded", ActiveFastFile.Sounds.Count);
                                }
                                else
                                {
                                    ClearLoadedData();
                                }
                                DimBox.Visibility = Visibility.Hidden;
                            }
                            );
                    }).Start();
            }
        }

        /// <summary>
        /// Clears all Loaded Sounds and Data
        /// </summary>
        private void ClearSoundsButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLoadedData();
        }

        /// <summary>
        /// Clears all loaded data and streams
        /// </summary>
        private void ClearLoadedData()
        {
            ActiveFastFile?.DeleteDecompressedFile();
            ActiveFastFile = null;
            Sounds.ItemsSource = null;
            SoundsLoadedLabel.Content = "0 Sounds Loaded";
            RottweilerUtil.ActiveGame = null;
            Sound.Exporter.AudioStreams.Clear();
            Sound.Exporter.AudioLocations.Clear();
        }

        /// <summary>
        /// Clears all Loaded Sounds and Data on close
        /// </summary>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ClearLoadedData();
        }

        /// <summary>
        /// Initiates Export with Asset List and a Message to Display if no assets are given
        /// </summary>
        public void InitExport(List<Sound> sounds, string zeroAssetsMessage)
        {
            if (sounds.Count == 0)
            {
                MessageBox.Show(zeroAssetsMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {

                ProgressWindow progressWindow = new ProgressWindow
                {
                    Owner = this
                };

                ExportFinishedWindow exportFinishedWindow = new ExportFinishedWindow
                {
                    Owner = this
                };

                progressWindow.label.Content = String.Format("Exporting {0} Sounds....", sounds.Count);
                Dispatcher.BeginInvoke(new Action(() => progressWindow.ShowDialog()));
                DimBox.Visibility = Visibility.Visible;
                new Thread(
                    delegate ()
                    {
                        Sound.Exporter.ExportSounds(sounds, progressWindow.UpdateProgress);
                        progressWindow.isWorking = false;
                        Dispatcher.BeginInvoke(new Action(() => progressWindow.Close()));

                        Dispatcher.Invoke(() =>
                        {
                            exportFinishedWindow.ShowDialog();
                            DimBox.Visibility = Visibility.Hidden;
                            Activate();
                        });

                    }).Start();
            }
        }

        /// <summary>
        /// Exports all listed sounds
        /// </summary>
        private void ExportAllButton_Click(object sender, RoutedEventArgs e)
        {
            InitExport(Sounds.Items.Cast<Sound>().ToList(), "No sounds loaded to export");
        }

        /// <summary>
        /// Exports Select Sounds
        /// </summary>
        private void ExportSelectedButton_Click(object sender, RoutedEventArgs e)
        {
            InitExport(Sounds.SelectedItems.Cast<Sound>().ToList(), "No sounds selected to export");
        }

        /// <summary>
        /// Updates View Filter on Search
        /// </summary>
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CollectionViewSource.GetDefaultView(Sounds.ItemsSource)?.Refresh();
        }

        /// <summary>
        /// Clears Search Box
        /// </summary>
        private void ClearSearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        /// <summary>
        /// Opens About Window
        /// </summary>
        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow()
            {
                Owner = this
            };
            aboutWindow.VersionLabel.Content = String.Format("Version: {0}", Version);
            DimBox.Visibility = Visibility.Visible;
            aboutWindow.ShowDialog();
            DimBox.Visibility = Visibility.Hidden;
        }
    }
}
