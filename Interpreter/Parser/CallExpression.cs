using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class CallExpression : Expression
    {
        public Token Token;
        public Expression Function; // IdentifierまたはFunctionLiteral
        public List<Expression> Arguments;

        public CallExpression(Token token, Expression function, List<Expression> arguments)
        {
            Token = token;
            Function = function;
            Arguments = arguments;
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
            var builder = new StringBuilder();
            builder.Append("(");
            builder.Append(Function.ToString());
            builder.Append(")");
            builder.Append("(");

            var argStrings = Arguments.Select(arg => arg.ToString()).ToArray();
            builder.Append(string.Join(", ", argStrings));

            builder.Append(")");

            return builder.ToString();
        }
    }
}
