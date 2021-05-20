namespace RikaScript.Exception
{
    public class FormatException : RikaScriptException
    {
        /// <summary>
        /// 语法格式错误
        /// </summary>
        /// <param name="info">完整错误代码</param>
        public FormatException(string info) : base("语法格式错误：" + info)
        {
        }
    }
}