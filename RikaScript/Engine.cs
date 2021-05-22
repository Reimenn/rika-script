using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RikaScript.Exception;
using RikaScript.Libs;
using RikaScript.Logger;
using FormatException = RikaScript.Exception.FormatException;

namespace RikaScript
{
    /// <summary>
    /// RikaScript 引擎，支持 if while func 等语法
    /// </summary>
    public class Engine
    {
        /// <summary>
        /// 所用运行时
        /// </summary>
        public readonly Runtime Runtime;

        /// <summary>
        /// 方法列表，存放的是方法名和内容代码
        /// </summary>
        private readonly Dictionary<string, string> _funcations = new Dictionary<string, string>();

        /// <summary>
        /// 最后一次执行的代码，显示报错的时候用得到
        /// </summary>
        private string _lastCode = "";

        /// <summary>
        /// 当前构建的方法名
        /// </summary>
        private string _curFuncName = "";

        private StringBuilder _funcBuilder;

        public Engine(LoggerBase logger)
        {
            Runtime = new Runtime(logger);
        }

        public Engine(Runtime runtime)
        {
            Runtime = runtime;
        }

        /// <summary>
        /// 执行一段代码
        /// </summary>
        public void Execute(string code)
        {
            var codes = code.Split('\n');
            foreach (var sc in codes)
            {
                try
                {
                    var s = sc.Trim();
                    if (s.Length == 0 || s.StartsWith("//")) continue;
                    // 如果当前不在函数定义当中，则执行各种命令
                    if (_curFuncName == "")
                    {
                        _lastCode = s;
                        if (s.StartsWith("func "))
                        {
                            var m = Regex.Match(s, @"func (.+?) ?{");
                            if (!m.Success)
                                throw new EngineException("func 格式错误");

                            var funcName = m.Groups[1].ToString();
                            _curFuncName = funcName;
                            _funcBuilder = new StringBuilder();
                        }
                        else if (s.StartsWith("call "))
                        {
                            var match = Regex.Match(s, @"call (.+)");
                            if (!match.Success)
                                throw new EngineException("call 格式错误");
                            var funcName = match.Groups[1].ToString();

                            if (!_funcations.ContainsKey(funcName))
                                throw new NotFoundFuncException(funcName);

                            var funcCode = _funcations[funcName];
                            this.Execute(funcCode);
                        }
                        else if (s.StartsWith("exec "))
                        {
                            var match = Regex.Match(s, "exec (.+)");
                            if (!match.Success)
                                throw new EngineException("exec 格式错误");
                            var group = match.Groups;
                            var allText = File.ReadAllText(group[1].Value);
                            this.Execute(allText);
                        }
                        else if (s.StartsWith("if @"))
                        {
                            var match = Regex.Match(s, @"if @(.+?) (.+)");
                            if (!match.Success)
                                throw new EngineException("if 格式错误");
                            var group = match.Groups;
                            var varName = group[1].Value;
                            var funcName = group[2].Value;

                            if (Runtime.GetBool(varName))
                            {
                                if (funcName == "return")
                                    break;
                                this.Execute("call " + funcName);
                            }
                        }
                        else if (s.StartsWith("while @"))
                        {
                            var match = Regex.Match(s, @"while @(.+?) (.+)");
                            if (!match.Success)
                                throw new EngineException("while 格式错误");

                            var group = match.Groups;
                            var varName = group[1].Value;
                            var funcName = group[2].Value;

                            while (Runtime.GetBool(varName))
                            {
                                this.Execute("call " + funcName);
                            }
                        }
                        else if (s.StartsWith("echo "))
                        {
                            var strings = s.Split(' ');
                            if (strings.Length != 2)
                                throw new EngineException("echo 格式错误");
                            Runtime.Echo = strings[1] == "true";
                        }
                        else if (s.Trim() == "help")
                        {
                            Execute("help()");
                        }
                        else
                        {
                            Runtime.Execute(s);
                        }
                    }
                    // 如果正在定义函数，则把全部内容添加到函数内容里
                    else
                    {
                        if (s.StartsWith("func "))
                        {
                            throw new EngineException("不能在函数内部定义函数");
                        }

                        // 结束函数定义
                        if (s.Trim() != "}")
                        {
                            _funcBuilder.Append(s + "\n");
                        }
                        else
                        {
                            _funcations[_curFuncName.Trim()] = _funcBuilder.ToString();
                            _curFuncName = "";
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Runtime.Logger.ShowException(e, _lastCode);
                }
            }
        }
    }
}