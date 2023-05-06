using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalString : EvalObject
    {
        public ObjectType Type => ObjectType.String;
        public string Value;

        public EvalString(string value)
        {
            Value = value;
        }

        public string Inspect()
        {
            return Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EvalString evalString)
            {
                return evalString.Value == Value;
            }
            return false;
        }
    }
}
