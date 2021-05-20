using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RikaScript.Exception;
using RikaScript.Libs;
using RikaScript.Logger;
using FormatException = RikaScript.Exception.FormatException;

namespace RikaScript
{
    /// <summary>
    /// 脚本运行时
    /// </summary>
    public class Runtime
    {
        public bool Echo = false;
        public readonly LoggerBase Logger;
        public readonly string Delimiter;

        private readonly Dictionary<string, object> _dataMap = new Dictionary<string, object>();
        private readonly Dictionary<string, ScriptLibBase> _libs = new Dictionary<string, ScriptLibBase>();
        private string _lastCode = "";
        private string _defaultLib = "std";

        /// <summary>
        /// 可以自己写一个 Logger 用于在不同环境下输出内容
        /// </summary>
        /// <param name="logger">输出用的接口</param>
        /// <param name="delimiter">字符串的定界符，可以同时支持多种定界符</param>
        public Runtime(LoggerBase logger, string delimiter = "\"'")
        {
            this.Logger = logger;
            this.Delimiter = delimiter;
            AddLib(new StandardLib());
        }

        /// <summary>
        /// 执行脚本代码，返回是否执行成功（不支持注释）
        /// </summary>
        public bool Execute(string methodScript)
        {
            _lastCode = methodScript;
            // 这一步只负责解析
            try
            {
                if (_libs.Count == 0)
                    throw new RuntimeException("没有类库，无法正常执行");

                // 收尾、空格处理
                methodScript = methodScript.Trim();
                methodScript = ScriptTools.SpaceClear(methodScript);
                // 空行剔除
                if (methodScript.Length == 0) return true;

                // 正则匹配
                var match = Regex.Match(methodScript, @"(.+)\((.*)\)( ?> ?.+)?$");
                if (!match.Success)
                    throw new FormatException(methodScript);

                // 函数名分隔
                var methodFullName = match.Groups[1].ToString();
                var methodNames = methodFullName.Split('.');

                if (methodNames.Length > 2)
                    throw new RikaScript.Exception.FormatException(methodScript);
                // 获取函数名和类名
                var className = methodNames.Length == 2 ? methodNames[0] : "";
                var methodName = methodNames.Length == 2 ? methodNames[1] : methodNames[0];

                // 参数处理
                var argsString = match.Groups[2].ToString();
                var argsStr = ScriptTools.SplitByDelimiter(argsString, Delimiter, ',');
                var argsObject = new List<object>();
                foreach (var arg in argsStr)
                {
                    if (arg.StartsWith("@"))
                    {
                        // 变量取值
                        var varName = arg.TrimStart('@');
                        argsObject.Add(GetValue<object>(varName));
                    }
                    else
                    {
                        // 字符串掐头去尾
                        argsObject.Add(Delimiter.Contains(arg[0])
                            ? arg.Trim(arg[0])
                            : arg);
                    }
                }

                // 保存的变量名
                var asName = match.Groups.Count >= 3 ? match.Groups[3].ToString() : "";
                if (asName != "")
                {
                    asName = asName.Split('>')[1].Trim();
                }

                if (string.IsNullOrEmpty(className))
                    className = _defaultLib;
                return Execute(className, methodName, argsObject.ToArray(), asName);
            }
            catch (System.Exception e)
            {
                Logger.ShowException(e, _lastCode);
                return false;
            }
        }

        /// <summary>
        /// 添加类库，重复添加会被覆盖，如果有特殊需求可以给类库起别名，起别名还能防止类库被覆盖
        /// </summary>
        public void AddLib(ScriptLibBase lib, string alias = null)
        {
            lib.Runtime = this;
            if (alias == null)
            {
                if (lib.LibName.Length == 0)
                {
                    _libs[lib.GetType().Name] = lib;
                }
                else
                {
                    _libs[lib.LibName] = lib;
                }
            }
            else
            {
                _libs[alias] = lib;
            }
        }

        /// <summary>
        /// 获取已经添加的库列表
        /// </summary>
        public string[] GetLibs()
        {
            var res = new string[_libs.Count];
            _libs.Keys.CopyTo(res, 0);
            return res;
        }

        /// <summary>
        /// 获取全部变量列表
        /// </summary>
        /// <returns></returns>
        public string[] GetVars()
        {
            return _dataMap.Keys.ToArray();
        }

        /// <summary>
        /// 设置默认类库
        /// </summary>
        public void SetDefaultLib(string name)
        {
            _defaultLib = name;
        }

        /// <summary>
        /// 获取数据集中的一个数据
        /// </summary>
        public T GetValue<T>(string varName)
        {
            if (!_dataMap.ContainsKey(varName))
                throw new NotFoundVarException(varName);
            var val = _dataMap[varName];
            if (val == null) return default(T);
            return (T) val;
        }

        /// <summary>
        /// 获取 bool 数据，如果原变量就是 bool 则直接返回，否则返回 变量 > 0
        /// </summary>
        /// <param name="varName"></param>
        /// <returns></returns>
        public bool GetBool(string varName)
        {
            var val = GetValue<object>(varName);
            if (val is bool b) return b;
            return val.Double() > 0;
        }

        /// <summary>
        /// 设置数据集中的一个数据，如果不存在则会创建
        /// </summary>
        public void SetValue(string name, object val)
        {
            _dataMap[name.ToString()] = val;
        }

        /// <summary>
        /// 删除某个变量
        /// </summary>
        public void DelValue(string name)
        {
            _dataMap.Remove(name);
        }

        /// <summary>
        /// 清空数据集
        /// </summary>
        public void ClearDataSet()
        {
            _dataMap.Clear();
        }

        /// <summary>
        /// 真正执行脚本
        /// </summary>
        /// <param name="className">类名</param>
        /// <param name="methodsName">方法名</param>
        /// <param name="args">参数列表</param>
        /// <param name="asName">保存结果的变量名</param>
        /// <returns>是否执行成功</returns>
        private bool Execute(string className, string methodsName, object[] args, string asName)
        {
            if (!_libs.ContainsKey(className))
                throw new NotFoundLibException(className);
            var method = _libs[className];
            method.Call(methodsName, args, out var res);
            if (!string.IsNullOrEmpty(asName))
                SetValue(asName, res);
            if (Echo && res != null)
                Logger.Print("[" + res.GetType().Name + "] " + res);
            return true;
        }
    }
}