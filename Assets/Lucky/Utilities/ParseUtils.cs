using System.Collections.Generic;

namespace Lucky.Utilities
{
    public static class ParseUtils
    {
        /// 判断在 string 中是否存在一个 a 使得 string[0:index(a) + 1] 里都没有 b, 也就是 a 必须在最前面, 有的话返回索引, 没的话返回 -1
        public static int FindABeforeB(this string str,  char a, HashSet<char> b)
        {
            int n = str.Length;
            for (int i = 0; i < n; i++)
            {
                if (str[i] == a)
                    return i;
                if (b.Contains(str[i]))
                    return -1;
            }

            return -1;
        }
    }
}