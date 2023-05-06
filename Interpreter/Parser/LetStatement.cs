using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class LetStatement : Statement
    {
        public Token Token;
        public Identifier Name;
        public Expression Value;

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public Node statementNode()
        {
            throw new NotImplementedException();
        }

        public LetStatement(Token token)
        {
            Token = token;
            Name = new Identifier(new Token(TokenType.ILLEGAL, ""), "初期化されていません");
        }

        public override string ToString()
        {
            return $"{TokenLiteral()} {Name} = {Value};";
        }
    }
}
