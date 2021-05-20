using System.Reflection;
using RikaScript.Exception;

namespace RikaScript.Methods
{
    /// <summary>
    /// 生产 IMethod 的简单工厂
    /// </summary>
    public class MethodFactory
    {
        public static IMethod Create(object target, MethodInfo m)
        {
            if (m.ReturnType == typeof(void))
            {
                switch (m.GetParameters().Length)
                {
                    case 0:
                        return new MethodAction0(target, m);
                    case 1:
                        return new MethodAction1(target, m);
                    case 2:
                        return new MethodAction2(target, m);
                    case 3:
                        return new MethodAction3(target, m);
                    case 4:
                        return new MethodAction4(target, m);
                }
            }
            else
            {
                switch (m.GetParameters().Length)
                {
                    case 0:
                        return new MethodFunc0(target, m);
                    case 1:
                        return new MethodFunc1(target, m);
                    case 2:
                        return new MethodFunc2(target, m);
                    case 3:
                        return new MethodFunc3(target, m);
                    case 4:
                        return new MethodFunc4(target, m);
                }
            }

            throw new MethodParseException(m.Name);
        }
    }
}