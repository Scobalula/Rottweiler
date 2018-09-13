/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using PhilUtil;

namespace RottweilerLib
{
    /// <summary>
    /// WaW Logic
    /// </summary>
    class WAW : Game
    {
        /// <summary>
        /// Initializes WaW Support
        /// </summary>
        public WAW()
        {
            Name = "World at War";
            Magic = Magic.InfinityWard;
            Version = 0x183;
            Register();
        }

        /// <summary>
        /// Sound Search Pattern
        /// </summary>
        public static byte[] Needle = {
                0x01, 0x01, 0x00, 0x00, 0xFF, 0xFF,
                0xFF, 0xFF, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
                0x00, 0x00, 0xFF, 0xFF, 0xFF, 0xFF,
                0xFF, 0xFF, 0xFF, 0xFF
            };

        /// <summary>
        /// WaW Sound Header
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        struct WAWSound
        {
            /// <summary>
            /// Sound Name Pointer
            /// </summary>
            public uint NamePointer;

            /// <summary>
            /// Sound Data Pointer
            /// </summary>
            public uint SoundDataPtr;

            /// <summary>
            /// Sound Data Size
            /// </summary>
            public uint SoundDataSize;
        };

        /// <summary>
        /// Decompresses and Load Sounds from a World at War FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            FastFile fastFile = new FastFile();

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
        /// Scans for audio from a World at War FF
        /// </summary>
        public static List<Sound> LoadAudio(BinaryReader reader)
        {
            long[] offsets = reader.FindBytes(Needle);

            List<Sound> sounds = new List<Sound>();

            foreach (long offset in offsets)
            {
                reader.Seek(offset - 8, SeekOrigin.Begin);

                var soundBlock = reader.ReadStruct<WAWSound>();
                string path = reader.ReadNullTerminatedString();
                byte[] waveHeader = reader.ReadBytes(48);

                // Verify FourCC
                if(BitConverter.ToInt32(waveHeader, 0) == 0x46464952)
                {
                    Sound sound = new Sound()
                    {
                        FilePath       = Path.ChangeExtension(path, null),
                        Size       = (int)soundBlock.SoundDataSize,
                        FrameRate = BitConverter.ToInt32(waveHeader, 24),
                        Channels   = BitConverter.ToInt16(waveHeader, 22),
                        Location   = "FastFile",
                        Position   = reader.BaseStream.Position - 48
                    };

                    // Check format from WAV header (WaW supports multiple types)
                    switch (BitConverter.ToInt16(waveHeader, 20))
                    {
                        case 0x1: sound.Format = Sound.Formats.PCM; break;
                        case 0x2: sound.Format = Sound.Formats.ADPCM; break;
                        case 0x161: sound.Format = Sound.Formats.XWMA; break;
                        default: sound.Format = Sound.Formats.UNKNOWN; break;
                    }

                    sounds.Add(sound);
                }
            }

            return sounds;
        }
    }
}
