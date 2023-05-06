using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class IntegerLiteral : Expression
    {
        public Token Token;
        public int Value;

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public IntegerLiteral(Token token, int value)
        {
            Token = token;
            this.Value = value;
        }

        public override string ToString()
        {
            return $"{Value}";
        }
    }
}
