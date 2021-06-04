using System;

namespace RikaScript.Libs.Base
{
    /// <summary>
    /// RikaScript 函数
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class Method : Attribute
    {
        /// <summary>
        /// 别名，在RikaScript中调用要使用这个名字，默认不填则用C#中的函数名代替
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 优先级，越高越优先，+=%是10，*/是100，乘方是110，关系比较是5，等于和不等与是3
        /// </summary>
        public int Priority = 0;
        /// <summary>
        /// 帮助信息，写点介绍就行
        /// </summary>
        public string Help = "";
        /// <summary>
        /// 是否持久，不被覆盖
        /// </summary>
        public bool Keep = false;
    }

    /// <summary>
    /// RikaScript Library
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class Library : Attribute
    {
        /// <summary>
        /// 函数名字
        /// </summary>
        public string Name = "";
        /// <summary>
        /// 类库描述、帮助
        /// </summary>
        public string Help = "";
        /// <summary>
        /// 类库版本
        /// </summary>
        public string Version = "";
    }
}