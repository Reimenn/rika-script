namespace RikaScript.Exception
{
    public class NotFoundMethodException:RikaScriptException
    
    {
        /// <summary>
        /// 找不到类库内部方法
        /// </summary>
        /// <param name="info">方法名</param>
        public NotFoundMethodException(string info) : base("找不到类库内部函数：" + info)
        {
        }
    }
}