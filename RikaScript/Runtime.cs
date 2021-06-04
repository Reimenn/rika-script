using System.Collections.Generic;
using System.Linq;
using System.Text;
using RikaScript.Exception;
using RikaScript.Libs;
using RikaScript.Libs.Base;
using RikaScript.Logger;
using RikaScript.Methods;

namespace RikaScript
{
    /// <summary>
    /// 脚本运行时
    /// </summary>
    public class Runtime
    {
        /// <summary>
        /// 输出用的
        /// </summary>
        public readonly LoggerBase Logger;

        /// <summary>
        /// 变量们
        /// </summary>
        private readonly Dictionary<string, object> _dataMap = new Dictionary<string, object>();

        /// <summary>
        /// 类库们
        /// </summary>
        private readonly Dictionary<string, ScriptLibBase> _libs = new Dictionary<string, ScriptLibBase>();

        /// <summary>
        /// 方法们
        /// </summary>
        private readonly Dictionary<string, IMethod> _methods = new Dictionary<string, IMethod>();

        /// <summary>
        /// 持久方法们，不能被覆盖的那些方法
        /// </summary>
        private readonly HashSet<string> _keepMethods = new HashSet<string>();

        /// <summary>
        /// 运算符优先级们
        /// </summary>
        private readonly Dictionary<string, int> _priority = new Dictionary<string, int>();

        /// <summary>
        /// 赋值运算符们
        /// </summary>
        private readonly string[] _assignmentOperator = {"=", "+=", "-=", "/=", "*="};

        /// <summary>
        /// 符号栈和数值栈
        /// </summary>
        private readonly Stack<string> _ops = new Stack<string>();

        private readonly Stack<object> _values = new Stack<object>();

        /// <summary>
        /// 解析表达式时临时用的
        /// </summary>
        private readonly StringBuilder _stringBuilder = new StringBuilder();

        /// <summary>
        /// 最后一次执行的代码，显示报错的时候用得到
        /// </summary>
        private string _lastCode = "";

        /// <summary>
        /// 可以自己写一个 Logger 用于在不同环境下输出内容
        /// </summary>
        /// <param name="logger">输出用的接口</param>
        public Runtime(LoggerBase logger)
        {
            this.Logger = logger;
            AddLib(new StandardLib());
        }

        /// <summary>
        /// 计算一下暂存的中缀表达式，可以设置一个优先级限制
        /// </summary>
        private void CalcInfixExpression(Stack<object> values, Stack<string> ops, int minPriority = int.MinValue)
        {
            // 计算到空或遇到 ( 为止
            while (ops.Count > 0 && ops.Peek() != "(")
            {
                // 尝试获取优先级或0
                _priority.TryGetValue(ops.Peek(), out var priority);

                // 赋值运算符的优先级放到最后
                if (_assignmentOperator.Contains(ops.Peek()))
                    priority = int.MinValue;

                // 优先级不够就 break 了
                if (priority < minPriority)
                    break;

                var op = ops.Pop();
                var rightValue = values.Pop();
                var leftValue = values.Pop();

                // 赋值操作处理
                if (_assignmentOperator.Contains(op))
                {
                    // 左值必须是一个字符串
                    if (leftValue is string leftValueString)
                    {
                        if (op == "=") // 等于号特殊处理
                        {
                            SetValue(leftValueString, rightValue);
                            values.Push(rightValue);
                        }
                        else // 其他那几个相对赋值运算符
                        {
                            var value = GetValue<object>(leftValueString).Double();
                            var res = value;
                            switch (op)
                            {
                                case "+=":
                                    res = value + rightValue.Double();
                                    break;
                                case "-=":
                                    res = value - rightValue.Double();
                                    break;
                                case "*=":
                                    res = value * rightValue.Double();
                                    break;
                                case "/=":
                                    res = value / rightValue.Double();
                                    break;
                                default:
                                    Logger.Error("匹配不到赋值运算符：" + op);
                                    break;
                            }

                            SetValue(leftValueString, res);
                            values.Push(res);
                        }
                    }
                    else
                    {
                        throw new RuntimeException("赋值号左边不是一个变量");
                    }
                }
                // 正常计算
                else
                {
                    var methodKey = op + "^2";
                    if (_methods.ContainsKey(methodKey))
                    {
                        values.Push(
                            _methods[methodKey].Call(new object[] {leftValue, rightValue}, out var res) ? res : null);
                    }
                    else
                    {
                        throw new NotFoundMethodException(methodKey);
                    }
                }
            }
        }

        /// <summary>
        /// 计算一下暂存的前缀表达式
        /// </summary>
        private void CalcPrefixExpression(Stack<object> values, Stack<string> ops)
        {
            if (ops.Peek() != "(")
                CalcInfixExpression(values, ops);
            if (ops.Pop() != "(")
            {
                throw new RuntimeException("有问题，前缀计算的符号栈最后一个东西应该是(");
            }

            var op = ops.Pop();
            var popValues = new List<object>();
            while (!(values.Peek() is InfixStart))
            {
                popValues.Insert(0, values.Pop());
            }

            // 弹出括号前缀
            values.Pop();

            var vals = new List<object>();

            for (var i = 0; i < popValues.Count; i++)
            {
                vals.Add(popValues[i]);
            }

            var methodKey = op + "^" + vals.Count;
            if (_methods.ContainsKey(methodKey))
            {
                values.Push(_methods[methodKey].Call(vals.ToArray(), out var res) ? res : null);
            }
            else
            {
                throw new NotFoundMethodException(methodKey);
            }
        }

        /// <summary>
        /// 添加类库，重复添加会被覆盖，如果有特殊需求可以给类库起别名，起别名还能防止类库被覆盖
        /// </summary>
        public void AddLib(ScriptLibBase lib, string alias = null)
        {
            if (alias == "std")
            {
                throw new ImportLibraryException("类库别名不能是 std");
            }

            // 别名处理
            lib.Runtime = this;
            if (alias == null)
            {
                alias = string.IsNullOrEmpty(lib.LibName) ? lib.GetType().Name : lib.LibName;
            }

            // 添加类库标识
            _libs[alias] = lib;

            HashSet<string> keepMethods = new HashSet<string>();

            // 遍历方法
            foreach (var m in lib.GetType().GetMethods())
            {
                // 遍历每个方法的特征们
                var attrs = m.GetCustomAttributes(true);
                foreach (var attr in attrs)
                {
                    // 仅处理 Method 特征
                    if (!(attr is Method methodAttr)) continue;

                    // 获取函数名，最原始的名字
                    var sourceName = methodAttr.Name.Length > 0 ? methodAttr.Name : m.Name;
                    // 添加到函数表，带上类库前缀 和 参数数量
                    _methods[alias + "." + sourceName + "^" + m.GetParameters().Length] =
                        MethodFactory.Create(lib, m);
                    if (methodAttr.Priority != 0)
                    {
                        _priority[alias + "." + sourceName] = methodAttr.Priority;
                    }

                    // 如果已经存在这个持久函数，则跳过下面那些操作
                    if (_keepMethods.Contains(sourceName)) continue;

                    // 添加到函数表，带上参数数量，没有类库前缀
                    _methods[sourceName + "^" + m.GetParameters().Length] =
                        MethodFactory.Create(lib, m);
                    // 如果有优先级，则添加到优先级表
                    if (methodAttr.Priority != 0)
                    {
                        _priority[sourceName] = methodAttr.Priority;
                    }

                    // 等待被添加到持久方法列表
                    if (methodAttr.Keep)
                    {
                        keepMethods.Add(sourceName);
                    }
                }
            }

            // 添加到持久方法列表
            foreach (var keepMethod in keepMethods)
            {
                _keepMethods.Add(keepMethod);
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
        /// 获取数据集中的一个数据
        /// </summary>
        public T GetValue<T>(string varName)
        {
            if (!_dataMap.ContainsKey(varName))
                throw new NotFoundVarException(varName);
            var val = _dataMap[varName];
            if (val == null) return default;
            return (T) val;
        }

        /// <summary>
        /// 设置数据集中的一个数据，如果不存在则会创建
        /// </summary>
        public void SetValue(string name, object val)
        {
            _dataMap[name.ToString()] = val;
        }

        /// <summary>
        /// 是否存在某个变量
        /// </summary>
        public bool ExistValue(string name)
        {
            return _dataMap.ContainsKey(name);
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
        /// 执行脚本代码，返回结果，代码中支持注释，不支持语法结构
        /// </summary>
        public object Execute(string script)
        {
            if (string.IsNullOrEmpty(script)) return true;
            _lastCode = script;
            try
            {
                // 准备计算
                _values.Clear();
                _ops.Clear();
                _stringBuilder.Pop();
                // 下个数是不是运算符
                var isOperator = false;
                var currentStringStart = ' ';
                for (var index = 0; index < script.Length; index++)
                {
                    // 当前符号
                    var currentChar = script[index];

                    // 字符串处理，这段代码最后 continue 了
                    if (currentStringStart != ' ')
                    {
                        // 如果遇到了字符串停止的地方
                        if (currentChar == currentStringStart)
                        {
                            currentStringStart = ' ';
                        }
                        // 转义符处理，原理就是如果下一位能转义，就把转移结果加进来并且 index + 1跳过下一位，同时不再append当前字符
                        else if (currentChar == '\\')
                        {
                            var next = script[index + 1];
                            switch (next)
                            {
                                case 'n':
                                    _stringBuilder.Append("\n");
                                    index += 1;
                                    continue;
                                case 't':
                                    _stringBuilder.Append("\t");
                                    index += 1;
                                    continue;
                                case '\\':
                                    _stringBuilder.Append("\\");
                                    index += 1;
                                    continue;
                                case '"':
                                    _stringBuilder.Append("\"");
                                    index += 1;
                                    continue;
                                case '\'':
                                    _stringBuilder.Append("'");
                                    index += 1;
                                    continue;
                            }
                        }

                        _stringBuilder.Append(currentChar);
                        continue;
                    }

                    // 遇到注释退出
                    if (currentChar == '/' && script[index + 1] == '/')
                    {
                        break;
                    }

                    // 非字符串内部
                    switch (currentChar)
                    {
                        case '\'':
                        case '"':
                            currentStringStart = currentChar;
                            _stringBuilder.Append(currentChar);
                            break;
                        case ' ':
                            // 整理字符串
                            var tempString = _stringBuilder.Pop();
                            // 空字符串跳过
                            if (string.IsNullOrEmpty(tempString)) break;
                            // 根据是否是运算符，压入不同栈
                            if (isOperator)
                            {
                                _priority.TryGetValue(tempString, out var currentOperatorPriority);
                                // 压入符号栈之前根据优先级先带优先级限制的中缀计算一次
                                CalcInfixExpression(_values, _ops, currentOperatorPriority);
                                _ops.Push(tempString);
                                isOperator = false;
                            }
                            else
                            {
                                _values.Push(ScriptTools.ParseData(tempString, _dataMap));
                                isOperator = true;
                            }

                            break;
                        case '(':
                            // 运算符进符号栈
                            var leftBracketBefore = _stringBuilder.Pop();
                            _ops.Push(!string.IsNullOrEmpty(leftBracketBefore) ? leftBracketBefore : "make");
                            // 左括号压入双栈
                            _values.Push(new InfixStart());
                            _ops.Push("(");
                            isOperator = false;
                            break;
                        case ',':
                            // 前值入栈
                            var commaBefore = _stringBuilder.Pop();
                            if (!string.IsNullOrEmpty(commaBefore))
                                _values.Push(ScriptTools.ParseData(commaBefore, _dataMap));
                            // 中缀运算
                            CalcInfixExpression(_values, _ops);
                            isOperator = false;
                            break;
                        case ')':
                            // 前值入栈
                            var rightBracketBefore = _stringBuilder.Pop();
                            if (!string.IsNullOrEmpty(rightBracketBefore))
                                _values.Push(ScriptTools.ParseData(rightBracketBefore, _dataMap));
                            // 前缀运算
                            CalcPrefixExpression(_values, _ops);
                            isOperator = true;
                            break;
                        default:
                            // 追加到 builder
                            _stringBuilder.Append(currentChar);
                            break;
                    }
                }

                // 式子结尾，最后来一次，防止一条纯中缀表达式结尾丢失
                var s = _stringBuilder.Pop();
                if (!string.IsNullOrEmpty(s)) _values.Push(ScriptTools.ParseData(s, _dataMap));
                CalcInfixExpression(_values, _ops);

                // 数值栈剩余一个，符号栈全空，表示正常执行
                if (_values.Count == 1 && _ops.Count == 0)
                    return _values.Pop();
                // 否则可能是有问题
                throw new RuntimeException("出现未知错误，堆栈剩余元素不是一个");
            }
            catch (System.Exception e)
            {
                Logger.ShowException(e, _lastCode);
                return null;
            }
        }
    }
}