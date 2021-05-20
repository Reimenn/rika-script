namespace RikaScript.Exception
{
    public class NotFoundLibException:RikaScriptException
    {
        /// <summary>
        /// 找不到类库
        /// </summary>
        /// <param name="info">库的名字</param>
        public NotFoundLibException(string info) : base("找不到类库：" + info)
        {
        }
    }
}