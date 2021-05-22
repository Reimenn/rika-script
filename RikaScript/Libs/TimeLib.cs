using System.Collections.Generic;
using System.Diagnostics;
using RikaScript.Exception;
using RikaScript.Methods;

namespace RikaScript.Libs
{
    public class TimeLib : ScriptLibBase
    {
        private Dictionary<string, Stopwatch> _watchs = new Dictionary<string, Stopwatch>();

        public TimeLib() : base("time", "v0.1.0")
        {
            Info.SetPreface("RikaScript 时间类库")
                .AddHelp("start_watch(name)", "开始一个定时器计时")
                .AddHelp("end_watch(name)", "结束一个定时器并返回结果")
                .AddHelp("end_watch_info(name)", "结束一个定时器计时并显示")
                .SetEnding("这几个方法不会返回任何内容，只需要根据 name 参数确定定时器即可");
        }

        public object start_watch(object name)
        {
            var s = new Stopwatch();
            _watchs[name.String()] = s;
            s.Start();
            return s;
        }

        public object end_watch(object name)
        {
            if (!_watchs.ContainsKey(name.String()))
                throw new RuntimeException("找不到定时器：" + name);
            var s = _watchs[name.String()];
            _watchs.Remove(name.String());
            return s.Elapsed;
        }

        public void end_watch_log(object name)
        {
            var res = end_watch(name);
            Runtime.Logger.Info("Watch " + name + " : " + res);
        }

        protected override bool OtherCall(string name, object[] args, out object res)
        {
            throw new NotFoundMethodException(MethodName.ToString(name, args.Length));
        }
    }
}