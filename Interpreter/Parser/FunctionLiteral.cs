using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class FunctionLiteral : Expression
    {
        public Token Token;
        public List<Identifier> Parameters;
        public BlockStatement Body;

        public FunctionLiteral(Token token, List<Identifier> parameters, BlockStatement body)
        {
            Token = token;
            Parameters = parameters;
            Body = body;
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
            builder.Append(TokenLiteral());
            builder.Append("(");

            var paramStrings = Parameters.Select(param => param.ToString()).ToArray();
            builder.Append(string.Join(", ", paramStrings));

            builder.Append(")");
            builder.Append(Body.ToString());

            return builder.ToString();
        }
    }
}
