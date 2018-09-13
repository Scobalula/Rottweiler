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
using System;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace PhilUtil
{
    public class Print
    {
        /// <summary>
        /// Print prefixed line to the console
        /// </summary>
        public static void Line(object value = null, object prefix = null, int padding = 12, ConsoleColor consoleColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" {0}", (prefix?.ToString() ?? "").PadRight(padding));
            Console.BackgroundColor = consoleColor;
            Console.Write("│");
            Console.WriteLine(" {0}", value);
            Console.ResetColor();
        }

        /// <summary>
        /// Print prefixed text to the console
        /// </summary>
        public static void Text(object value = null, object prefix = null, int padding = 12, ConsoleColor consoleColor = ConsoleColor.Black)
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.Write(" {0}", (prefix?.ToString() ?? "").PadRight(padding));
            Console.BackgroundColor = consoleColor;
            Console.Write("│");
            Console.Write(" {0}", value);
            Console.ResetColor();
        }
    }
}
