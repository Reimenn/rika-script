namespace RikaScript.Exception
{
    public class NotFoundVarException:RikaScriptException
    {
        /// <summary>
        /// 找不到变量
        /// </summary>
        /// <param name="info">变量名</param>
        public NotFoundVarException(string info) : base("找不到变量：" + info)
        {
        }
    }
}