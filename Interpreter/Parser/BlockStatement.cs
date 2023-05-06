using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class BlockStatement : Statement
    {
        public Token Token;
        public List<Statement> Statements;

        public Node statementNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public BlockStatement(Token token, List<Statement> statements)
        {
            Token = token;
            Statements = statements;
        }

        public override string ToString()
        {
            string result = "{ ";

            foreach (var statement in Statements)
            {
                result += statement.ToString();
            }

            result += " }";
            return result;
        }
    }
}
