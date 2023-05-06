using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class ArrayLiteral : Expression
    {
        public Token Token;
        public List<Expression> Elements;
        public ArrayLiteral(Token token, List<Expression> elements)
        {
            Token = token;
            Elements = elements;
        }

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
            string ret = "[";
            foreach(var elm in Elements)
            {
                ret += $"elm.ToString(), ";
            }
            ret += "]";
            return ret;
        }
    }
}
