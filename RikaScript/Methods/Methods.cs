using System;
using System.Diagnostics;
using System.Reflection;

namespace RikaScript.Methods
{
    public class MethodAction0 : IMethod
    {
        private readonly Action _action;

        public MethodAction0(object target, MethodInfo method)
        {
            _action = (Action) Delegate.CreateDelegate(typeof(Action), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            _action.Invoke();
            res = null;
            return false;
        }
    }

    public class MethodAction1 : IMethod
    {
        private readonly Action<object> _action;

        public MethodAction1(object target, MethodInfo method)
        {
            _action = (Action<object>) Delegate.CreateDelegate(typeof(Action<object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            _action.Invoke(args[0]);
            res = null;
            return false;
        }
    }

    public class MethodAction2 : IMethod
    {
        private readonly Action<object, object> _action;

        public MethodAction2(object target, MethodInfo method)
        {
            _action = (Action<object, object>) Delegate.CreateDelegate(typeof(Action<object, object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            _action.Invoke(args[0], args[1]);
            res = null;
            return false;
        }
    }

    public class MethodAction3 : IMethod
    {
        private readonly Action<object, object, object> _action;

        public MethodAction3(object target, MethodInfo method)
        {
            _action = (Action<object, object, object>) Delegate.CreateDelegate(typeof(Action<object, object, object>),
                target, method);
        }

        public bool Call(object[] args, out object res)
        {
            _action.Invoke(args[0], args[1], args[2]);
            res = null;
            return false;
        }
    }

    public class MethodAction4 : IMethod
    {
        private readonly Action<object, object, object, object> _action;

        public MethodAction4(object target, MethodInfo method)
        {
            _action = (Action<object, object, object, object>) Delegate.CreateDelegate(
                typeof(Action<object, object, object, object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            _action.Invoke(args[0], args[1], args[2], args[3]);
            res = null;
            return false;
        }
    }

    public class MethodFunc0 : IMethod
    {
        private readonly Func<object> _func;

        public MethodFunc0(object target, MethodInfo method)
        {
            _func = (Func<object>) Delegate.CreateDelegate(typeof(Func<object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            res = _func.Invoke();
            return true;
        }
    }

    public class MethodFunc1 : IMethod
    {
        private readonly Func<object, object> _func;

        public MethodFunc1(object target, MethodInfo method)
        {
            _func = (Func<object, object>) Delegate.CreateDelegate(typeof(Func<object, object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            res = _func.Invoke(args[0]);
            return true;
        }
    }

    public class MethodFunc2 : IMethod
    {
        private readonly Func<object, object, object> _func;

        public MethodFunc2(object target, MethodInfo method)
        {
            _func = (Func<object, object, object>) Delegate.CreateDelegate(typeof(Func<object, object, object>), target,
                method);
        }

        public bool Call(object[] args, out object res)
        {
            res = _func.Invoke(args[0], args[1]);
            return true;
        }
    }

    public class MethodFunc3 : IMethod
    {
        private readonly Func<object, object, object, object> _func;

        public MethodFunc3(object target, MethodInfo method)
        {
            _func = (Func<object, object, object, object>) Delegate.CreateDelegate(
                typeof(Func<object, object, object, object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            res = _func.Invoke(args[0], args[1], args[2]);
            return true;
        }
    }

    public class MethodFunc4 : IMethod
    {
        private readonly Func<object, object, object, object, object> _func;

        public MethodFunc4(object target, MethodInfo method)
        {
            _func = (Func<object, object, object, object, object>) Delegate.CreateDelegate(
                typeof(Func<object, object, object, object, object>), target, method);
        }

        public bool Call(object[] args, out object res)
        {
            res = _func.Invoke(args[0], args[1], args[2], args[3]);
            return true;
        }
    }
}