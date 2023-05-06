using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalNull : EvalObject
    {
        public ObjectType Type => ObjectType.Null;

        public string Inspect()
        {
            return "null";
        }

        public override bool Equals(object? obj)
        {
            if(obj is EvalNull evalNull)
            {
                return true;
            }
            return false;
        }
    }
}
