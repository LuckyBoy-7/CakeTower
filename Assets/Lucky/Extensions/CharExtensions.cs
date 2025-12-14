namespace Lucky.Extensions
{
    public static class CharExtensions
    {
        public static bool IsAsciiPrintable(this char c)
        {
            return c >= 32 && c  <= 126;
        }
    }
}