using System;

namespace RikaScript.Logger
{
    /// <summary>
    /// 控制台输出
    /// </summary>
    public class ConsoleLogger : LoggerBase
    {
        public override void Print(object message)
        {
            Console.WriteLine(message);
        }

        public override void Info(object message)
        {
            Console.WriteLine("[INFO] " + message);
        }

        public override void Warning(object message)
        {
            Console.WriteLine("[WARN] " + message);
        }

        public override void Error(object message)
        {
            Console.WriteLine("[ERROR] " + message);
        }
    }
}