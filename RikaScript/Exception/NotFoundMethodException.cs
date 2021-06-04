using RikaScript.Methods;

namespace RikaScript.Exception
{
    public class NotFoundMethodException : RikaScriptException

    {
        /// <summary>
        /// 找不到类库内部函数
        /// </summary>
        public NotFoundMethodException(MethodName name) : base("找不到类库内部函数：" + name.ToString())
        {
        }

        public NotFoundMethodException(string info) : base("找不到类库内部函数：" + info)
        {
        }

        public NotFoundMethodException(string name, int args) : base("找不到类库内部函数：" + MethodName.ToString(name, args))
        {
        }
    }
}