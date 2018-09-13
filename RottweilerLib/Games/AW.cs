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
    /// Initializes AW Support
    /// </summary>
    class AW : Game
    {
        /// <summary>
        /// Initializes AW Support
        /// </summary>
        public AW()
        {
            Name = "Advanced Warfare";
            Magic = Magic.Sledgehammer;
            Version = 0x72E;
            Register();
        }

        /// <summary>
        /// AW Sound Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct AWSound
        {
            /// <summary>
            /// Sound Name Pointer
            /// </summary>
            public ulong NamePointer;

            /// <summary>
            /// ???
            /// </summary>
            public ushort UnknownShort;

            /// <summary>
            /// Pack File Index
            /// </summary>
            public ushort PackFileIndex;

            /// <summary>
            /// ???
            /// </summary>
            public uint Padding;

            /// <summary>
            /// Pack Offset
            /// </summary>
            public ulong PackFileOffset;

            /// <summary>
            /// Data Size in Pak File
            /// </summary>
            public ulong PackFileSize;

            /// <summary>
            /// Sound Data Pointer
            /// </summary>
            public ulong SoundDataPtr;

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
            /// Channel Count
            /// </summary>
            public byte Channels;

            /// <summary>
            /// ???
            /// </summary>
            public ushort UnknownShort1;

            /// <summary>
            /// ???
            /// </summary>
            public byte UnknownByte;

            /// <summary>
            /// Sound Format (1 = PCM WAV, 2 = FLAC)
            /// </summary>
            public uint Format;

            /// <summary>
            /// Sound Buffer Size
            /// </summary>
            public uint SoundBufferSize;
        };

        /// <summary>
        /// Sound Search Pattern
        /// </summary>
        public static byte[] Needle = {
                0x00, 0xFF, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFD, 0xFD, 0xFD };


        /// <summary>
        /// Loads an Advanced Warfare Fast File
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="outputName"></param>
        /// <param name="isSigned"></param>
        /// <param name="progressCallback"></param>
        /// <returns></returns>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            FastFile fastFile = new FastFile();


            reader.Seek(24, SeekOrigin.Begin);

            int numPreBlocks = reader.ReadInt32();
            
            reader.Seek((24 * numPreBlocks) + 16, SeekOrigin.Current);

            if (isSigned)
                reader = FastFile.Utils.RemoveHashBlocks(reader, 0x8000, 0x800000, 0x4000);

            reader.Seek(2, SeekOrigin.Current);

            int numLZ4Blocks = reader.ReadInt32();

            reader.Seek(6, SeekOrigin.Current);

            if (FastFile.Utils.DecompressLZ4(reader.BaseStream, outputName, numLZ4Blocks, progressCallback))
            {
                if (progressCallback(100))
                {
                    using (BinaryReader decompressedReader = new BinaryReader(new FileStream(outputName, FileMode.Open)))
                    {
                        fastFile.Sounds = LoadAudio(decompressedReader);
                    }
                }
            }


            return fastFile;
        }

        /// <summary>
        /// Scans for and Loads Audio Metadata from an Advanced Warfare Fast File
        /// </summary>
        public static List<Sound> LoadAudio(BinaryReader reader)
        {
            long[] offsets = reader.FindBytes(Needle);

            List<Sound> sounds = new List<Sound>();

            foreach (long offset in offsets)
            {
                reader.Seek(offset - 40, SeekOrigin.Begin);

                var sound = reader.ReadStruct<AWSound>();

                if (Sound.AcceptedFrameRates.Contains((int)sound.FrameRate))
                {
                    sounds.Add(new Sound()
                    {
                        FilePath      = reader.ReadNullTerminatedString(),
                        Size          = (int)sound.SoundDataSize,
                        FrameRate     = (int)sound.FrameRate,
                        Frames        = (int)sound.FrameCount,
                        Channels      = sound.Channels,
                        Format        = sound.Format == 6 || sound.Format == 7 ? Sound.Formats.FLAC : Sound.Formats.PCM,
                        Location      = sound.PackFileIndex > 0 ? String.Format("Pak {0}", sound.PackFileIndex) : "FastFile",
                        Position      = sound.PackFileIndex > 0 ? (long)sound.PackFileOffset : reader.BaseStream.Position
                    });
                }
            }

            return sounds;
        }
    }
}
