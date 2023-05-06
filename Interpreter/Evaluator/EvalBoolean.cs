using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalBoolean : EvalObject
    {
        public bool Value;
        public string Inspect()
        {
            return $"{(Value ? "true" : "false")}";
        }

        public ObjectType Type => ObjectType.Boolean;
        public EvalBoolean(bool value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EvalBoolean evalBoolean)
            {
                return evalBoolean.Value == Value;
            }
            return false;
        }
    }
}
