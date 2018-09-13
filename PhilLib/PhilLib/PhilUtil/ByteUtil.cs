using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhilUtil
{
    /// <summary>
    /// Byte Handling Utilies
    /// </summary>
    public class ByteUtil
    {
        /// <summary>
        /// Reads a null terminated string from a byte array
        /// </summary>
        /// <param name="input">Byte Array input</param>
        /// <param name="startIndex">Start Index</param>
        /// <returns>Resulting string</returns>
        public static string ReadNullTerminatedString(byte[] input, int startIndex)
        {
            List<byte> result = new List<byte>();

            for(int i = startIndex; i < input.Length && input[i] != 0; i++)
            {
                result.Add(input[i]);
            }

            return Encoding.ASCII.GetString(result.ToArray());
        }

        /// <summary>
        /// Gets bit from an integer as bool
        /// </summary>
        /// <param name="input"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static bool GetBit(int input, int bit)
        {
            return ((input >> bit) & 1) == 1;
        }

        /// <summary>
        /// Gets bit from an integer as an int
        /// </summary>
        /// <param name="input"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public static int GetBitAsInt(int input, int bit)
        {
            return ((input >> bit) & 1);
        }
    }
}
