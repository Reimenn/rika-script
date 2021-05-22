using System;
using RikaScript.Exception;

namespace RikaScript.Libs
{
    public class RandomLib : ScriptLibBase

    {
        public RandomLib() : base("random", "v0.1.0")
        {
            Info.SetPreface("RikaScript 随机数生成类库")
                .AddHelp("random()", "获取一个随机小数")
                .AddHelp("range(start,end)", "获取一个范围内的随机小数");

        }

        public object random()
        {
            var ran = new Random();
            return ran.NextDouble();
        }

        public object range(object a, object b)
        {
            var ran = new Random();
            return a.Double() + (ran.NextDouble() * b.Double() - a.Double());
        }

        protected override bool OtherCall(string name, object[] args, out object res)
        {
            throw new NotFoundMethodException(name);
        }
    }
}