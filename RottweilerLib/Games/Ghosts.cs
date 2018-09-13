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
using System.Linq;
using PhilUtil;
using System.Runtime.InteropServices;

namespace RottweilerLib.Games
{
    /// <summary>
    /// Ghosts Logic
    /// </summary>
    class Ghosts : Game
    {
        /// <summary>
        /// Ghosts Sound Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct GhostsSound
        {
            /// <summary>
            /// Sound Name Pointer
            /// </summary>
            public ulong NamePointer;

            /// <summary>
            /// Sound Data Pointer
            /// </summary>
            public ulong SoundDataPtr;

            /// <summary>
            /// Null Bytes
            /// </summary>
            public ulong Padding;

            /// <summary>
            /// Sound Frame Rate
            /// </summary>
            public uint FrameRate;

            /// <summary>
            /// Sound Data Size
            /// </summary>
            public uint SoundDataSize;

            /// <summary>
            /// Sound Frame Count
            /// </summary>
            public uint FrameCount;

            /// <summary>
            /// Sound Byte Rate
            /// </summary>
            public uint ByteRate;

            /// <summary>
            /// Channels Count
            /// </summary>
            public ushort Channels;

            /// <summary>
            /// Number of Bits Per Each Sample
            /// </summary>
            public ushort BitsPerSample;

            /// <summary>
            /// ??? (2 shorts (Could relate to format as Flac is allowed))
            /// </summary>
            public uint UnknownInt;

            /// <summary>
            /// Full Data Size
            /// </summary>
            public ulong DataSize;
        };

        /// <summary>
        /// Initializes Ghosts Support
        /// </summary>
        public Ghosts()
        {
            Name    = "Ghosts";
            Magic   = Magic.InfinityWard;
            Version = 0x235;
            Register();
        }

        /// <summary>
        /// Sound Search Pattern
        /// </summary>
        public static byte[] Needle = {
            0xFF, 0xFF, 0xFF, 0xFF, 0xFF, 0xFD,
            0xFD, 0xFD, 0xFE, 0xFF, 0xFF, 0xFF,
            0xFF, 0xFD, 0xFD, 0xFD };

        /// <summary>
        /// Decompresses and Load Sounds from a Ghosts FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            FastFile fastFile = new FastFile();

            reader.Seek(24, SeekOrigin.Begin);

            int numPreBlocks = reader.ReadInt32();

            reader.Seek((24 * numPreBlocks) + 16, SeekOrigin.Current);

            if (isSigned)
                reader = FastFile.Utils.RemoveHashBlocks(reader, 0x4000, 0x200000, 0x2000);

            if (FastFile.Utils.DecompressZlib(reader.BaseStream, outputName, progressCallback))
            {
                if (!progressCallback(100))
                    return null;

                using (BinaryReader decompressedReader = new BinaryReader(new FileStream(outputName, FileMode.Open)))
                    fastFile.Sounds = LoadAudio(decompressedReader);
            }

            return fastFile;
        }

        /// <summary>
        /// Scans for audio from a Ghosts FF
        /// </summary>
        public static List<Sound> LoadAudio(BinaryReader reader)
        {
            long[] offsets = reader.FindBytes(Needle);

            List<Sound> sounds = new List<Sound>();

            foreach (long offset in offsets)
            {
                reader.Seek(offset - 16, SeekOrigin.Begin);

                var sound = reader.ReadStruct<GhostsSound>();

                if (Sound.AcceptedFrameRates.Contains((int)sound.FrameRate))
                {
                    string name = reader.ReadNullTerminatedString();

                    if (name.EndsWith(".flac"))
                        continue;

                    sounds.Add(new Sound()
                    {
                        FilePath  = name,
                        Size      = (int)sound.SoundDataSize,
                        FrameRate = (int)sound.FrameRate,
                        Frames    = (int)sound.FrameCount,
                        Channels  = sound.Channels,
                        Format    = Sound.Formats.PCM,
                        Location  = "FastFile",
                        Position  = reader.BaseStream.Position
                    });
                }
            }

            return sounds;
        }
    }
}
