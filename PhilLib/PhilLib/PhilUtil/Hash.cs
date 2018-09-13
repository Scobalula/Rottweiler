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

namespace PhilUtil
{
    /// <summary>
    /// Common Hash Functions
    /// </summary>
    public class Hash
    {
        /// <summary>
        /// Calculates DJB Hash for a sequence of bytes.
        /// </summary>
        public static uint DJB(byte[] bytes)
        {
            uint hash = 0x1505;

            for (int i = 0; i < bytes.Length; i++)
                hash = ((hash << 5) + hash) + bytes[i];

            return hash;
        }

        /// <summary>
        /// Calculates 64bit FNV Hash for a given string
        /// </summary>
        public static ulong FNV1a(string input)
        {
            ulong result = 0xCBF29CE484222325;

            for (int i = 0; i < input.Length; i++)
            {
                result ^= input[i];
                result *= 0x100000001B3;
            }

            return result;
        }

        /// <summary>
        /// Calculates SDBM Hash for a given string
        /// </summary>
        public static uint SDBM(string name)
        {
            uint hash = 5381;

            for (int i = 0; i < name.Length; i++)
                hash = name[i] + (hash << 6) + (hash << 16) - hash;

            return hash;
        }
    }
}
