using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class IfExpression : Expression
    {
        public Token Token;
        public Expression Condition;
        public BlockStatement Consequence;
        // else節は省略可能
        public BlockStatement? Alternative;

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public IfExpression(Token token, Expression condition, BlockStatement consequence, BlockStatement alternative)
        {
            Token = token;
            Condition = condition;
            Consequence = consequence;
            Alternative = alternative;
        }

        public override string ToString()
        {
            // TODO
            string result = "if ";
            result += Condition.ToString();
            result += " ";
            result += Consequence.ToString();

            if (Alternative != null)
            {
                result += " else ";
                result += Alternative.ToString();
            }

            return result;
        }
    }
}
