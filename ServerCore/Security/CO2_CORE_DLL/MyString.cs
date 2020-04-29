// * ************************************************************
// * * START:                                       mystring.cs *
// * ************************************************************

// * ************************************************************
// *                      INFORMATIONS
// * ************************************************************
// * String extensions for the library.
// * mystring.cs
// * 
// * --
// *
// * Feel free to use this class in your projects, but don't
// * remove the header to keep the paternity of the class.
// * 
// * ************************************************************
// *                      CREDITS
// * ************************************************************
// * Originally created by CptSky (January 17th, 2011)
// * Copyright (C) 2011 CptSky
// *
// * ************************************************************

using System;
using System.Text;

namespace CO2_CORE_DLL
{
    public unsafe static class MyString
    {
        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static Byte* ToPointer(this String Str)
        {
            Byte[] Buffer = Encoding.GetEncoding("Windows-1252").GetBytes(Str + "\0");
            Byte* ptr = (Byte*)Kernel.malloc(Buffer.Length);

            fixed (Byte* pBuffer = Buffer)
                Kernel.memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }

        /// <summary>
        /// Get a pointer to the Windows-1252 encoded string.
        /// It will create a null-terminating string...
        /// </summary>
        public static Byte* ToPointer(this String Str, Byte* ptr)
        {
            Byte[] Buffer = Encoding.GetEncoding("Windows-1252").GetBytes(Str + "\0");
            fixed (Byte* pBuffer = Buffer)
                Kernel.memcpy(ptr, pBuffer, Buffer.Length);
            return ptr;
        }
    }
}

// * ************************************************************
// * * END:                                         mystring.cs *
// * ************************************************************