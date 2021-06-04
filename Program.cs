using System;
using RikaScript;
using RikaScript.Logger;


namespace RikaScriptDev
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            var eng = new Engine(new ConsoleLogger());

            Console.WriteLine("RikaScript 尝鲜程序 v0.6，输入 help 查看帮助");
            while (true)
            {
                Console.Write("RS > ");
                eng.Execute(Console.ReadLine());
            }
        }
    }
}