using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class Environment
    {
        public Dictionary<string, EvalObject> Store { get; }

        public Environment? Outer;

        public Environment()
        {
            Store = new Dictionary<string, EvalObject>();
        }

        public EvalObject Get(string key)
        {
            if (Store.ContainsKey(key))
            {
                return Store[key];
            }else if (Outer != null)
            {
                return Outer.Get(key);
            }
            return null;
        }

    }
}
