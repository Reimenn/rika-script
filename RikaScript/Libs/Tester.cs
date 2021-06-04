using System;
using RikaScript.Libs.Base;

namespace RikaScript.Libs
{
    [Library("test", "v0.0.0.0.0.0.1")]
    public class Tester : ScriptLibBase
    {
        private string str = "";
        [Method]
        public void set(object val)
        {
            str = val.String();
        }

        [Method]
        public void show()
        {
            Console.WriteLine(str);
        }
    }
}