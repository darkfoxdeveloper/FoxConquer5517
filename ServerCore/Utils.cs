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
    }
}
