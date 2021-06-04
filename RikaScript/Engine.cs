using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using RikaScript.Exception;
using RikaScript.Logger;

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
        /// 过程列表，存放的是过程名和过程代码
        /// </summary>
        private readonly Dictionary<string, string> _funcations = new Dictionary<string, string>();

        /// <summary>
        /// 最后一次执行的代码，显示报错的时候用得到
        /// </summary>
        private string _lastCode = "";

        /// <summary>
        /// 当前构建的过程名
        /// </summary>
        private string _currentBuildingFuncName = "";

        /// <summary>
        /// 当前构建的过程用途，可以有func、
        /// </summary>
        private string _currentBuildingFuncType = "";

        /// <summary>
        /// 过程构建
        /// </summary>
        private StringBuilder _funcBuilder;

        /// <summary>
        /// 当前{的深度
        /// </summary>
        private int _currentDepth = 0;

        /// <summary>
        /// 做随机数用的
        /// </summary>
        private readonly Random _random = new Random((int) DateTime.Now.Ticks);

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
                    _lastCode = s;
                    if (s.Length == 0 || s.StartsWith("//")) continue;
                    // 如果当前不在构建过程当中，则执行各种命令
                    if (_currentBuildingFuncName == "")
                    {
                        if (s.StartsWith("set "))
                        {
                            var m = Regex.Match(s, @"^set +(.+?) *([\+\*\-\/]?=) *(.+)$");
                            if (!m.Success)
                                throw new EngineException("set 格式错误");

                            var varName = m.Groups[1].Value;
                            var op = m.Groups[2].Value;
                            var value = m.Groups[3].Value;

                            Runtime.Execute("'" + varName + "' " + op + " " + value);
                        }
                        else if (s.StartsWith("if "))
                        {
                            var match = Regex.Match(s, @"^if +(.+)( *({)| +(.+))$");
                            if (!match.Success)
                                throw new EngineException("if 格式错误");
                            var group = match.Groups;
                            var varName = group[1].Value;
                            var funcName = group[2].Value;
                            var value = Runtime.Execute(varName).Bool();
                            if (funcName == "{")
                            {
                                _currentBuildingFuncName = "_IF_FUNCTION_" + _random.Next();
                                _currentBuildingFuncType = value ? "if" : "ignore";
                                _currentDepth++;
                                _funcBuilder = new StringBuilder();
                            }
                            else if (value)
                            {
                                if (funcName == "return")
                                    break;
                                this.Execute("call " + funcName);
                            }
                        }
                        else if (s.StartsWith("while "))
                        {
                            var match = Regex.Match(s, @"^while +(.+)( *({)| +(.+))$");
                            if (!match.Success)
                                throw new EngineException("while 格式错误");

                            var group = match.Groups;
                            var varName = group[1].Value;
                            var funcName = group[2].Value;

                            if (funcName == "{")
                            {
                                _currentBuildingFuncName = varName + " _WHILE_FUNCTION_" + _random.Next();
                                _currentBuildingFuncType = "while";
                                _currentDepth++;
                                _funcBuilder = new StringBuilder();
                            }
                            else
                            {
                                while (Runtime.Execute(varName).Bool())
                                {
                                    this.Execute("call " + funcName);
                                }
                            }
                        }
                        else if (s.StartsWith("call "))
                        {
                            var match = Regex.Match(s, @"^call +(.+)$");
                            if (!match.Success)
                                throw new EngineException("call 格式错误");
                            var funcName = match.Groups[1].ToString();

                            if (!_funcations.ContainsKey(funcName))
                                throw new NotFoundFuncException(funcName);

                            var funcCode = _funcations[funcName];
                            this.Execute(funcCode);
                        }
                        else if (s.StartsWith("func "))
                        {
                            var m = Regex.Match(s, @"^func +(.+?) *{$");
                            if (!m.Success)
                                throw new EngineException("func 格式错误");

                            var funcName = m.Groups[1].ToString();
                            _currentBuildingFuncName = funcName;
                            _currentDepth++;
                            _currentBuildingFuncType = "func";
                            _funcBuilder = new StringBuilder();
                        }
                        else if (s.StartsWith("exec "))
                        {
                            var match = Regex.Match(s, "^exec +(.+)$");
                            if (!match.Success)
                                throw new EngineException("exec 格式错误");
                            var group = match.Groups;
                            var allText = File.ReadAllText(group[1].Value);
                            this.Execute(allText);
                        }
                        else if (s == "help")
                        {
                            Runtime.Execute("help()");
                        }
                        else
                        {
                            Runtime.Execute(s);
                        }
                    }
                    // 如果正在定义过程，则把全部内容添加到过程内容里
                    else
                    {
                        if (s.EndsWith("{"))
                        {
                            _currentDepth++;
                        }

                        // 结束过程定义
                        if (s == "}")
                        {
                            _currentDepth--;
                            if (_currentDepth == 0)
                            {
                                var name = _currentBuildingFuncName.Trim();
                                var type = _currentBuildingFuncType;
                                var nameArr = type == "while" ? name.Split(' ') : null;
                                _currentBuildingFuncName = "";
                                _currentBuildingFuncType = "";
                                if (type == "while")
                                {
                                    _funcations[nameArr[nameArr.Length - 1]] = _funcBuilder.ToString();
                                }
                                else
                                    _funcations[name] = _funcBuilder.ToString();

                                switch (type)
                                {
                                    case "func":
                                        break;
                                    case "if":
                                        this.Execute("call " + name);
                                        _funcations.Remove(name);
                                        break;
                                    case "while":
                                        this.Execute("while " + name);
                                        _funcations.Remove(nameArr[nameArr.Length - 1]);
                                        break;
                                }
                            }
                            else
                            {
                                _funcBuilder.Append(s + "\n");
                            }
                        }
                        else
                        {
                            _funcBuilder.Append(s + "\n");
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