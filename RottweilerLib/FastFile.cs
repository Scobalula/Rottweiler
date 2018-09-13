/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using PhilUtil;
using ZLibNet;
using System.Reflection;
using System.Runtime.InteropServices;

namespace RottweilerLib
{
    /// <summary>
    /// FastFile Magic Numbers
    /// </summary>
    public enum Magic
    {
        InfinityWard = 0x66665749,
        Treyarch     = 0x66664154,
        Sledgehammer = 0x66663153,
    }

    /// <summary>
    /// Fast File Header
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    struct Header
    {
        /// <summary>
        /// Fast File Magic
        /// </summary>
        public Magic Magic;

        /// <summary>
        /// Fast File Signed Int
        /// </summary>
        public int Signed;

        /// <summary>
        /// Fast File Version
        /// </summary>
        public int Version;
    };

    /// <summary>
    /// Holds Game Information
    /// </summary>
    public class Game
    {
        /// <summary>
        /// Registered Model Converters
        /// </summary>
        public static readonly List<Game> Games = new List<Game>();

        /// <summary>
        /// Game Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Fast File Magic
        /// </summary>
        public Magic Magic { get; set; }

        /// <summary>
        /// Fast File Version
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// Load Method for this Game
        /// </summary>
        public Func<BinaryReader, string, bool, Func<float, bool>, FastFile> LoadMethod { get; set; }

        /// <summary>
        /// Registers Game
        /// </summary>
        public void Register()
        {
            Games.Add(this);
        }

        /// <summary>
        /// Default Load Method
        /// </summary>
        public virtual FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            return null;
        }

        /// <summary>
        /// Registers supported games
        /// </summary>
        public static void RegisterGames()
        {
            foreach (var type in Assembly.GetExecutingAssembly().GetTypes().Where(x => x.BaseType == typeof(Game)))
                Activator.CreateInstance(type);
        }

        /// <summary>
        /// Gets the matching game for magic and version
        /// </summary>
        public static Game GetGame(Magic magic, int version)
        {
            foreach (var game in Games)
                if (magic == game.Magic && version == game.Version)
                    return game;

            return null;
        }
    }

    /// <summary>
    /// Holds Fast File Information
    /// </summary>
    public class FastFile
    {
        /// <summary>
        /// Fast File 
        /// </summary>
        public string Filename { get; set; }

        /// <summary>
        /// Decompressed Filename
        /// </summary>
        public string DecompressedFilename { get { return Filename + ".decompressed"; } }

        /// <summary>
        /// Sounds Loaded from Fast File
        /// </summary>
        public List<Sound> Sounds = new List<Sound>();

        /// <summary>
        /// Fast File Utilities
        /// </summary>
        public class Utils
        {
            /// <summary>
            /// Handles calculating byte padding, from PyCod's xBin py 
            /// (https://github.com/dtzxporter/PyCod/blob/master/PyCod/xbin.py#L24)
            /// </summary>
            static long CalculateBytePadding(long size)
            {
                return (size + 0x3) & 0xFFFFFFFFFFFFFC;
            }

            /// <summary>
            /// Decompresses a ZLIB Fast File
            /// </summary>
            /// <param name="stream">ZLIB Stream</param>
            /// <param name="outputName">Output Filename</param>
            /// <param name="chunkSize">Decompression Chunk Size</param>
            /// <param name="ProgressCallBack">Progress Callback, if returns false the decompression will end.</param>
            public static bool DecompressZlib(Stream stream, string outputName, Func<float, bool> ProgressCallBack = null, int chunkSize = 2097152)
            {
                if (ProgressCallBack == null)
                    ProgressCallBack = delegate (float val) { return true; };

                // Skip ZLIB Header
                stream.Seek(2, SeekOrigin.Current);

                using (var output = new FileStream(outputName, FileMode.Create))
                {
                    using (var deflate = new DeflateStream(stream, CompressionMode.Decompress))
                    {
                        byte[] chunk = new byte[chunkSize];


                        int bytesRead;

                        while ((bytesRead = deflate.Read(chunk, 0, chunk.Length)) > 0 && ProgressCallBack(((deflate.BaseStream.Position) / (float)(deflate.BaseStream.Length)) * (float)100.000))
                            output.Write(chunk, 0, bytesRead);

                        if (!ProgressCallBack(100))
                            return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Decompresses an LZ4 Fast File
            /// </summary>
            /// <param name="stream">LZ4 Data Stream (All blocks)</param>
            /// <param name="outputName">Output Filename</param>
            /// <param name="numBlocks">Number of LZ4 Blocks</param>
            /// <param name="ProgressCallBack">Progress Callback, if returns false the decompression will end.</param>
            public static bool DecompressLZ4(Stream stream, string outputName, int numBlocks, Func<float, bool> ProgressCallBack)
            {
                using (BinaryReader input = new BinaryReader(stream))
                {
                    using (BinaryWriter output = new BinaryWriter(new FileStream(outputName, FileMode.Create)))
                    {
                        for (int i = 0; i < numBlocks && ProgressCallBack((i / (float)(numBlocks)) * (float)100.000); i++)
                        {
                            int blockSize = input.ReadInt32();
                            int decomSize = input.ReadInt32();

                            long start = input.BaseStream.Position;

                            output.Write(LZ4Util.Decode(input.ReadBytes(blockSize), decomSize));

                            // Seek to next 
                            input.Seek(start + ((input.BaseStream.Position - start) + 0x3) & 0xFFFFFFFFFFFFFC, SeekOrigin.Begin);
                        }

                        if (!ProgressCallBack(100))
                            return false;
                    }
                }

                return true;
            }

            /// <summary>
            /// Handles removing Hash Blocks from Signed Fast Files
            /// </summary>
            /// <param name="input">Fast File Input</param>
            /// <param name="initialSize">Initial Hash Block Size</param>
            /// <param name="blockSize">Number of bytes between each hash block.</param>
            /// <param name="hashSize">Size of each hash block.</param>
            /// <returns>Processed Fast File as a memory stream.</returns>
            public static BinaryReader RemoveHashBlocks(BinaryReader input, int initialSize, int blockSize, int hashSize)
            {
                input.Seek(initialSize, SeekOrigin.Current);

                // Create new memory stream (most signed fast files are < 300MB~)
                MemoryStream output = new MemoryStream();

                byte[] buffer = new byte[blockSize];
                int bytesRead;

                // Copy cleansed bytes to memory stream
                while ((bytesRead = input.BaseStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    output.Write(buffer, 0, buffer.Length);
                    input.Seek(hashSize, SeekOrigin.Current);
                }

                // Flush Stream and reset position
                output.Flush();
                output.Position = 0;

                return new BinaryReader(output);
            }
        }

        /// <summary>
        /// Loads a Call of Duty Fast File
        /// </summary>
        /// <param name="fileName">Fast File Path</param>
        /// <returns>If supported and load was successful, a fast file is returned, otherwise null.</returns>
        public static FastFile Load(string fileName, Func<float, bool> progressCallBack = null)
        {
            if (progressCallBack == null)
                progressCallBack = delegate (float val) { return true; };

            using (BinaryReader reader = new BinaryReader(new FileStream(fileName, FileMode.Open)))
            {
                var ffHeader = reader.ReadStruct<Header>();

                Log(String.Format("Loading {0}", fileName));

                RottweilerUtil.ActiveGame = Game.GetGame(ffHeader.Magic, ffHeader.Version);

                if (RottweilerUtil.ActiveGame == null)
                {
                    LogError(String.Format("Fast File Version 0x{0:X} with Magic 0x{1:X} is not supported.", 
                        ffHeader.Magic, 
                        ffHeader.Version));
                    return null;
                }

                Log(String.Format("Recognized Fast File from game: {0}", RottweilerUtil.ActiveGame.Name));

                // Some games store sounds in the PAK (AW/MWR/WW2) so run a scan on the folder
                Sound.PAK.Find(Path.GetDirectoryName(fileName));

                string decodedFastFile = fileName + ".decompressed";

                FastFile fastFile = null;

                try
                {
                    Log("Decompressing and loading audio from fast file");

                    var watch = Stopwatch.StartNew();

                    fastFile          = RottweilerUtil.ActiveGame.Load(reader, decodedFastFile, ffHeader.Signed == 0x30303130, progressCallBack);
                    fastFile.Sounds   = fastFile.Sounds?.OrderBy(x => x.FilePath).ToList();
                    fastFile.Filename = fileName;

                    watch.Stop();

                    // If we didn't hit 100% we either failed or user cancelled
                    if (!progressCallBack(100))
                    {
                        fastFile?.DeleteDecompressedFile();
                        RottweilerUtil.ActiveGame = null;
                        return null;
                    }

                    Log(String.Format("Decompressed and loaded {1} sounds from fast file in {0} seconds", 
                        watch.ElapsedMilliseconds / 1000.0, 
                        fastFile.Sounds.Count));
                }
                catch(Exception e)
                {
                    LogError(String.Format("An unhandled Exception occured:\n\n{0}", e));
                    RottweilerUtil.ActiveGame = null;
                    fastFile?.DeleteDecompressedFile();
                    return null;
                }

                Sound.Exporter.AudioLocations["FastFile"] = decodedFastFile;

                return fastFile;
            }
        }

        /// <summary>
        /// Handles deleting the decompressed fast file
        /// </summary>
        public void DeleteDecompressedFile()
        {
            if (Filename != null)
            {
                if (File.Exists(DecompressedFilename))
                {
                    try
                    {
                        File.Delete(DecompressedFilename);
                    }
                    catch
                    {
                        Logger.ActiveLogger.Write(String.Format("Failed to delete decompressed fast file."), MessageType.ERROR);
                    }
                }
            }
        }

        /// <summary>
        /// Writes to Log
        /// </summary>
        /// <param name="message">Message to output</param>
        public static void Log(object message)
        {
            Logger.ActiveLogger?.Write(message.ToString(), MessageType.INFO);
        }

        /// <summary>
        /// Logs an error and flashes an error window
        /// </summary>
        /// <param name="message">Message to output</param>
        public static void LogError(object message)
        {
            Logger.ActiveLogger?.Write(message.ToString(), MessageType.ERROR);
            MessageBox.Show(message.ToString(), "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
