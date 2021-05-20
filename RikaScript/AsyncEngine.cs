using System;
using System.Collections.Generic;
using System.Threading;
using RikaScript.Logger;

namespace RikaScript
{
    /// <summary>
    /// 异步解析引擎
    /// </summary>
    public class AsyncEngine : Engine
    {
        /// <summary>
        /// 代码队列
        /// </summary>
        private readonly Queue<string> _codeList = new Queue<string>();

        /// <summary>
        /// 代码解析线程
        /// </summary>
        public readonly Thread Thread;

        /// <summary>
        /// 当没有代码时每次检查新代码的时间间隔
        /// </summary>
        public int PauseWaitingTime = 10;

        public AsyncEngine(LoggerBase logger) : base(logger)
        {
            Thread = new Thread(new ThreadStart(Running));
            InitThread();
        }

        public AsyncEngine(Runtime runtime) : base(runtime)
        {
            Thread = new Thread(new ThreadStart(Running));
            InitThread();
        }

        /// <summary>
        /// 线程初始化工作
        /// </summary>
        private void InitThread()
        {
            Thread.Start();
        }

        /// <summary>
        /// 添加代码到代码队列
        /// </summary>
        public void PushCode(string code)
        {
            var strings = code.Split('\n');
            foreach (var s in strings)
            {
                if (s.Trim().StartsWith("//")) continue;
                else _codeList.Enqueue(s.Trim());
            }
        }

        [Obsolete("不建议对异步引擎使用 Execute() 执行代码，推荐使用 PushCode() 方法")]
        public void Execute(string code)
        {
            Runtime.Logger.Warning("不建议对异步引擎使用 Execute() 执行代码，推荐使用 PushCode() 方法");
            base.Execute(code);
        }

        /// <summary>
        /// 异步循环调用的代码执行方法
        /// </summary>
        private void Running()
        {
            while (true)
            {
                if (_codeList.Count > 0)
                {
                    var peek = _codeList.Dequeue();
                    if (peek.Trim().Length > 0)
                    {
                        base.Execute(peek);
                    }
                }
                else
                {
                    Thread.Sleep(PauseWaitingTime);
                }
            }
        }
    }
}