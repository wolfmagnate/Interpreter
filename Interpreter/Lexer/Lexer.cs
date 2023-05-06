using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Interpreter.Lexer
{
    public class Lexer
    {
        string input;
        int position;
        int readPosition;
        char target;

        public Lexer(string input)
        {
            this.input = input;
            readPosition = 0;
            position = -1;
            readChar();
        }

        /// <summary>
        /// 次の1文字を呼んで現在位置を進める
        /// </summary>
        void readChar()
        {
            if (readPosition >= input.Length)
            {
                // null文字
                target = '\0';
            }
            else
            {
                target = input[readPosition];
            }
            position = readPosition;
            readPosition++;
        }

        public Token NextToken()
        {
            skipWhitespace();
            // 単発で分かるトークン
            Token t = target switch
            {
                '=' => new(TokenType.ASSIGN, $"{target}"),
                ';' => new(TokenType.SEMICOLON, $"{target}"),
                '(' => new(TokenType.LEFT_PAREN, $"{target}"),
                ')' => new(TokenType.RIGHT_PAREN, $"{target}"),
                ',' => new(TokenType.COMMA, $"{target}"),
                '+' => new(TokenType.PLUS, $"{target}"),
                '-' => new(TokenType.MINUS, $"{target}"),
                '*' => new(TokenType.ASTERISK, $"{target}"),
                '/' => new(TokenType.SLASH, $"{target}"),
                '!' => new(TokenType.BANG, $"{target}"),
                '<' => new(TokenType.LESS, $"{target}"),
                '>' => new(TokenType.GREATER, $"{target}"),
                '{' => new(TokenType.LEFT_BRACE, $"{target}"),
                '}' => new(TokenType.RIGHT_BRACE, $"{target}"),
                '[' => new(TokenType.LEFT_BRACKET, $"{target}"),
                ']' => new(TokenType.RIGHT_BRACKET, $"{target}"),
                ':' => new(TokenType.COLON, $"{target}"),
                '\0' => new(TokenType.EOF, ""),
                _ => new(TokenType.ILLEGAL, ""),
            };
            // 長く読まないといけないようなトークン
            if (t.Type == TokenType.ILLEGAL)
            {
                if (isLetter(target))
                {
                    return readIdentifier();
                }
                if (isDigit(target))
                {
                    return readNumber();
                }
                if (target == '"')
                {
                    return readString();
                }
            }
            // 2文字のトークンが存在する可能性がある場合、Longest Matchを取る必要がある
            if (t.Type == TokenType.BANG && peekChar() == '=')
            {
                readChar();
                t = new Token(TokenType.NOT_EQUAL, "!=");
            }
            if (t.Type == TokenType.ASSIGN && peekChar() == '=')
            {
                readChar();
                t = new Token(TokenType.EQUAL, "==");
            }
            readChar();
            return t;
        }

        static bool isLetter(char c)
        {
            return Regex.IsMatch($"{c}", "[a-zA-Z_]");
        }
        static bool isDigit(char c)
        {
            return Regex.IsMatch($"{c}", "[0-9]");
        }

        char peekChar()
        {
            if (readPosition >= input.Length)
            {
                return '\0';
            }
            else
            {
                return input[readPosition];
            }
        }

        /// <summary>
        /// targetをひたすら読み進めて、全部まとめる
        /// </summary>
        /// <returns></returns>
        Token readIdentifier()
        {
            int startPosition = position;
            while (isLetter(target))
            {
                readChar();
            }
            int endPosition = position;
            // endPositionは既に識別子では無くなっているような場所
            string id = input.Substring(startPosition, endPosition - startPosition);
            var type = lookUpKeyword(id);
            return new Token(type, id);
        }

        /// <summary>
        /// idがキーワードかどうか判定して適切なトークンを返す
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TokenType lookUpKeyword(string id)
        {
            return id switch
            {
                "fn" => TokenType.FUNCTION,
                "let" => TokenType.LET,
                "true" => TokenType.TRUE,
                "false" => TokenType.FALSE,
                "if" => TokenType.IF,
                "else" => TokenType.ELSE,
                "return" => TokenType.RETURN,
                _ => TokenType.IDENTIFIER
            };
        }

        void skipWhitespace()
        {
            while (target == ' ' || target == '\t' || target == '\n' || target == '\r')
            {
                readChar();
            }
        }

        Token readNumber()
        {
            int startPosition = position;
            while (isDigit(target))
            {
                readChar();
            }
            int endPosition = position;
            string number = input.Substring(startPosition, endPosition - startPosition);
            return new Token(TokenType.INT, number);
        }

        Token readString()
        {
            int startPosition = position + 1;
            readChar(); // 文字列の"を読み飛ばす
            while (target != '"')
            {
                readChar();
            }
            // 1文字だった場合、1回のreadCharでtargetが"になる
            int endPosition = position;
            string str = input.Substring(startPosition, endPosition - startPosition);
            readChar();
            return new Token(TokenType.STRING, str);
        }
    }
}
