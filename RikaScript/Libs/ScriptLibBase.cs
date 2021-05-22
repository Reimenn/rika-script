using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Reflection;
using RikaScript.Exception;
using RikaScript.Methods;

namespace RikaScript.Libs
{
    /// <summary>
    /// 一切类库的基类
    /// </summary>
    public abstract class ScriptLibBase
    {
        /// <summary>
        /// 类库默认别名
        /// </summary>
        public readonly string LibName;

        /// <summary>
        /// 类库版本
        /// </summary>
        public readonly string Version;

        /// <summary>
        /// 类库信息
        /// </summary>
        public readonly LibInfo Info;

        /// <summary>
        /// 这个类库所在的运行时环境
        /// </summary>
        public Runtime Runtime = null;

        /// <summary>
        /// 预缓存方法
        /// </summary>
        private readonly Dictionary<string, IMethod> _methods = new Dictionary<string, IMethod>();

        protected ScriptLibBase(string name, string version)
        {
            LibName = name;
            Version = version;
            Info = new LibInfo(this);
            // 预缓存方法
            var ms = GetType().GetMethods();
            foreach (var m in ms)
            {
                var mName = m.Name;
                if (mName[0] < 97 || mName[0] > 122 || mName.StartsWith("_")) continue;
                _methods.Add(MethodName.ToString(mName, m.GetParameters().Length), MethodFactory.Create(this, m));
            }
        }

        /// <summary>
        /// 调用该类库
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="args">参数</param>
        /// <param name="res">返回值</param>
        /// <returns>是否有返回值</returns>
        public bool Call(string methodName, object[] args, out object res)
        {
            IMethod method = null;
            var n = MethodName.ToString(methodName, args.Length);

            if (_methods.ContainsKey(n))
                method = _methods[n];
            else
                return OtherCall(methodName, args, out res);
            var hasReturn = method.Call(args, out res);
            return hasReturn;
        }

        /// <summary>
        /// 调用这个库不存在的方法时执行
        /// </summary>
        protected abstract bool OtherCall(string name, object[] args, out object res);

        /// <summary>
        /// 查看帮助
        /// </summary>
        public object help()
        {
            var res = Info.ToString();
            Runtime.Logger.Print(res);
            return res;
        }

        /// <summary>
        /// 搜索帮助
        /// </summary>
        public object help(object str)
        {
            var res = Info.SearchInfo(str.String());
            Runtime.Logger.Print(res);
            return res;
        }
    }
}