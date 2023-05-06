using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalReturnValue : EvalObject
    {
        public ObjectType Type => ObjectType.Return;
        public EvalObject ReturnValue;

        public EvalReturnValue(EvalObject returnValue)
        {
            ReturnValue = returnValue;
        }

        public string Inspect()
        {
            return $"return {ReturnValue}";
        }

        // 内部で便宜的に生成される値なのでハッシュで使われることはない
    }
}
