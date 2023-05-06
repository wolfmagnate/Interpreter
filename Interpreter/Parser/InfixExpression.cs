using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class InfixExpression : Expression
    {
        public Token Token;
        public Expression Left;
        public Expression Right;
        public string Operator;

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public InfixExpression(Token token, Expression left, Expression right, string @operator)
        {
            Token = token;
            Left = left;
            Right = right;
            Operator = @operator;
        }
        public override string ToString()
        {
            return $"({Left} {Operator} {Right})";
        }
    }
}
