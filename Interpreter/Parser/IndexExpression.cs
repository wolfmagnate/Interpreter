using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class IndexExpression : Expression
    {
        public Token Token;
        // 配列・オブジェクトリテラルかそれらを示すIdentifier
        public Expression Left;
        // インデックスの中身を示す式
        public Expression Index;

        public IndexExpression(Token token, Expression array, Expression index)
        {
            Token = token;
            Left = array;
            Index = index;
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
