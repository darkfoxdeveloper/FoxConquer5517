using System;

namespace ServerCore
{
    public static class Utils
    {
        public unsafe static void Memset(void* str, byte b, int n)
        {
            int i;
            byte* s = (byte*)str;
            for (i = 0; i < n; i++)
                s[i] = b;
        }

        public unsafe static void Memcpy(void* dest, void* src, Int32 size)
        {
            Int32 count = size / sizeof(long);
            for (Int32 i = 0; i < count; i++)
                *(((long*)dest) + i) = *(((long*)src) + i);

            Int32 pos = size - (size % sizeof(long));
            for (Int32 i = 0; i < size % sizeof(long); i++)
                *(((Byte*)dest) + pos + i) = *(((Byte*)src) + pos + i);
        }
    }
}
