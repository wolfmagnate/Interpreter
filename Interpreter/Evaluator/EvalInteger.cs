using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalInteger : EvalObject
    {
        public int Value;
        public string Inspect()
        {
            return $"{Value}";
        }
        public ObjectType Type => ObjectType.Integer;

        public EvalInteger(int value)
        {
            Value = value;
        }

        public override int GetHashCode()
        {
            return Value;
        }

        public override bool Equals(object? obj)
        {
            if (obj is EvalInteger evalInteger)
            {
                return evalInteger.Value == Value;
            }
            return false;
        }
    }
}
