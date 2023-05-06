using Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalFunctionObject : EvalObject
    {
        public ObjectType Type => ObjectType.Function;
        public List<Identifier> Parameters;
        public Environment Environment;
        public BlockStatement Body;

        public EvalFunctionObject(List<Identifier> parameters, Environment environment, BlockStatement body)
        {
            Parameters = parameters;
            Environment = environment;
            Body = body;
        }

        public string Inspect()
        {
            string ret = $"fn(";
            foreach(var param in Parameters)
            {
                ret += param.Value;
            }
            ret += ")";
            ret += Body.ToString();
            return ret;
        }
    }
}
