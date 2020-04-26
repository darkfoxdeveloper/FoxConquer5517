// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Packet Structure.cs
// Last Edit: 2016/11/23 07:59
// Created: 2016/11/23 07:52

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace ServerCore.Networking.Packets
{
    /// <summary>
    /// This class encapsulates a packet structure in the form of a byte array. By inheriting this class, you can make
    /// any class a packet that contains methods for reading and writing data values to the packet. After inheriting
    /// this class, you may send it to any method that accepts a byte array (this class will automatically convert it).
    /// When constructing a new packet, it will write the header for you.
    /// </summary>
    public unsafe abstract class PacketStructure
    {
        // Local-Scope Variable Declarations:
        private byte[] _array;  // The packet byte array. The base of the packet.

        // Local-Scope Native Function Calls:
        #region Native Function Calls
        public const string MSVCRT = "msvcrt.dll";

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void* memcpy(void* dst, string src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void* memcpy(void* dst, byte[] src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        private static extern void* memcpy(byte[] dst, void* src, int length);

        /// <summary> Replaces the value of a location in memory with an initialization value.</summary>
        /// <param name="dst">The destination of the data being replaced.</param>
        /// <param name="fill">The initialization value replacing the data.</param>
        /// <param name="length">The length to replace.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memset(void* dst, byte fill, int length);
        #endregion

        /// <summary>
        /// This class encapsulates a packet structure in the form of a byte array. By inheriting this class, you can make
        /// any class a packet that contains methods for reading and writing data values to the packet. After inheriting
        /// this class, you may send it to any method that accepts a byte array (this class will automatically convert it).
        /// When constructing a new packet, it will write the header for you.
        /// </summary>
        /// <param name="receivedPacket">The packet received from the client..</param>
        public PacketStructure(byte[] receivedPacket)
        {
            _array = receivedPacket;
        }

        /// <summary>
        /// This class encapsulates a packet structure in the form of a byte array. By inheriting this class, you can make
        /// any class a packet that contains methods for reading and writing data values to the packet. After inheriting
        /// this class, you may send it to any method that accepts a byte array (this class will automatically convert it).
        /// When constructing a new packet, it will not write the header for you.
        /// </summary>
        /// <param name="length">The size of the new packet to be sent to the client.</param>
        public PacketStructure(int length)
        {
            // Create the packet:
            _array = new byte[length];
        }

        /// <summary>
        /// This class encapsulates a packet structure in the form of a byte array. By inheriting this class, you can make
        /// any class a packet that contains methods for reading and writing data values to the packet. After inheriting
        /// this class, you may send it to any method that accepts a byte array (this class will automatically convert it).
        /// When constructing a new packet, it will not write the header for you.
        /// </summary>
        /// <param name="type">The type of packet being created.</param>
        /// <param name="length">The size of the new packet to be sent to the client.</param>
        public PacketStructure(PacketType type, int length)
        {
            // Create the packet:
            _array = new byte[length];
            WriteHeader(length, type);
        }

        /// <summary>
        /// This class encapsulates a packet structure in the form of a byte array. By inheriting this class, you can make
        /// any class a packet that contains methods for reading and writing data values to the packet. After inheriting
        /// this class, you may send it to any method that accepts a byte array (this class will automatically convert it).
        /// When constructing a new packet, it will not write the header for you.
        /// </summary>
        /// <param name="type">The type of packet being created.</param>
        /// <param name="arrayLength">The size of the new packet to be sent to the client.</param>
        /// <param name="writtenLenth">The written length for the packet (not including the footer).</param>
        public PacketStructure(PacketType type, int arrayLength, int writtenLenth)
        {
            // Create the packet:
            _array = new byte[arrayLength];
            WriteHeader(writtenLenth, type);
        }

        #region Read Methods
        /// <summary> This method returns a boolean from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The boolean read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool ReadBoolean(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return ptr[offset] == 1;
        }

        /// <summary> This method returns an unsigned byte from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The unsigned byte read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte ReadByte(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return ptr[offset];
        }

        /// <summary> This method returns a signed byte from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The signed byte read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public sbyte ReadSByte(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(sbyte*)(ptr + offset);
        }

        /// <summary> This method returns an unsigned short from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The unsigned short read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadUShort(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(ushort*)(ptr + offset);
        }

        /// <summary> This method returns a signed short from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The signed short read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public short ReadShort(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(short*)(ptr + offset);
        }

        /// <summary> This method returns an unsigned integer from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The unsigned integer read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint ReadUInt(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(uint*)(ptr + offset);
        }

        /// <summary> This method returns a signed integer from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The signed integer read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public int ReadInt(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(int*)(ptr + offset);
        }

        /// <summary> This method returns an unsigned long from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The unsigned long read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ulong ReadULong(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(ulong*)(ptr + offset);
        }

        /// <summary> This method returns a signed long from the packet at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The signed long read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public long ReadLong(int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return *(long*)(ptr + offset);
        }

        /// <summary> This method returns a string from the packet at the specified offset. </summary>
        /// <param name="length">The length to read for.</param>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The string read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(int length, int offset)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return new string((sbyte*)ptr, offset, length, Encoding.GetEncoding(1252)).TrimEnd('\0');
        }

        /// <summary> This method returns a string from the packet at the specified offset. </summary>
        /// <param name="length">The length to read for.</param>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The string read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ReadString(int length, int offset, Encoding encoding)
        {
            // Read the value:
            fixed (byte* ptr = _array)
                return new string((sbyte*)ptr, offset, length, encoding).TrimEnd('\0');
        }

        /// <summary> This method returns an array from the packet at the specified offset. </summary>
        /// <param name="length">The length to read for.</param>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The array read from the packet at the specified offset.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte[] ReadArray(int length, int offset)
        {
            // Read the value:
            byte[] array = new byte[length];
            Array.Copy(_array, offset, array, 0, length);
            return array;
        }

        /// <summary> This indexer returns and writes the unsigned byte at the specified offset. </summary>
        /// <param name="offset">The position at which the reader will read from.</param>
        /// <returns>The unsigned byte read from the packet at the specified offset.</returns>
        public byte this[int offset]
        {
            get { fixed (byte* ptr = _array) return ptr[offset]; }
            set { fixed (byte* ptr = _array) ptr[offset] = value; }
        }
        #endregion

        #region Reconstruction Methods
        /// <summary>
        /// This method writes the packet header to the packet structure. It does not change the length of the
        /// array. To change the array length, you must use the resize method.
        /// </summary>
        /// <param name="length">The size of the new packet to be sent to the client.</param>
        /// <param name="type">The type of packet being sent.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteHeader(int length, PacketType type)
        {
            // Write the value:
            fixed (byte* ptr = _array)
            {
                *(ushort*)(ptr) = (ushort)length;
                *(PacketType*)(ptr + 2) = type;
            }
        }

        /// <summary> The length of the packet's array. </summary>
        public int Length
        {
            get { return _array.Length; }
        }

        /// <summary>
        /// This method resizes the packet. Warning, this may result in data loss if the new size is smaller than
        /// the original size of the packet. Use with caution.
        /// </summary>
        /// <param name="length">The new length of the packet.</param>
        public void Resize(int length)
        {
            // Error check the length:
            if (length < 4)
                throw new IndexOutOfRangeException("Packet cannot be resized. Length must be 4 or greater.");

            // Resize the packet:
            if (_array == null || _array.Length == 0) _array = new byte[length];
            else Array.Resize<byte>(ref _array, length);
        }

        /// <summary>
        /// Rebuilds the packet from scratch.
        /// </summary>
        /// <param name="size">The new size of the packet.</param>
        /// <param name="type">The type of the packet.</param>
        public void ClearPacket(int size, PacketType type)
        {
            _array = new byte[size];
            WriteHeader(size, type);
        }
        #endregion

        #region Write Methods
        /// <summary>
        /// This method fills a length of bytes from the position specified with the value specified in the parameters
        /// of the method. This method is used for clearing bytes for rewriting strings.
        /// </summary>
        /// <param name="value">The value the length of bytes will be filled with.</param>
        /// <param name="offset">The offset that the filling will start at.</param>
        /// <param name="length">The amount of bytes to fill for.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Fill(byte value, int offset, int length)
        {
            // Fill the array with the value:
            fixed (byte* ptr = _array)
                memset(ptr + offset, value, length);
        }

        /// <summary> This method writes a boolean to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteBoolean(bool value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                ptr[offset] = (byte)(value ? 1 : 0);
        }

        /// <summary> This method writes an unsigned byte to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteByte(byte value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                ptr[offset] = value;
        }

        /// <summary> This method writes a signed byte to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteSByte(sbyte value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(sbyte*)(ptr + offset) = value;
        }

        /// <summary> This method writes an unsigned short to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUShort(ushort value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(ushort*)(ptr + offset) = value;
        }

        /// <summary> This method writes a signed short to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteShort(short value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(short*)(ptr + offset) = value;
        }

        /// <summary> This method writes an unsigned integer to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteUInt(uint value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(uint*)(ptr + offset) = value;
        }

        /// <summary> This method writes a signed integer to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteInt(int value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(int*)(ptr + offset) = value;
        }

        /// <summary> This method writes an unsigned long to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteULong(ulong value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(ulong*)(ptr + offset) = value;
        }

        /// <summary> This method writes a signed long to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteLong(long value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(long*)(ptr + offset) = value;
        }

        /// <summary> This method writes a string to the packet structure (without writing the length). </summary>
        /// <param name="value">The string being written to the packet.</param>
        /// <param name="length">The maximum length of the string.</param>
        /// <param name="offset">The position where the string will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, int length, int offset)
        {
            // Write the value:
            Fill(0, offset, length);
            fixed (byte* ptr = _array)
                memcpy(ptr + offset, value, value.Length);
        }

        /// <summary> This method writes a string to the packet structure (without writing the length). </summary>
        /// <param name="value">The string being written to the packet.</param>
        /// <param name="length">The maximum length of the string.</param>
        /// <param name="offset">The position where the string will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteString(string value, int length, int offset, Encoding encoding)
        {
            // Write the value:
            byte[] bytes = encoding.GetBytes(value);
            Fill(0, offset, length);
            fixed (byte* ptr = _array)
                memcpy(ptr + offset, bytes, bytes.Length);
        }

        /// <summary> This method writes a byte length and string to the packet structure. </summary>
        /// <param name="value">The string being written to the packet.</param>
        /// <param name="offset">The position where the string will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteStringWithLength(string value, int offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
            {
                ptr[offset] = (byte)value.Length;
                Fill(0, offset + 1, value.Length);
                memcpy(ptr + offset + 1, value, value.Length);
            }
        }

        /// <summary> This method writes an unsigned byte array to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray(byte[] value, int offset)
        {
            // Write the value:
            Fill(0, offset, value.Length);
            fixed (byte* ptr = _array)
                memcpy(ptr + offset, value, value.Length);
        }

        /// <summary> This method writes an unsigned byte array to the packet structure. </summary>
        /// <param name="value">The value being written to the packet.</param>
        /// <param name="length">The length of the array to copy.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteArray(byte[] value, int length, int offset)
        {
            // Write the value:
            Fill(0, offset, length);
            fixed (byte* ptr = _array)
                memcpy(ptr + offset, value, length);
        }

        /// <summary> This method writes a signed random integer to the packet structure. </summary>
        /// <param name="minValue">The minimum value that can be generated by the random number generator.</param>
        /// <param name="maxValue">The maximum value that can be generated by the random number generator.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRandomInt(int minValue, int maxValue, byte offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(int*)(ptr + offset) = new Random().Next(minValue, maxValue);
        }

        /// <summary> This method writes a signed random integer to the packet structure. </summary>
        /// <param name="seed">The seed being used to generate the numbers</param>
        /// <param name="minValue">The minimum value that can be generated by the random number generator.</param>
        /// <param name="maxValue">The maximum value that can be generated by the random number generator.</param>
        /// <param name="offset">The position where the value will be written to.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteRandomInt(int seed, int minValue, int maxValue, byte offset)
        {
            // Write the value:
            fixed (byte* ptr = _array)
                *(int*)(ptr + offset) = new Random(seed).Next(minValue, maxValue);
        }
        #endregion

        #region Build Methods
        /// <summary>
        /// This method should not be called in a method outside the packet structure class. This method is called
        /// by the packet structure class during packet construction (when passing it as a byte array). For advanced
        /// packet construction, you may override this method and define how the array is constructed.
        /// </summary>
        protected virtual byte[] Build()
        {
            return _array;
        }

        /// <summary> This method converts the packet class inheriting this into an unsigned byte array. </summary>
        /// <param name="structure">The structure being converted into an unsigned byte array.</param>
        /// <returns>This method returns the array built by the packet structure class.</returns>
        public static implicit operator byte[](PacketStructure structure)
        {
            return structure.Build();
        }

        /// <summary> This method creates a new string that represents the packet structure. </summary>
        /// <returns>This method returns the string representation of the array.</returns>
        public override string ToString()
        {
            return BitConverter.ToString(_array).Replace("-", " ");
        }

        /// <summary> Returns the hash code of the packet array. </summary>
        public override int GetHashCode()
        {
            return _array.GetHashCode();
        }

        /// <summary> Returns true if the byte array is equal to this packet. </summary>
        /// <param name="obj">The byte array being compared with this packet.</param>
        public override bool Equals(object obj)
        {
            return _array.Equals(obj);
        }
        #endregion
    }
}
