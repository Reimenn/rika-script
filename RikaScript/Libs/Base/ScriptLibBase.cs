using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using System.Text;
using RikaScript.Exception;
using RikaScript.Methods;

namespace RikaScript.Libs.Base
{
    /// <summary>
    /// 一切类库的基类
    /// </summary>
    public abstract class ScriptLibBase
    {
        /// <summary>
        /// 类库默认别名
        /// </summary>
        public readonly string LibName = null;

        /// <summary>
        /// 类库版本
        /// </summary>
        public readonly string Version = null;

        /// <summary>
        /// 帮助信息
        /// </summary>
        public readonly string Help = null;

        public string CompleteHelp = "";

        /// <summary>
        /// 这个类库所在的运行时环境
        /// </summary>
        public Runtime Runtime = null;

        protected ScriptLibBase()
        {
            // 寻找 Library 特征
            foreach (var customAttribute in GetType().GetCustomAttributes(true))
            {
                if (!(customAttribute is Library library)) continue;
                LibName = library.Name;
                Version = library.Version;
                Help = library.Help;
                break;
            }
        }

        [Method(Keep = true, Help = "显示帮助")]
        public void help()
        {
            // 如果已经生成过帮助，就不再生成了，直接输出
            if (!string.IsNullOrEmpty(CompleteHelp))
            {
                Runtime.Logger.Print(CompleteHelp);
                return;
            }

            // 生成帮助并输出
            var list = new StringBuilder("提供的函数如下：\n");
            var other = new StringBuilder("可供使用但没有帮助文档的函数如下：\n");

            foreach (var methodInfo in GetType().GetMethods())
            {
                foreach (var customAttribute in methodInfo.GetCustomAttributes(true))
                {
                    if (!(customAttribute is Method method)) continue;
                    var name = new StringBuilder(string.IsNullOrEmpty(method.Name)
                        ? methodInfo.Name
                        : method.Name);
                    name.Append("(");
                    var num = 0;
                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        if (num != 0) name.Append(", ");
                        name.Append(parameterInfo.Name);
                        num++;
                    }

                    name.Append(")");
                    if (method.Keep) name.Append(" [KEEP]");

                    if (string.IsNullOrEmpty(method.Help))
                    {
                        other.Append(name + "\n");
                    }
                    else
                    {
                        list.Append(name + "\n");
                        list.Append("\t" + method.Help + "\n");
                    }
                }
            }

            CompleteHelp = LibName + " : " + GetType().FullName + "\t[" + Version + "]\n";
            CompleteHelp += Help + "\n\n";
            CompleteHelp += list.ToString() + "\n";
            CompleteHelp += other.ToString();
            Runtime.Logger.Print(CompleteHelp);
        }

        [Method(Keep = true, Help = "搜索帮助并显示")]
        public void help(object keyword)
        {
            foreach (var methodInfo in GetType().GetMethods())
            {
                foreach (var customAttribute in methodInfo.GetCustomAttributes(true))
                {
                    if (!(customAttribute is Method method)) continue;
                    var name = new StringBuilder(string.IsNullOrEmpty(method.Name)
                        ? methodInfo.Name
                        : method.Name);
                    name.Append("(");
                    var num = 0;
                    foreach (var parameterInfo in methodInfo.GetParameters())
                    {
                        if (num != 0) name.Append(", ");
                        name.Append(parameterInfo.Name);
                        num++;
                    }

                    name.Append(")");
                    if (method.Keep) name.Append(" [KEEP]");

                    var nameStr = name.ToString();
                    
                    if (nameStr.Contains(keyword.String()) || method.Help.Contains(keyword.String()))
                    {
                        Console.WriteLine(nameStr);
                        Console.WriteLine("\t" + method.Help);
                    }
                }
            }
        }
    }
}