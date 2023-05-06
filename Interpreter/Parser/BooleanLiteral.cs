using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class BooleanLiteral : Expression
    {
        public Token Token;
        public bool Value;

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return $"{Value}";
        }

        public override string ToString()
        {
            return $"{(Value ? "true" : "false")}";
        }

        public BooleanLiteral(Token token, bool value)
        {
            Token = token;
            Value = value;
        }
    }
}
