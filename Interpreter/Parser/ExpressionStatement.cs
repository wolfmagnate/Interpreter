using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class ExpressionStatement : Statement
    {
        public Token Token;
        public Expression Expression;

        public ExpressionStatement(Token token, Expression expression)
        {
            Token = token;
            Expression = expression;
        }

        public Node statementNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public override string ToString()
        {
            return $"{Expression}";
        }
    }
}
