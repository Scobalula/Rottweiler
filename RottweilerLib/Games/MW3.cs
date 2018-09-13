/*
 *  Rottweiler - Call of Duty Sound Exporter - Copyright 2018 Philip/Scobalula
 *  
 *  This file is subject to the license terms set out in the
 *  "LICENSE.txt" file. 
 * 
 */
using System;
using System.IO;

namespace RottweilerLib.Games
{
    /// <summary>
    /// MW3 Logic
    /// </summary>
    class MW3 : Game
    {
        /// <summary>
        /// Initializes MW3 Support
        /// </summary>
        public MW3()
        {
            Name = "Modern Warfare 3";
            Magic = Magic.InfinityWard;
            Version = 0x1;
            Register();
        }

        /// <summary>
        /// Decompresses and Load Sounds from a Modern Warfare 3 FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            // Use MW's Load Method
            return GetGame(Magic.InfinityWard, 0x5).Load(reader, outputName, isSigned, progressCallback);
        }
    }
}
