using System.Collections.Generic;
using System.Linq;
using System.Text;
using RikaScript.Exception;

namespace RikaScript.Libs
{
    /// <summary>
    /// 类库信息类
    /// </summary>
    public class LibInfo
    {
        /// <summary>
        /// 逐条显示的帮助列表
        /// </summary>
        private readonly Dictionary<string, string> Infos = new Dictionary<string, string>();

        /// <summary>
        /// 写在帮助列表开头或结尾的内容
        /// </summary>
        private string Preface, Ending;

        /// <summary>
        /// 目标类库
        /// </summary>
        private readonly ScriptLibBase _lib;

        public LibInfo(ScriptLibBase lib, string preface = "", string ending = "")
        {
            this._lib = lib;
            Preface = preface;
            Ending = ending;
        }

        /// <summary>
        /// 添加一个帮助，应该在类库的构造方法内调用
        /// </summary>
        public LibInfo AddHelp(string name, string info)
        {
            Infos.Add(name, info);
            return this;
        }

        /// <summary>
        /// 设置信息全文的前言
        /// </summary>
        public LibInfo SetPreface(string preface)
        {
            Preface = preface;
            return this;
        }

        /// <summary>
        /// 设置信息全文的结束语
        /// </summary>
        public LibInfo SetEnding(string ending)
        {
            Ending = ending;
            return this;
        }

        /// <summary>
        /// 搜索帮助并返回
        /// </summary>
        public string SearchInfo(string str)
        {
            var sb = new StringBuilder();
            foreach (var infosKey in Infos.Keys)
            {
                var info = Infos[infosKey];
                if (infosKey.Contains(str) || info.Contains(str))
                    sb.Append(infosKey + "\n\t" + info);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 根据 key 获取唯一的一条 info
        /// </summary>
        public string GetInfo(string name)
        {
            if (Infos.ContainsKey(name))
                return name + "\n\t" + Infos[name] + "\n";
            throw new NotFoundInfoException(name);
        }

        /// <summary>
        /// 转换成标准的帮助文档 string
        /// </summary>
        public override string ToString()
        {
            var sb = new StringBuilder(_lib.GetType().FullName + ":" + _lib.LibName + "\n");
            sb.Append(_lib.Version + "\n");
            if (!string.IsNullOrEmpty(Preface)) sb.Append(Preface + "\n");
            foreach (var infosKey in Infos.Keys)
            {
                sb.Append("\t" + infosKey + "\n\t\t" + Infos[infosKey] + "\n");
            }

            if (!string.IsNullOrEmpty(Ending)) sb.Append(Ending + "\n");
            return sb.ToString();
        }
    }
}