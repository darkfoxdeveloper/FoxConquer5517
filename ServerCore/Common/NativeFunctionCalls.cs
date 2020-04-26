// World Conquer Online Project 2.5517 - Phoenix Project Based
// This project has been created by Felipe Vieira Vendramini and updated by Cristian Ocaña Soler
// Source Infrastructure based on Phoenix Source, written by Gareth Jensen
// This source is targeted to Conquer Online, client version 5517
// 

// File Created by:  Felipe Vieira Vendramini 
// zfserver v2.5517 - ServerCore - Native Function Calls.cs
// Last Edit: 2016/11/23 07:57
// Created: 2016/11/23 07:50

using System.Runtime.InteropServices;

namespace ServerCore.Common
{
    /// <summary>
    /// This container provides the servers with platform invocation services (PInvokes) that enables managed code in C# to
    /// call C-style functions during runtime. The results of these functions should be monitored by the programmer. They can
    /// cause memory leaks.
    /// </summary>
    public unsafe static class NativeFunctionCalls
    {
        // Local-Scope Variable Declaractions:
        public const string MSVCRT = "msvcrt.dll";
        public const string KERNEL32 = "kernel32.dll";

        /// <summary> Allocates a location in memory. </summary>
        /// <param name="size">The length of data to be allocated.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* malloc(int size);

        /// <summary> 
        /// Allocates a block of memory for an array of num elements, each of them size bytes long, and 
        /// initializes all its bits to zero. 
        /// </summary>
        /// <param name="size">The length of data to be allocated.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* calloc(int size);

        /// <summary> Deallocates a location in memory. </summary>
        /// <param name="memblock">The address location to deallocate.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void free(void* memblock);

        /// <summary> Replaces the value of a location in memory with an initialization value.</summary>
        /// <param name="dst">The destination of the data being replaced.</param>
        /// <param name="fill">The initialization value replacing the data.</param>
        /// <param name="length">The length to replace.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memset(void* dst, byte fill, int length);

        /// <summary> Compares the values of two locations in memory. </summary>
        /// <param name="buf1">The first value to be compared.</param>
        /// <param name="buf2">The second value to be compared.</param>
        /// <param name="count">The length of the values being compared.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern int memcmp(void* buf1, void* buf2, int count);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(void* dst, void* src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(void* dst, string src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(byte[] dst, void* src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(void* dst, byte[] src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(byte[] dst, byte[] src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(uint[] dst, uint[] src, int length);

        /// <summary> Copies the value of one memory location into another. </summary>
        /// <param name="dst">The destination of the value being copied.</param>
        /// <param name="src">The source of the copy.</param>
        /// <param name="length">The length of data to be copied.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void* memcpy(byte[] dst, string src, int length);

        /// <summary> Reallocates a location in memory, expanding or reducing the amount of memory available in the block. </summary>
        /// <param name="ptr">Pointer to a memory block previously allocated with malloc, calloc or realloc to be reallocated.</param>
        /// <param name="size">New size for the memory block, in bytes.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe void* realloc(void* ptr, int size);

        /// <summary> Sets the starting seed value for the pseudorandom number generator. </summary>
        /// <param name="seed">Seed for pseudorandom number generation.</param>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern void srand(int seed);

        /// <summary> Generates a pseudorandom number. </summary>
        [DllImport(MSVCRT, CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern short rand();
    }
}
