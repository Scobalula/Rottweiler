/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using Newtonsoft.Json;

namespace Rottweiler
{
    /// <summary>
    /// Basic Update Checker
    /// </summary>
    class Updater
    {
        /// <summary>
        /// Github Application Release Data
        /// </summary>
        public class ApplicationRelease
        {
            /// <summary>
            /// Version from Tag
            /// </summary>
            [JsonProperty("tag_name")]
            public string Version { get; set; }

            /// <summary>
            /// Release URL
            /// </summary>
            [JsonProperty("html_url")]
            public string URL { get; set; }
        }

        /// <summary>
        /// Checks for Updates from Github
        /// </summary>
        public static void CheckForUpdates(float currentVersion, MainWindow mainWindow)
        {
            try
            {
                float latestVersion = 0;
                string latestURL = "";
                var webRequest = WebRequest.Create(@"https://api.github.com/repos/Scobalula/Rottweiler/releases") as HttpWebRequest;

                webRequest.UserAgent = "HydraX";

                using (var reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {

                    List<ApplicationRelease> applicationReleases = LoadGithubReleases(reader);

                    foreach (var release in applicationReleases)
                    {
                        var match = Regex.Match(release.Version, @"([-+]?[0-9]*\.?[0-9]+)");

                        if (match.Success)
                        {
                            if (Single.TryParse(match.Groups[0].Value, out float releaseVersion))
                            {
                                if (releaseVersion > latestVersion)
                                {
                                    latestVersion = releaseVersion;
                                    latestURL = release.URL;
                                }
                            }
                        }
                    }
                }

                if (latestVersion > currentVersion)
                {

                    mainWindow.Dispatcher.Invoke(
                        () =>
                        {
                            var result = MessageBox.Show("A new version of Rottweiler is available, do you want to download it now?", 
                                "Update Available",
                                MessageBoxButton.YesNo, 
                                MessageBoxImage.Information);

                            if (result == MessageBoxResult.Yes)
                                System.Diagnostics.Process.Start(latestURL);
                        });
                }
            }
            catch
            {
                return;
            }

            return;
        }

        /// <summary>
        /// Loads Github Releases 
        /// </summary>
        private static List<ApplicationRelease> LoadGithubReleases(StreamReader input)
        {
            return JsonConvert.DeserializeObject<List<ApplicationRelease>>(input.ReadToEnd());
        }
    }
}
