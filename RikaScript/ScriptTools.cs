using System;
using System.Collections.Generic;
using System.Text;
using RikaScript.Exception;

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
            switch (obj)
            {
                case null:
                    return 0;
                case double d:
                    return d;
                default:
                    try
                    {
                        return double.Parse(obj.String());
                    }
                    catch
                    {
                        throw new RuntimeException("无法转换成数值类型：" + obj);
                    }
            }
        }

        /// <summary>
        /// Object 快速转换成 long
        /// </summary>
        public static long Long(this object obj)
        {
            switch (obj)
            {
                case null:
                    return 0;
                case long i:
                    return i;
                default:
                    try
                    {
                        return long.Parse(obj.String());
                    }
                    catch
                    {
                        throw new RuntimeException("无法转换成数值类型：" + obj);
                    }
            }
        }

        /// <summary>
        /// Object 快速转换成 bool，null = false，大于零的数为true，其他数为false，bool值直接返回
        /// </summary>
        public static bool Bool(this object obj)
        {
            switch (obj)
            {
                case null:
                    return false;
                case bool b:
                    return b;
                default:
                    try
                    {
                        return obj.Double() > 0;
                    }
                    catch (RuntimeException re)
                    {
                        return true;
                    }
            }
        }

        /// <summary>
        /// Object 快速转换成 string，null 为 空字符串
        /// </summary>
        public static string String(this object obj)
        {
            return obj == null ? "" : obj.ToString();
        }

        /// <summary>
        /// 给 stringBuilder 用的 pop，获取全部内容并清空
        /// </summary>
        public static string Pop(this StringBuilder sb)
        {
            var s = sb.ToString();
            sb.Remove(0, sb.Length);
            return s;
        }

        /// <summary>
        /// 只要有一个数是小数，就返回true
        /// </summary>
        public static bool AnyIsDecimal(object a, object b)
        {
            return a.IsDecimal() || b.IsDecimal();
        }

        /// <summary>
        /// 是否是小数
        /// </summary>
        public static bool IsDecimal(this object obj)
        {
            return obj is double || obj is float;
        }

        /// <summary>
        /// 是否是整数
        /// </summary>
        public static bool IsInteger(this object obj)
        {
            return obj is long || obj is int || obj is short || obj is byte;
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

        /// <summary>
        /// 解析字面值，返回 string、int(默认)、long、float(默认)、double
        /// </summary>
        public static object ParseData(string source, Dictionary<string, object> data)
        {
            // 判断是否为空，为空返回 null
            if (source.Length == 0 || source == "null")
            {
                return null;
            }

            switch (source)
            {
                case "false":
                    return false;
                case "true":
                    return true;
            }

            // 判断是否是字符串，长度大于等于2才能是字符串
            if (source.Length >= 2)
            {
                // 两种定界符都可以
                if (source.StartsWith("'") && source.EndsWith("'"))
                {
                    return source.Substring(1, source.Length - 2);
                }

                // 两种定界符都可以
                if (source.StartsWith("\"") && source.EndsWith("\""))
                {
                    return source.Substring(1, source.Length - 2);
                }
            }

            // 判断第一位是不是数字，如果是，则转换成对应的数字类型
            if (source[0] >= '0' && source[0] <= '9')
            {
                // if (source.EndsWith("d") || source.EndsWith("D"))
                //     return double.Parse(source.Substring(0, source.Length - 1));
                //
                // if (source.EndsWith("f") || source.EndsWith("F"))
                //     return float.Parse(source.Substring(0, source.Length - 1));
                //
                // if (source.EndsWith("l") || source.EndsWith("L"))
                //     return long.Parse(source.Substring(0, source.Length - 1));

                if (source.Contains("."))
                    return double.Parse(source);
                return long.Parse(source);
            }

            // 最后断定它是个变量
            if (data.ContainsKey(source))
            {
                return data[source];
            }

            throw new NotFoundVarException(source);
        }
    }

    /// <summary>
    /// 表示左括号的占位符
    /// </summary>
    public class InfixStart
    {
    }
}