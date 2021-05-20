using System.Reflection;
using RikaScript.Exception;

namespace RikaScript.Logger
{
    /// <summary>
    /// 可以自己写一个这东西用于在不同环境下输出内容
    /// </summary>
    public abstract class LoggerBase
    {
        /// <summary>
        /// 源内容输出
        /// </summary>
        /// <param name="message"></param>
        public abstract void Print(object message);

        /// <summary>
        /// 标准信息输出
        /// </summary>
        public abstract void Info(object message);

        /// <summary>
        /// 警告信息输出
        /// </summary>
        public abstract void Warning(object message);

        /// <summary>
        /// 错误信息输出
        /// </summary>
        public abstract void Error(object message);

        /// <summary>
        /// 报错显示
        /// </summary>
        public void ShowException(System.Exception e, string code)
        {
            // Error(e);

            if (e == null)
            {
                Error("[ 未知错误 ] \n\t异常代码：" + code);
                return;
            }

            switch (e)
            {
                case RikaScriptException re:
                    Error("[" + e.GetType().FullName + "] " + e + "\n\t异常代码：" + code);
                    break;
                case TargetInvocationException te:
                    ShowException(te.InnerException, code);
                    break;
                default:
                    Error("[" + e.GetType().FullName + "]\n\t异常代码：" + code);
                    break;
            }
        }
    }
}