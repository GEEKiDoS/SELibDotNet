using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

///
///   ExtendedBinaryReader.cs
///   Author: DTZxPorter
///   Written for the SE Format Project
///

namespace SELib.Utilities
{
    /// <summary>
    /// Supports custom methods for manipulating data between c++ streams and .net ones
    /// </summary>
    public class ExtendedBinaryReader : BinaryReader
    {
        public ExtendedBinaryReader(Stream stream)
            : base(stream)
        {
        }

        /// <summary>
        /// Reads a null-terminated string from the stream
        /// </summary>
        /// <returns>The resulting string</returns>
        public string ReadNullTermString()
        {
            // Buffer
            string StringBuffer = "";
            // Char buffer
            char CharBuffer;
            // Read until null-term
            while ((int)(CharBuffer = this.ReadChar()) != 0)
            {
                StringBuffer = StringBuffer + CharBuffer;
            }
            // Result
            return StringBuffer;
        }

        /// <summary>
        /// Reads a string started with a length stored in ushort
        /// </summary>
        /// <returns></returns>
        public string ReadStringWithUInt16Length()
        {
            ushort length = ReadUInt16();
            return new string(ReadChars(length));
        }

        /// <summary>
        /// Skip few bytes
        /// </summary>
        /// <param name="len">Length of bytes to skip</param>
        public void Skip(int len, bool start = false)
        {
            BaseStream.Seek(len, start ? SeekOrigin.Begin : SeekOrigin.Current);
        }
    }
}
