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
using System.IO;

namespace PhilUtil
{
    /// <summary>
    /// Message Types for logging
    /// </summary>
    public enum MessageType
    {
        INFO,
        WARNING,
        ERROR,
    }

    /// <summary>
    /// Class handling logging.
    /// </summary>
    public class Logger
    {
        public static Logger ActiveLogger { get; set; }

        /// <summary>
        /// Log File Name
        /// </summary>
        public string LogFile { get; set; }

        /// <summary>
        /// Current Log Name
        /// </summary>
        private string LogName { get; set; }

        /// <summary>
        /// Active Stream
        /// </summary>
        private StreamWriter ActiveStream { get; set; }

        /// <summary>
        /// Initiate Logger
        /// </summary>
        /// <param name="logName">Log Name</param>
        /// <param name="fileName">Log File Name</param>
        public Logger(string logName, string fileName)
        {
            LogFile = fileName;
            LogName = logName;

            Write(LogName, MessageType.INFO);
            CloseStream();
        }

        public void Write(string message, MessageType messageType)
        {
            // Re-open stream if closed/null
            if ((ActiveStream == null) || (ActiveStream.BaseStream == null))
                ActiveStream = new StreamWriter(LogFile, true);

            // Write to file
            ActiveStream.WriteLine("{0} [ {1} ] {2}", DateTime.Now.ToString("dd-MM-yyyy - HH:mm:ss"), messageType, message);
        }

        /// <summary>
        /// Closes Active Streamwriter
        /// </summary>
        public void CloseStream()
        {
            if (!(ActiveStream == null) && !(ActiveStream.BaseStream == null))
            {
                ActiveStream.Close();
                ActiveStream = null;
            }
            else
            {
                ActiveStream = null;
            }
        }

        /// <summary>
        /// Disposes Stream
        /// </summary>
        public void Dispose()
        {
            if (ActiveStream != null)
                ActiveStream.Close();

            GC.SuppressFinalize(this);
        }

    }
}
