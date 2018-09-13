/*
    Copyright (c) 2018 Philip/Scobalula - Utility Lib

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using System.Runtime.InteropServices;

namespace PhilUtil
{
    /// <summary>
    /// File Handling Utilies
    /// </summary>
    public class FileUtil
    {
        /// <summary>
        /// Checks if a file can be accessed.
        /// </summary>
        /// <param name="file">File Path</param>
        /// <returns>True if file can be accessed, false if we failed to access the file.</returns>
        public static bool CanAccessFile(string file)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.None);
            }
            catch
            {
                return false;
            }
            finally
            {
                stream?.Close();
            }

            return true;
        }
    }

    /// <summary>
    /// Class containing extensions to BinaryReader/Writer
    /// </summary>
    public static class BinaryIOExtensions
    {
        /// <summary>
        /// Reads C/C++ Style String terminated by a null byte. 
        /// Advances the current position of the stream by the length of the string + 1.
        /// </summary>
        /// <returns>Read String</returns>
        public static string ReadNullTerminatedString(this BinaryReader br, int maxSize = -1)
        {
            StringBuilder str = new StringBuilder();

            int byteRead;

            int size = 0;

            while ((byteRead = br.BaseStream.ReadByte()) != 0x0 && size++ != maxSize)
                str.Append(Convert.ToChar(byteRead));

            return str.ToString();
        }

        /// <summary>
        /// Reads a string of fixed size
        /// </summary>
        /// <param name="numBytes">Size of string in bytes</param>
        /// <returns>Read String</returns>
        public static string ReadFixedString(this BinaryReader br, int numBytes)
        {
            return Encoding.ASCII.GetString(br.ReadBytes(numBytes)).TrimEnd('\0');
        }

        /// <summary>
        /// Reads a string of fixed size
        /// </summary>
        /// <param name="numBytes">Size of string in bytes</param>
        /// <returns>Read String</returns>
        public static T ReadStruct<T>(this BinaryReader br)
        {
            // Cast it
            GCHandle handle = GCHandle.Alloc(br.ReadBytes(Marshal.SizeOf(typeof(T))), GCHandleType.Pinned);
            // Make it
            T theStructure = (T)Marshal.PtrToStructure(handle.AddrOfPinnedObject(), typeof(T));
            // Free handle
            handle.Free();
            // Return it
            return theStructure;
        }

        /// <summary>
        /// Sets the position of the Base Stream
        /// </summary>
        /// <param name="br"></param>
        /// <param name="offset">Offset to seek to.</param>
        /// <param name="seekOrigin">Seek Origin</param>
        public static void Seek(this BinaryReader br, long offset, SeekOrigin seekOrigin)
        {
            br.BaseStream.Seek(offset, seekOrigin);
        }

        /// <summary>
        /// Searches for bytes in file and returns offsets.
        /// </summary>
        /// <param name="br"></param>
        /// <param name="needle">Bytes to search for.</param>
        /// <returns></returns>
        public static long[] FindBytes(this BinaryReader br, byte[] needle, bool firstOccurence = false, long? from = null, bool byteStart = false)
        {
            /*
               TODO: Needs heavy improvement.

                Switched to buffer than byte by byte,
                MUCH faster.
            */
            if(from != null)
                br.Seek((long)from, SeekOrigin.Begin);
            // List of offsets in file.
            List<long> offsets = new List<long>();
            // Buffer
            byte[] buffer = new byte[1048576];
            // Bytes Read
            int bytesRead = 0;
            // Starting Offset
            long readBegin = br.BaseStream.Position;
            // Needle Index
            int needleIndex = 0;
            // Byte Array Index
            int bufferIndex = 0;
            // Read chunk of file
            while ((bytesRead = br.BaseStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                // Loop through byte array
                for (bufferIndex = 0; bufferIndex < bytesRead; bufferIndex++)
                {
                    // Check if current bytes match
                    if (needle[needleIndex] == buffer[bufferIndex])
                    {
                        // Indc
                        needleIndex++;
                        // Check if we have a match
                        if (needleIndex == needle.Length)
                        {
                            // Add Offset
                            offsets.Add(readBegin + bufferIndex + 1 - (byteStart ? needle.Length : 0));
                            // Reset Index
                            needleIndex = 0;
                            // Check before continuing
                            if (needle[needleIndex] == buffer[bufferIndex])
                                needleIndex++;
                            // If only first occurence, end search
                            if(firstOccurence)
                                goto complete;
                        }
                    }
                    else
                    {
                        // Reset Index
                        needleIndex = 0;
                        // TODO: Better way of checking if was match then 
                        // then didn't match, for now this
                        if (needle[needleIndex] == buffer[bufferIndex])
                            needleIndex++;
                    }
                }
                // Set next offset
                readBegin += bytesRead;
            } complete:;
            // Return offsets as an array
            return offsets.ToArray();
        }
    }
}
