using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    internal class EvalBuiltIn<Tout> : EvalObject
    {
        public ObjectType Type => throw new NotImplementedException();
        public Func<List<object>, Tout> Func;

        public EvalBuiltIn(Func<List<object>, Tout> func)
        {
            Func = func;
        }

        public string Inspect()
        {
            return "built in function";
        }
    }
}
