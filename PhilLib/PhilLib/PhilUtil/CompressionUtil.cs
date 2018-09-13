/*
    Copyright (c) 2018 Philip/Scobalula

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
// using ZLibNet;
using LZ4cc;

namespace PhilUtil
{
    /*
    /// <summary>
    /// Deflate/ZLIB Utilities
    /// </summary>
    public class DeflateUtil
    {
        /// <summary>
        /// Decodes byte array to Memory Stream
        /// </summary>
        /// <param name="data">Byte Array of Deflate Data</param>
        /// <returns>Decoded Memory Stream</returns>
        public static MemoryStream Decode(byte[] data)
        {
            MemoryStream output = new MemoryStream();
            MemoryStream input = new MemoryStream(data);

            using (DeflateStream deflateStream = new DeflateStream(input, CompressionMode.Decompress))
            {
                deflateStream.CopyTo(output);
            }

            output.Flush();
            output.Position = 0;

            return output;
        }

        /// <summary>
        /// Encodes byte array to Memory Stream
        /// </summary>
        /// <param name="data">Byte Array of Data</param>
        /// <returns>Encoded Memory Stream</returns>
        public static MemoryStream Encode(byte[] data)
        {

            MemoryStream output = new MemoryStream();

            using (DeflateStream deflateStream = new DeflateStream(output, CompressionMode.Compress))
            {
                deflateStream.Write(data, 0, data.Length);
            }

            return output;
        }
    }
    */
    /// <summary>
    /// LZ4 Utilities
    /// </summary>
    public class LZ4Util
    {
        /// <summary>
        /// Decodes a byte array of LZ4 Data
        /// </summary>
        /// <param name="data">Encoded Data</param>
        /// <param name="decompressedSize">Decompressed Size</param>
        /// <returns>Decoded data</returns>
        public static byte[] Decode(byte[] data, int decompressedSize)
        {
            return LZ4Codec.Decode64(data, 0, data.Length, decompressedSize);
        }
    }
}
