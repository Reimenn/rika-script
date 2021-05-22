namespace RikaScript.Exception
{
    public class NotFoundInfoException:RikaScriptException
    {
        public NotFoundInfoException(string info) : base("找不到相关信息：" + info)
        {
        }
    }
}