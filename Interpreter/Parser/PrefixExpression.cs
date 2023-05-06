using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class PrefixExpression : Expression
    {
        public Token Token;
        public string Operator;
        public Expression Right;

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
            return $"({Operator}{Right})";
        }

        public PrefixExpression(Token token, string @operator, Expression right)
        {
            Token = token;
            Operator = @operator;
            Right = right;
        }
    }
}
