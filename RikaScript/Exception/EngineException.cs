namespace RikaScript.Exception
{
    public class EngineException:RikaScriptException
    {
        /// <summary>
        /// 引擎解析错误
        /// </summary>
        /// <param name="info">错误原因</param>
        public EngineException(string info) : base("引擎解析错误：" + info)
        {
        }
    }
}