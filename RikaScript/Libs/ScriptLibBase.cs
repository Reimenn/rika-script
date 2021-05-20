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
        public string LibName;

        /// <summary>
        /// 这个类库所在的运行时环境
        /// </summary>
        public Runtime Runtime = null;

        /// <summary>
        /// 预缓存方法
        /// </summary>
        private readonly Dictionary<string, IMethod> _methods = new Dictionary<string, IMethod>();

        protected ScriptLibBase()
        {
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
            if (methodName == "help")
            {
                help();
                res = null;
                return false;
            }

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
        protected abstract void help();
    }
}