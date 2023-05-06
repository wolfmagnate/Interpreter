using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class HashLiteral : Expression
    {
        public Token Token;
        public Dictionary<Expression, Expression> KeyValuePairs;

        public HashLiteral(Token token, Dictionary<Expression, Expression> keyValuePairs)
        {
            Token = token;
            KeyValuePairs = keyValuePairs;
        }

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            List<string> keyValuePairsStrings = new List<string>();

            foreach (var keyValuePair in KeyValuePairs)
            {
                keyValuePairsStrings.Add($"{keyValuePair.Key}: {keyValuePair.Value}");
            }

            sb.Append("{");
            sb.Append(string.Join(", ", keyValuePairsStrings));
            sb.Append("}");

            return sb.ToString();
        }
    }
}
