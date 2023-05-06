using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalArray : EvalObject
    {
        public ObjectType Type => ObjectType.Array;
        public List<EvalObject> Array;
        public EvalArray(List<EvalObject> array)
        {
            Array = array;
        }

        public string Inspect()
        {
            string ret = "[";
            for(int i = 0; i < Array.Count; i++)
            {
                ret += Array[i].Inspect() + ",";
            }
            ret += "]";
            return ret;
        }


        public override bool Equals(object other)
        {
            if (other is EvalArray evalArray)
            {
                evalArray.Array.SequenceEqual(Array);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Array.GetHashCode();
        }
    }
}
