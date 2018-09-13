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
    /// MW Logic
    /// </summary>
    class MW : Game
    {
        /// <summary>
        /// Initializes MW Support
        /// </summary>
        public MW()
        {
            Name = "Modern Warfare";
            Magic = Magic.InfinityWard;
            Version = 0x5;
            Register();
        }


        /// <summary>
        /// MW Sound Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct MWSound
        {
            /// <summary>
            /// Sound Name Pointer
            /// </summary>
            public uint NamePointer;

            /// <summary>
            /// ???
            /// </summary>
            public uint UnknownInt;

            /// <summary>
            /// Sound Data Pointer
            /// </summary>
            public uint SoundDataPtr;

            /// <summary>
            /// Sound Data Size
            /// </summary>
            public uint SoundDataSize;

            /// <summary>
            /// Sound Frame Rate
            /// </summary>
            public uint FrameRate;

            /// <summary>
            /// Number of Bits Per Each Sample
            /// </summary>
            public uint BitsPerSample;

            /// <summary>
            /// Channel Count
            /// </summary>
            public uint Channels;

            /// <summary>
            /// Sound Frame Count
            /// </summary>
            public uint FrameCount;

            /// <summary>
            /// ??? (Always 2?)
            /// </summary>
            public uint UnknownInt1;

            /// <summary>
            /// ??? (Equal to DataPtr in Memory?)
            /// </summary>
            public uint UnknownInt2;

            /// <summary>
            /// ??? (Equal to DataPtr in Memory?)
            /// </summary>
            public uint UnknownInt3;
        };

        /// <summary>
        /// Sound Search Pattern
        /// </summary>
        public static byte[] Needle = {
                0x01, 0x01, 0x00, 0x00, 0xFF,
                0xFF, 0xFF, 0xFF, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0xFF, 0xFF,
                0xFF, 0x01, 0x00, 0x00, 0x00 };

        /// <summary>
        /// Decompresses and Load Sounds from a Modern Warfare FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            FastFile fastFile = new FastFile();

            // If we're loading MW2 or 3, we need to skip further
            if (RottweilerUtil.ActiveGame.Name != "Modern Warfare")
                reader.Seek(21, SeekOrigin.Begin);

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
        /// Scans for audio from a Modern Warfare FF
        /// </summary>
        public static List<Sound> LoadAudio(BinaryReader reader)
        {
            long[] offsets = reader.FindBytes(Needle);

            List<Sound> sounds = new List<Sound>();

            foreach (long offset in offsets)
            {
                reader.Seek(offset - 8, SeekOrigin.Begin);

                var sound = reader.ReadStruct<MWSound>();

                if (Sound.AcceptedFrameRates.Contains((int)sound.FrameRate))
                {
                    sounds.Add(new Sound()
                    {
                        FilePath  = reader.ReadNullTerminatedString(),
                        Size      = (int)sound.SoundDataSize,
                        FrameRate = (int)sound.FrameRate,
                        Frames    = (int)sound.FrameCount,
                        Channels  = (int)sound.Channels,
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
