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
using System.Text;

namespace PhilUtil
{
    /// <summary>
    /// Primitive WAV Class
    /// </summary>
    public class WAVUtil
    {
        /// <summary>
        /// Writes a WAV file
        /// </summary>
        /// <param name="fileName">File Path</param>
        /// <param name="sampleRate">Sample Rate in Hertz</param>
        /// <param name="channels">Number of Channels</param>
        /// <param name="audioData">Audio Data</param>
        public static void WriteWavFile(string fileName, int sampleRate, int channels, byte[] audioData)
        {
            using (BinaryWriter bw = new BinaryWriter(new FileStream(fileName, FileMode.Create)))
            {
                // Write RIFF ChunkID
                bw.Write(Encoding.ASCII.GetBytes("RIFF"));
                // Write Chunk Size
                bw.Write(36 + audioData.Length);
                // Write Format / Chunk 1 ID
                bw.Write(Encoding.ASCII.GetBytes("WAVEfmt "));
                // Write Chunk 1 Size
                bw.Write(16);
                // Write Audio Format
                bw.Write((ushort)1);
                // Write Number of Channels
                bw.Write((ushort)channels);
                // Write Sample Rate
                bw.Write(sampleRate);
                // Write Byte Rate (SampleRate * NumChannels * BitsPerSample/8)
                bw.Write(sampleRate * channels * 2);
                // Write Block Align (NumChannels * BitsPerSample/8)
                bw.Write((ushort)(channels * 2));
                // Write Bits Per Sample
                bw.Write((ushort)16);
                // Write data ChunkID
                bw.Write(Encoding.ASCII.GetBytes("data"));
                // Write Size of Audio
                bw.Write(audioData.Length);
                // Write Audio
                bw.Write(audioData);
            }
        }
    }
}
