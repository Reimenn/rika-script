using System.Collections.Generic;
using RikaScript.Exception;
using RikaScript.Libs.Base;

namespace RikaScript.Libs
{
    [Library(Name = "bool",Version = "v0.1.0")]
    public class BoolLib : ScriptLibBase
    {
        [Method(Help = "计算一串bool公式")]
        public object calc(object value)
        {
            var expression = value.String();
            Stack<char> values = new Stack<char>();
            Stack<char> ops = new Stack<char>();
            foreach (var charCurrent in expression)
            {
                switch (charCurrent)
                {
                    case ')':
                        var op = ops.Pop();
                        var val = values.Pop();
                        var pop = values.Pop();
                        if (op == '!')
                        {
                            val = val == 't' ? 'f' : 't';
                        }

                        while (pop != '(')
                        {
                            switch (op)
                            {
                                case '&':
                                    val = val == 't' && pop == 't' ? 't' : 'f';
                                    break;
                                case '|':
                                    val = val == 't' || pop == 't' ? 't' : 'f';
                                    break;
                            }

                            pop = values.Pop();
                        }

                        values.Push(val);
                        break;
                    case '|':
                    case '&':
                    case '!':
                        ops.Push(charCurrent);
                        break;
                    case '(':
                    case 'f':
                    case 't':
                        values.Push(charCurrent);
                        break;
                }
            }

            return values.Pop() == 't';
        }
    }
}