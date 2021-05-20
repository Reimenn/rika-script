using System;
using RikaScript.Exception;

namespace RikaScript.Libs
{
    public class RandomLib : ScriptLibBase

    {
        public RandomLib()
        {
            this.LibName = "random";
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

        protected override void help()
        {
            Runtime.Logger.Print(message:
                "RandomLib:random - RikaScript 随机数生成类库\n"+
                "\t random() - 获取一个随机小数\n" +
                "\t range(start,end) - 获取一个范围内的随机小数\n"
                );
        }
    }
}