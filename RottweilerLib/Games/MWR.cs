/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace RottweilerLib.Games
{
    /// <summary>
    /// MWR Logic
    /// </summary>
    class MWR : Game
    {
        /// <summary>
        /// Initializes MWR Support
        /// </summary>
        public MWR()
        {
            Name = "Modern Warfare Remastered";
            Magic = Magic.Sledgehammer;
            Version = 0x42;
            Register();
        }

        /// <summary>
        /// Decompresses and Load Sounds from a Modern Warfare Remastered FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            // Use AW's Load Method
            return GetGame(Magic.Sledgehammer, 0x72E).Load(reader, outputName, isSigned, progressCallback);
        }
    }
}
