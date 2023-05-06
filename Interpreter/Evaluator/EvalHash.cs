using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class EvalHash : EvalObject
    {
        public ObjectType Type => ObjectType.Hash;
        public Dictionary<EvalObject, EvalObject> KeyValuePairs;
        public EvalHash(Dictionary<EvalObject, EvalObject> keyValuePairs)
        {
            KeyValuePairs = keyValuePairs;
        }

        public string Inspect()
        {
            StringBuilder sb = new StringBuilder();
            List<string> keyValuePairsStrings = new List<string>();

            foreach (var keyValuePair in KeyValuePairs)
            {
                keyValuePairsStrings.Add($"{keyValuePair.Key.Inspect()}: {keyValuePair.Value.Inspect()}");
            }

            sb.Append("{");
            sb.Append(string.Join(", ", keyValuePairsStrings));
            sb.Append("}");

            return sb.ToString();
        }

        public override int GetHashCode()
        {
            return KeyValuePairs.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            if (obj is EvalHash evalHash)
            {
                return evalHash.KeyValuePairs.SequenceEqual(KeyValuePairs);
            }
            return false;
        }
    }
}
