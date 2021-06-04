using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using RikaScript.Exception;
using RikaScript.Libs.Base;

namespace RikaScript.Libs
{
    /// <summary>
    /// RikaScript 标准库
    /// </summary>
    [Library(Name = "std", Version = "v0.2.0", Help = "RikaScript 标准类库，内部的函数都不能被覆盖")]
    public class StandardLib : ScriptLibBase
    {
        /// <summary>
        /// 标准输出
        /// </summary>
        [Method(Keep = true, Help = "以 INFO 的形式输出一段消息")]
        public void log(object message)
        {
            Runtime.Logger.Info(message);
        }

        [Method(Keep = true, Help = "以 INFO 的形式输出一段消息，多个参数之间直接相连")]
        public void log(object m1, object m2)
        {
            Runtime.Logger.Info(m1.String() + m2.String());
        }

        [Method(Keep = true, Help = "以 INFO 的形式输出一段消息，多个参数之间直接相连")]
        public void log(object m1, object m2, object m3)
        {
            Runtime.Logger.Info(m1.String() + m2.String() + m3.String());
        }

        [Method(Keep = true, Help = "以 INFO 的形式输出一段消息，多个参数之间直接相连")]
        public void log(object m1, object m2, object m3, object m4)
        {
            Runtime.Logger.Info(m1.String() + m2.String() + m3.String() + m4.String());
        }

        /// <summary>
        /// 制造数据
        /// </summary>
        [Method(Keep = true, Help = "进来什么数据，出去就还是什么数据，这里其实就写了一行return")]
        public object make(object value)
        {
            return value;
        }

        [Method(Keep = true, Help = "字符串转换成数值，第一个参数是想要的数据，第二个参数是类型，支持 int long float double，类型不正确返回原有值")]
        public object make(object value, object type)
        {
            switch (type.String())
            {
                case "int":
                    return int.Parse(value.String());
                case "float":
                    return float.Parse(value.String());
                case "long":
                    return long.Parse(value.String());
                case "double":
                    return double.Parse(value.String());
                default:
                    return make(value);
            }
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        [Method(Keep = true, Help = "获取数据类型，内部：return val.GetType()")]
        public object type(object obj)
        {
            return obj.GetType();
        }

        [Method(Keep = true, Help = "用字符串指定变量，给变量赋值，不存在变量则创建")]
        public void set(object varName, object value)
        {
            Runtime.SetValue(varName.String(), value);
        }

        [Method(Keep = true, Help = "用字符串指定变量，返回某个变量的值，不存在返回默认值")]
        public object get(object varName, object defaultValue)
        {
            try
            {
                return Runtime.GetValue<object>(varName.String());
            }
            catch
            {
                return defaultValue;
            }
        }

        [Method(Keep = true, Help = "用字符串指定变量，返回某个变量的值，不存在返回null")]
        public object get(object varName)
        {
            return get(varName, null);
        }

        /// <summary>
        /// 根据 变量名 删除一个变量，释放内存
        /// </summary>
        [Method(Keep = true, Help = "用字符串指定变量，删除一个变量")]
        public void del(object varName)
        {
            var name = varName.String();
            Runtime.DelValue(name);
        }

        /// <summary>
        /// 返回是否存在，bool 类型
        /// </summary>
        [Method(Keep = true, Help = "用字符串指定变量，返回是否存在某个变量")]
        public object exist(object varName)
        {
            return Runtime.ExistValue(varName.String());
        }

        /// <summary>
        /// 导入类库
        /// </summary>
        [Method(Keep = true, Help = "导入类库，填写类库全名（带上 namespace）")]
        public void import(object libraryFullName)
        {
            import(libraryFullName, null);
        }

        [Method(Keep = true, Help = "导入类库，填写类库全名（带上 namespace），可以用第二个参数给类库起个别名")]
        public void import(object libraryFullName, object asName)
        {
            // 存在检查
            var libraryClassType = Type.GetType(libraryFullName.String());
            if (libraryClassType == null)
                throw new ImportLibraryException("没有找到：" + libraryFullName);
            // 构造器检查
            var constructor = libraryClassType.GetConstructor(new Type[0]);
            if (constructor == null)
                throw new ImportLibraryException(libraryFullName + "类库没有空构造方法");
            // 父类检查
            if (libraryClassType.BaseType != typeof(ScriptLibBase))
                throw new ImportLibraryException(libraryFullName + "类不是 RikaScript 类库");

            // 实例化、添加类库
            var obj = constructor.Invoke(new object[0]);

            if (string.IsNullOrEmpty(asName.String()))
                Runtime.AddLib((ScriptLibBase) obj);
            else
                Runtime.AddLib((ScriptLibBase) obj, asName.String());
        }

        [Method(Keep = true, Help = "判断是否存在某个类库，以别名为依据")]
        public object exist_lib(object libraryAlias)
        {
            return Runtime.GetLibs().Contains(libraryAlias.String());
        }

        /// <summary>
        /// 显示全部类库
        /// </summary>
        [Method(Keep = true, Help = "显示一下全部已经导入的类库")]
        public void show_libs()
        {
            var strings = Runtime.GetLibs();
            var j = string.Join(", ", strings);
            Runtime.Logger.Info(j);
        }

        /// <summary>
        /// 显示全部变量
        /// </summary>
        [Method(Keep = true, Help = "显示一下全部变量")]
        public void show_vars()
        {
            var strings = Runtime.GetVars();
            var j = string.Join(", ", strings);
            Runtime.Logger.Info(j);
        }

        [Method(Name = "int", Keep = true, Help = "将小数转换成整数")]
        public object Int(object number)
        {
            return (int) number.Double();
        }

        [Method(Name = "float", Keep = true, Help = "将整数转换成小数")]
        public object Float(object number)
        {
            return number.Double();
        }

        [Method(Name = "str", Keep = true, Help = "转换成字符串")]
        public object Str(object value)
        {
            return value.String();
        }

        [Method(Name = "hash", Keep = true, Help = "获取 hash 码")]
        public object hash(object value)
        {
            return value.GetHashCode();
        }

        // 数学
        [Method(Name = "+", Priority = 10, Keep = true)]
        public object add(object a, object b)
        {
            if (ScriptTools.AnyIsDecimal(a, b))
                return a.Double() + b.Double();
            return a.Long() + b.Long();
        }

        [Method(Name = "-", Priority = 10, Keep = true)]
        public object sub(object a, object b)
        {
            if (ScriptTools.AnyIsDecimal(a, b))
                return a.Double() - b.Double();
            return a.Long() - b.Long();
        }

        [Method(Name = "*", Priority = 100, Keep = true)]
        public object mul(object a, object b)
        {
            if (ScriptTools.AnyIsDecimal(a, b))
                return a.Double() * b.Double();
            return a.Long() * b.Long();
        }

        [Method(Name = "/", Priority = 100, Keep = true)]
        public object div(object a, object b)
        {
            if (ScriptTools.AnyIsDecimal(a, b))
                return a.Double() / b.Double();
            return a.Long() / b.Long();
        }

        [Method(Name = "%", Priority = 10, Keep = true, Help = "求余")]
        public object mod(object a, object b)
        {
            if (ScriptTools.AnyIsDecimal(a, b))
            {
                if (a.Double() == 0) return 0.0;
                return a.Double() % b.Double();
            }

            if (a.Long() == 0) return 0L;
            return a.Long() % a.Long();
        }

        [Method(Name = "**", Priority = 110, Keep = true, Help = "乘方操作，左值是底数，右值为指数")]
        public object pow(object a, object b)
        {
            return Math.Pow(a.Double(), b.Double());
        }

        // 关系
        [Method(Name = ">", Priority = 5, Keep = true)]
        public object gt(object a, object b)
        {
            return a.Double() > b.Double();
        }

        [Method(Name = "<", Priority = 5, Keep = true)]
        public object lt(object a, object b)
        {
            return a.Double() < b.Double();
        }

        [Method(Name = ">=", Priority = 5, Keep = true)]
        public object ge(object a, object b)
        {
            return a.Double() >= b.Double();
        }

        [Method(Name = "<=", Priority = 5, Keep = true)]
        public object le(object a, object b)
        {
            return a.Double() <= b.Double();
        }

        [Method(Name = "==", Priority = 3, Keep = true)]
        public object eq(object a, object b)
        {
            return a.String() == b.String();
        }

        [Method(Name = "!=", Priority = 3, Keep = true)]
        public object ne(object a, object b)
        {
            return a.String() != b.String();
        }

        // Bool
        [Method(Keep = true)]
        public object not(object a)
        {
            return !a.Bool();
        }

        [Method(Priority = 2, Keep = true)]
        public object and(object a, object b)
        {
            return a.Bool() && b.Bool();
        }

        [Method(Priority = 1, Keep = true)]
        public object or(object a, object b)
        {
            return a.Bool() || b.Bool();
        }
    }
}