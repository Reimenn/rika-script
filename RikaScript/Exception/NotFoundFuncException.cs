namespace RikaScript.Exception
{
    public class NotFoundFuncException:RikaScriptException
    {
        /// <summary>
        /// 找不到过程
        /// </summary>
        /// <param name="info">过程名</param>
        public NotFoundFuncException(string info) : base("找不到过程：" + info)
        {
        }
    }
}