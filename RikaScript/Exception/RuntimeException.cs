namespace RikaScript.Exception
{
    public class RuntimeException : RikaScriptException
    {
        /// <summary>
        /// 运行时异常
        /// </summary>
        /// <param name="info">写什么就报什么错</param>
        public RuntimeException(string info) : base(info)
        {
        }
    }
}