using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class StringLiteral : Expression
    {
        public Token Token;
        public string Value;

        public StringLiteral(Token token, string value)
        {
            Token = token;
            Value = value;
        }

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }
    }
}
