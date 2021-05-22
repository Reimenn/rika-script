using System.IO;
using System.Text;
using RikaScript.Exception;

namespace RikaScript.Libs
{
    public class FileLib : ScriptLibBase
    {
        private Encoding _encoding = Encoding.UTF8;
        private string _newLine = "\n";

        public FileLib() : base("file", "v0.1.0")
        {
            Info.SetPreface("RikaScript 基本文件交互库")
                .AddHelp("read(path)", "读取文件内容并返回")
                .AddHelp("write(path, text)", "覆盖写入文件")
                .AddHelp("append(path, text)", "追加写入文件")
                .AddHelp("set_encoding(encoding)", "设定类库采用的文件编码，例如utf-8、gbk")
                .AddHelp("set_new_line(str)", "指定追加文件时的前置符号，默认是换行符");
        }

        public void set_encoding(object a)
        {
            this._encoding = System.Text.Encoding.GetEncoding(a.String());
        }

        public void set_new_line(object newline)
        {
            this._newLine = newline.String();
        }

        public object read(object path)
        {
            return File.ReadAllText(path.String(), _encoding);
        }

        public void write(object path, object text)
        {
            File.WriteAllText(path.String(), text.String(), _encoding);
        }

        public void append(object path, object text)
        {
            File.AppendAllText(path.String(), _newLine + text.String(), _encoding);
        }

        protected override bool OtherCall(string name, object[] args, out object res)
        {
            throw new NotFoundMethodException(name);
        }
    }
}