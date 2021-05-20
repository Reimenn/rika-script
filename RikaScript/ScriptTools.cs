using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RikaScript
{
    /// <summary>
    /// 有关脚本处理的工具类
    /// </summary>
    public static class ScriptTools
    {
        /// <summary>
        /// Object 快速转换成 double
        /// </summary>
        public static double Double(this object obj)
        {
            if (obj == null) return 0;
            if (obj is double d) return d;
            return double.Parse(obj.ToString());
        }

        /// <summary>
        /// Object 快速转换成 bool
        /// </summary>
        public static bool Bool(this object obj)
        {
            if (obj is bool b) return b;
            return obj.Double() > 0;
        }

        /// <summary>
        /// Object 快速转换成 string，null 为 空字符串
        /// </summary>
        public static string String(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        /// <summary>
        /// 清除多余的空格，不会清除定界符内部的空格
        /// </summary>
        public static string SpaceClear(string str, string delimiters = "\"'")
        {
            var inDelimiter = false;
            var lastIsSpace = false;
            var curDelimiter = ' ';
            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];
                if (delimiters.Contains(c + ""))
                {
                    var leftOne = i > 0 ? str[i - 1] : c;
                    if (leftOne != '\\')
                    {
                        if (inDelimiter && c != curDelimiter) continue;
                        inDelimiter = !inDelimiter;
                        if (inDelimiter) curDelimiter = c;
                    }
                }

                if (!inDelimiter)
                {
                    if (lastIsSpace && c == ' ')
                    {
                        str = str.Remove(i, 1);
                        i--;
                    }
                    else
                    {
                        lastIsSpace = c == ' ';
                    }
                }
            }

            return str;
        }

        /// <summary>
        /// 分隔字符串，不会分隔定界符内部的东西
        /// </summary>
        public static string[] SplitByDelimiter(string str, string delimiters = "\"'", char sep = ',')
        {
            var inDelimiter = false;
            var curDelimiter = ' ';
            var lastIndex = 0;
            var res = new List<string>();

            for (var i = 0; i < str.Length; i++)
            {
                var c = str[i];

                if (delimiters.Contains(c + ""))
                {
                    var leftOne = i > 0 ? str[i - 1] : c;
                    if (leftOne != '\\')
                    {
                        if (inDelimiter && c != curDelimiter) continue;
                        inDelimiter = !inDelimiter;
                        if (inDelimiter) curDelimiter = c;
                    }
                }

                if (!inDelimiter && (c == sep || i == str.Length - 1))
                {
                    res.Add(str.Substring(lastIndex, i - lastIndex + 1).TrimEnd(sep).Trim());
                    lastIndex = i + 1;
                }
            }

            return res.ToArray();
        }
    }
}