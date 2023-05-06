using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class ReturnStatement : Statement
    {
        public Token Token;
        public Expression ReturnValue;

        public Node statementNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public ReturnStatement(Token token)
        {
            this.Token = token;
        }

        public override string ToString()
        {
            return $"{TokenLiteral()} {ReturnValue};";
        }
    }
}
