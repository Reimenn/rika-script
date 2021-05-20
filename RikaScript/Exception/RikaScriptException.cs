namespace RikaScript.Exception
{
    /// <summary>
    /// 异常基类
    /// </summary>
    public abstract class RikaScriptException:System.Exception
    {
        protected readonly string info;

        public RikaScriptException(string info)
        {
            this.info = info;
        }

        public override string ToString()
        {
            return info;
        }
    }
}