using System;

namespace RikaScript.Libs.Base
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Method : Attribute
    {
        public string Name = "";
        public int Priority = 0;
        public string Help = "";
        public bool Keep = false;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class Library : Attribute
    {
        public string Name = "";
        public string Help = "";
        public string Version = "";

        public Library(string name, string version, string help = "")
        {
            Name = name;
            Version = version;
            Help = help;
        }
    }
}