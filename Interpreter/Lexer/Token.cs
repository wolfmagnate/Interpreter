using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Lexer
{
    public class Token
    {
        public TokenType Type;
        public string Literal;

        public Token(TokenType t, string l)
        {
            Type = t;
            Literal = l;
        }
    }

    public enum TokenType
    {
        // 不正なトークン
        ILLEGAL,
        // ファイルの終端
        EOF,
        // 識別子
        IDENTIFIER,
        // 整数
        INT,
        // 文字列
        STRING,
        // 代入演算子(=)
        ASSIGN,
        // 演算子(+,-,*,/,<,>,!)
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        GREATER, // >
        LESS, // <
        BANG, // !

        // 2文字の演算子
        EQUAL,
        NOT_EQUAL,

        // デリミタ
        COMMA,
        SEMICOLON,
        COLON,
        // 記号
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACE,
        RIGHT_BRACE,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        // キーワード
        FUNCTION,
        LET,
        TRUE,
        FALSE,
        IF,
        ELSE,
        RETURN,
    }
}
