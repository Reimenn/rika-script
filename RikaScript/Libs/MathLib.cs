using System;
using System.Collections.Generic;
using System.Linq;
using RikaScript.Exception;
using RikaScript.Libs.Base;

namespace RikaScript.Libs
{
    [Library("math", "v0.1.0")]
    public class MathLib : ScriptLibBase
    {
        private Dictionary<string, int> map = new Dictionary<string, int>();

        public MathLib()
        {
            map.Add("+", 0);
            map.Add("-", 0);
            map.Add("*", 10);
            map.Add("/", 10);
            map.Add(")", -10000);
            map.Add("(", -10000);
        }

        [Method(Help = "计算一段字符串公式")]
        public object calc(object val)
        {
            return Calculate(val.ToString());
        }

        private double Calculate(string s)
        {
            // 追加空格
            for (var i = 0; i < s.Length; i++)
            {
                if (!map.Keys.Contains(s[i] + "")) continue;

                s = s.Insert(i, " ");
                if (i + 2 < s.Length) s = s.Insert(i + 2, " ");
                i += 2;
            }

            // 根据空格分隔
            var arr = s.Split(' ');
            var nums = new Stack<double>();
            var ops = new Stack<string>();

            // 遍历计算
            foreach (var v in arr)
            {
                if (string.IsNullOrEmpty(v)) continue;
                if (map.Keys.Contains(v) && v != "(")
                {
                    while (ops.Count > 0 && map[ops.Peek()] >= map[v])
                    {
                        var f = ops.Pop();
                        if (f == "(") break;
                        var a = nums.Pop();
                        var b = nums.Pop();
                        nums.Push(Cale(b, a, f));
                    }

                    if (v != ")") ops.Push(v);
                }
                else if (v == "(")
                {
                    ops.Push(v);
                }
                else
                {
                    if (v != " ") nums.Push(double.Parse(v));
                }
            }

            while (ops.Count > 0)
            {
                var a = nums.Pop();
                var b = nums.Pop();
                nums.Push(Cale(b, a, ops.Pop()));
            }

            return nums.Pop();
        }

        private double Cale(double a, double b, string f)
        {
            double res = 0;
            switch (f)
            {
                case "+":
                    res = a + b;
                    break;
                case "*":
                    res = a * b;
                    break;
                case "-":
                    res = a - b;
                    break;
                case "/":
                    res = a / b;
                    break;
                default:
                    throw new RuntimeException("这是个什么符号：" + f);
            }

            return res;
        }
    }
}