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
    /// MW2 Logic
    /// </summary>
    class MW2 : Game
    {
        /// <summary>
        /// Initializes MW2 Support
        /// </summary>
        public MW2()
        {
            Name = "Modern Warfare 2";
            Magic = Magic.InfinityWard;
            Version = 0x114;
            Register();
        }

        /// <summary>
        /// Decompresses and Load Sounds from a Modern Warfare 2 FF
        /// </summary>
        public override FastFile Load(BinaryReader reader, string outputName, bool isSigned, Func<float, bool> progressCallback)
        {
            // Use MW's Load Method
            return GetGame(Magic.InfinityWard, 0x5).Load(reader, outputName, isSigned, progressCallback);
        }
    }
}
