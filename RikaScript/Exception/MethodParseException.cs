namespace RikaScript.Exception
{
    public class MethodParseException:RikaScriptException
    {
        /// <summary>
        /// 方法反射解析错误
        /// </summary>
        /// <param name="info">方法全名</param>
        public MethodParseException(string info) : base("类库内部方法反射解析错误：" + info)
        {
        }
    }
}