using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    /// <summary>
    /// 変数束縛を表す。Tokenで変数を表し、それに対応する値をstringで表す。
    /// </summary>
    public class Identifier : Expression
    {
        public Token Token;
        public string Value;

        public Node expressionNode()
        {
            throw new NotImplementedException();
        }

        public string TokenLiteral()
        {
            return Token.Literal;
        }

        public Identifier(Token token, string value)
        {
            Token = token;
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
