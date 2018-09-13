using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Windows;

namespace RottweilerLib
{
    /// <summary>
    /// IWD Logic
    /// </summary>
    public class IWD
    {
        /// <summary>
        /// Looks for .wav files in an IWD/ZIP file.
        /// </summary>
        /// <param name="fileName">File name</param>
        /// <param name="mainWin">Main Window</param>
        public static List<Sound> Load(string fileName)
        {
            List<Sound> sounds = new List<Sound>();
            // Load IWD using Zip
            using (ZipArchive zip = new ZipArchive(new FileStream(fileName, FileMode.Open)))
            {
                // Loop through and find wav files
                foreach (ZipArchiveEntry entry in zip.Entries)
                {
                    // Found WAV file
                    if (Path.GetExtension(entry.Name).ToLower() == ".wav")
                    {
                        sounds.Add(new Sound()
                        {
                            FilePath = entry.FullName,
                            Size = (int)entry.Length,
                            Location = "IWD",
                        });
                    }
                }
            }

            return sounds;
        }
    }
}
