namespace RikaScript.Exception
{
    public class NotFoundFuncException:RikaScriptException
    {
        /// <summary>
        /// 找不到方法
        /// </summary>
        /// <param name="info">方法名</param>
        public NotFoundFuncException(string info) : base("找不到方法：" + info)
        {
        }
    }
}