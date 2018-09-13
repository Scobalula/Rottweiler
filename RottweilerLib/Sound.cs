/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PhilUtil;

namespace RottweilerLib
{
    /// <summary>
    /// Holds Fast File Audio Data
    /// </summary>
    public class Sound
    {
        /// <summary>
        /// Accepted Sample Rates
        /// </summary>
        public static int[] AcceptedFrameRates =
        {
            16000,
            22050,
            32000,
            36000,
            37800,
            44100,
            48000
        };

        /// <summary>
        /// Audio Extensions and matching magic
        /// </summary>
        public static Dictionary<int, string> Extensions = new Dictionary<int, string>()
        {
            { 0x43614C66, ".flac" },
            { 0x46464952, ".wav" },
        };

        /// <summary>
        /// Sound Formats
        /// </summary>
        public enum Formats
        {
            FLAC,
            PCM,
            ADPCM,
            XWMA,
            UNKNOWN,
        }

        /// <summary>
        /// Sound File Path
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// Sound Name
        /// </summary>
        public string Name { get { return Path.GetFileNameWithoutExtension(FilePath); } }

        /// <summary>
        /// Sound Format
        /// </summary>
        public Formats Format { get; set; }

        /// <summary>
        /// Data Size
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Display Size String (In KB)
        /// </summary>
        public string DisplaySize { get { return (Size / 1024.0).ToString("0.00") + "KB"; } }

        /// <summary>
        /// Frame Rate
        /// </summary>
        public int FrameRate { get; set; }

        /// <summary>
        /// Number of Frames
        /// </summary>
        public int Frames { get; set; }

        /// <summary>
        /// Number of Channels
        /// </summary>
        public int Channels { get; set; }

        /// <summary>
        /// Audio Length in Milliseconds
        /// </summary>
        public int Length { get { return (int)(1000 * ((float)Frames / FrameRate)); } }

        /// <summary>
        /// Audio Length as a Human readable length
        /// </summary>
        public string DisplayLength { get { return DurationToReadableTime(Length); } }

        /// <summary>
        /// Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Position in Location
        /// </summary>
        public long Position { get; set; }

        /// <summary>
        /// Returns Sound Name
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FilePath;
        }

        /// <summary>
        /// Returns Milliseconds as a Readable Time String 
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        private static string DurationToReadableTime(int ms)
        {
            // If we're 0 or less we haven't got a valid length
            if (ms < 1)
                return "N/A";

            var time = TimeSpan.FromMilliseconds(ms);

            return string.Format("{0}{1}{2}",
                time.Hours > 0 ? String.Format("{0}h ", time.Hours) : "",
                time.Minutes > 0 ? String.Format("{0}m ", time.Minutes) : "",
                String.Format("{0}.{1}s ", time.Seconds, time.Milliseconds)
                );
        }

        /// <summary>
        /// Sound Exporter Utils
        /// </summary>
        public class Exporter
        {
            /// <summary>
            /// Audio Location File Names and ID
            /// </summary>
            public static Dictionary<string, string> AudioLocations = new Dictionary<string, string>();

            /// <summary>
            /// Audio Streams
            /// </summary>
            public static Dictionary<string, BinaryReader> AudioStreams = new Dictionary<string, BinaryReader>();

            /// <summary>
            /// Disposes all loaded audio locations 
            /// </summary>
            public static void ClearAudioStreams()
            {
                foreach (var stream in AudioStreams)
                    stream.Value?.Dispose();

                AudioStreams.Clear();
            }

            /// <summary>
            /// Streams possible audio locations
            /// </summary>
            public static void StreamAudioLocations()
            {
                FastFile.Log("Streaming audio locations");

                foreach(var audioLocation in AudioLocations)
                {
                 
                    FastFile.Log(String.Format("Streaming File: {0}", audioLocation.Value));

                    if (!File.Exists(audioLocation.Value))
                    {
                        FastFile.Log(String.Format("ERROR: Could not find file: {0}", audioLocation.Value));
                        continue;
                    }

                    if (!FileUtil.CanAccessFile(audioLocation.Value))
                    {
                        FastFile.Log(String.Format("ERROR: Cannot access file: {0}", audioLocation.Value));
                        continue;
                    }

                    AudioStreams[audioLocation.Key] = new BinaryReader(new FileStream(audioLocation.Value, FileMode.Open));
                }
            }

            /// <summary>
            /// Exports a list of sounds
            /// </summary>
            public static void ExportSounds(List<Sound> sounds, Func<float, bool> ProgressCallBack = null)
            {
                if (ProgressCallBack == null)
                    ProgressCallBack = delegate (float val) { return true; };

                StreamAudioLocations();

                for(int i = 0; i < sounds.Count && ProgressCallBack((i / (float)(sounds.Count)) * (float)100.000); i++)
                {
                    try
                    {
                        ExportSound(sounds[i]);
                    }
                    catch(Exception e)
                    {
                        FastFile.Log(String.Format("ERROR: Unhandled Exception Occured: \n{0}.", e));
                    }

                }

                ClearAudioStreams();
            }

            /// <summary>
            /// Exports a sound
            /// </summary>
            private static void ExportSound(Sound sound)
            {

                FastFile.Log(String.Format("Exporting sound: {0}", sound.FilePath));

                if (!AudioStreams.ContainsKey(sound.Location))
                {
                    FastFile.Log(String.Format("ERROR: Audio location not loaded: {0}", sound.Location));
                    return;
                }

                BinaryReader reader = AudioStreams[sound.Location];

                // Check if the file contains this offset
                if(sound.Position > reader.BaseStream.Length)
                {
                    FastFile.Log(String.Format("ERROR: Audio Location {0} does not contain offset 0x{1:X}.", sound.Location, sound.Position));
                    return;
                }

                // Check if the file can provide the required buffer
                if (sound.Position + sound.Size > reader.BaseStream.Length)
                {
                    FastFile.Log("ERROR: Unexpected EOF.");
                    return;
                }

                string outputPath = Path.Combine("exported_audio", RottweilerUtil.ActiveGame.Name, Path.ChangeExtension(sound.FilePath, null));

                PathUtil.CreateFilePath(outputPath);

                reader.Seek(sound.Position, SeekOrigin.Begin);

                byte[] buffer = reader.ReadBytes(sound.Size);
                int fourCC    = BitConverter.ToInt32(buffer, 0);

                // If buffer matches FLAC or WAV fourcc we can just dump the audio, otherwise assume PCM Wave
                if(fourCC == 0x43614C66 || fourCC == 0x46464952)
                    File.WriteAllBytes(outputPath + Extensions[fourCC], buffer);
                else
                    WAVUtil.WriteWavFile(outputPath + ".wav", sound.FrameRate, sound.Channels, buffer);
            }
        }

        /// <summary>
        /// Sound Pak Util
        /// </summary>
        public class PAK
        {
            /// <summary>
            /// Gets PAK file ID/Number
            /// </summary>
            public static string GetPakID(string filePath)
            {
                return Regex.Match(Path.GetFileNameWithoutExtension(filePath), @"\d+").Value;
            }

            /// <summary>
            /// Handles searching a given directory for PAK files
            /// </summary>
            public static void Find(string directory)
            {
                FastFile.Log("Searching for Sound PAKs");

                foreach (string pakFile in Directory.GetFiles(directory, "*.pak", SearchOption.TopDirectoryOnly))
                    if (pakFile.Contains("soundfile"))
                        Exporter.AudioLocations[string.Format("Pak {0}", GetPakID(pakFile))] = pakFile;

                FastFile.Log(String.Format("Found {0} Sound PAKs", Exporter.AudioLocations.Count));
            }
        }
    }
}
