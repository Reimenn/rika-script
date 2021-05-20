using System;
using System.Collections.Generic;
using System.Runtime.Remoting;
using RikaScript.Exception;

namespace RikaScript.Libs
{
    /// <summary>
    /// RikaScript 标准库
    /// </summary>
    public class StandardLib : ScriptLibBase
    {
        public StandardLib()
        {
            LibName = "std";
        }

        /// <summary>
        /// 标准输出
        /// </summary>
        public void log(object message)
        {
            Runtime.Logger.Info(message);
        }

        public void log(object m1, object m2)
        {
            Runtime.Logger.Info(m1.String() + m2.String());
        }

        public void log(object m1, object m2, object m3)
        {
            Runtime.Logger.Info(m1.String() + m2.String() + m3.String());
        }

        public void log(object m1, object m2, object m3, object m4)
        {
            Runtime.Logger.Info(m1.String() + m2.String() + m3.String() + m4.String());
        }

        /// <summary>
        /// 制造数据
        /// </summary>
        public object make(object val)
        {
            switch (val.ToString())
            {
                case "false":
                    return false;
                case "true":
                    return true;
                default:
                    return make(val, "double");
            }
        }

        public object make(object val, object type)
        {
            switch (type.ToString())
            {
                case "int":
                    return int.Parse(val.ToString());
                case "float":
                    return float.Parse(val.ToString());
                case "long":
                    return long.Parse(val.ToString());
                case "double":
                    return double.Parse(val.ToString());
                case "string":
                    return val.ToString();
                default:
                    return make(val);
            }
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        public object type(object val)
        {
            return val.GetType();
        }

        /// <summary>
        /// 根据 变量名 删除一个变量，释放内存
        /// </summary>
        public void del(object val)
        {
            var name = val.String();
            Runtime.DelValue(name);
        }

        /// <summary>
        /// 导入类库
        /// </summary>
        public void import(object name)
        {
            import(name, null);
        }

        public void import(object name, object asName)
        {
            var type = Type.GetType(name.String());
            if (type == null)
                throw new RuntimeException("不能导入类库，因为没有找到：" + name);
            var con = type.GetConstructor(new Type[0]);
            if (con == null)
                throw new RuntimeException("不能导入类库，因为没有空构造方法：" + name);
            var obj = con.Invoke(new object[0]);
            if (!(obj is ScriptLibBase))
                throw new RuntimeException("不能导入类库，因为这个类不是 RikaScript 的类库：" + name);

            string asn = null;
            if (asName.String().Length > 0)
                asn = asName.String();

            Runtime.AddLib((ScriptLibBase) obj, asn);
        }

        /// <summary>
        /// 设置默认类库
        /// </summary>
        public void set_default_lib(object name)
        {
            Runtime.SetDefaultLib(name.String());
        }

        /// <summary>
        /// 显示全部类库
        /// </summary>
        public void show_libs()
        {
            var strings = Runtime.GetLibs();
            var j = string.Join(", ", strings);
            Runtime.Logger.Info(j);
        }

        /// <summary>
        /// 显示全部变量
        /// </summary>
        public void show_vars()
        {
            var strings = Runtime.GetVars();
            var j = string.Join(", ", strings);
            Runtime.Logger.Info(j);
        }

        // Math

        public object add(object a, object b)
        {
            return a.Double() + b.Double();
        }

        public object add(object a, object b, object c)
        {
            return a.Double() + b.Double() + c.Double();
        }

        public object add(object a, object b, object c, object d)
        {
            return a.Double() + b.Double() + c.Double() + d.Double();
        }

        public object sub(object a, object b)
        {
            return a.Double() - b.Double();
        }

        public object mul(object a, object b)
        {
            return a.Double() * b.Double();
        }

        public object div(object a, object b)
        {
            return a.Double() / b.Double();
        }

        public object mod(object a, object b)
        {
            return a.Double() % b.Double();
        }

        public object pow(object a, object b)
        {
            return Math.Pow(a.Double(), b.Double());
        }

        // Bool

        public object not(object a)
        {
            if (a is bool ab)
                return !bool.Parse(ab.ToString());
            else
                return !(a.Double() > 0);
        }

        public object and(object a, object b)
        {
            return a.Bool() && b.Bool();
        }

        public object or(object a, object b)
        {
            return a.Bool() || b.Bool();
        }

        // object

        public object equals(object a, object b)
        {
            return a.Equals(b);
        }

        protected override bool OtherCall(string name, object[] args, out object res)
        {
            switch (name)
            {
                case "int":
                    res = (int) (args[0].Double());
                    return true;
                case "<":
                    res = args[0].Double() < args[1].Double();
                    return true;
                case "<=":
                    res = args[0].Double() <= args[1].Double();
                    return true;
                case ">":
                    res = args[0].Double() > args[1].Double();
                    return true;
                case ">=":
                    res = args[0].Double() >= args[1].Double();
                    return true;
                case "=":
                    res = args[0].Double() == args[1].Double();
                    return true;
                case "!=":
                    res = args[0].Double() != args[1].Double();
                    return true;
                default:
                    throw new NotFoundMethodException(name);
                    break;
            }
        }

        protected override void help()
        {
            Runtime.Logger.Print(message:
                "StandardLib:std - RikaScript 标准库，包含运行必备的基本方法\n" +
                "\t log(message*) - 输出一段 info，支持1到4个参数\n" +
                "\t make(value, type*) - 根据 value 返回一个数据，可选通过 type 指定类型\n" +
                "\t type(value) - 返回 value 的数据类型\n" +
                "\t del(var) - 删除一个变量\n" +
                "\t import(libName,asName*) - 导入类库，可选通过 asName 起别名\n" +
                "\t set_default_lib(libName) - 设置默认类库\n" +
                "\t show_libs() - 显示全部类库\n" +
                "\t show_vars() - 显示全部变量\n" +
                "\t add|sub|mul|div|mod|pow - 对两个数进行数学计算，其中 add 最多支持 4 个参数\n" +
                "\t not|and|or - bool运算\n" +
                "\t equals(obj1,obj2) - Equals判断\n" +
                "\t > >= < <= = 二元关系运算\n" +
                "\t int(num) - 将小数转换成整数"
            );
        }
    }
}