using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public enum ObjectType
    {
        Boolean, Integer, Null, Return, Function,
        String, BuiltIn,
        Array,
        Hash
    }

    public interface EvalObject
    {
        ObjectType Type { get; }
        string Inspect();
        int GetHashCode();
        bool Equals(object other);
    }
}
