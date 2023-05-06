using Interpreter.Lexer;

namespace InterpretTest
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Token t;
        }

        [Test]
        public void Test1()
        {
            string input = @"=+(){},;";
            Token[] expected = {
                new(TokenType.ASSIGN, "="),
                new(TokenType.PLUS, "+"),
                new(TokenType.LEFT_PAREN, "("),
                new(TokenType.RIGHT_PAREN, ")"),
                new(TokenType.LEFT_BRACE, "{"),
                new(TokenType.RIGHT_BRACE, "}"),
                new(TokenType.COMMA, ","),
                new(TokenType.SEMICOLON, ";"),
                new(TokenType.EOF, "")
            };

            Lexer lexer = new Lexer(input);
            for(int i = 0;i < expected.Length; i++)
            {
                var t =lexer.NextToken();
                Assert.That(t.Type, Is.EqualTo(expected[i].Type));
                Assert.That(t.Literal, Is.EqualTo(expected[i].Literal));
            }

            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            string input = @"let five = 5;
let ten = 10;

let add = fn(x, y) {
    x + y;
};

let result = add(five, ten);";
            Token[] expected =
            {
                new (TokenType.LET, "let"),
                new (TokenType.IDENTIFIER, "five"),
                new (TokenType.ASSIGN, "="),
                new (TokenType.INT, "5"),
                new (TokenType.SEMICOLON, ";"),
                new (TokenType.LET, "let"),
                new (TokenType.IDENTIFIER, "ten"),
                new (TokenType.ASSIGN, "="),
                new (TokenType.INT, "10"),
                new (TokenType.SEMICOLON, ";"),
                new (TokenType.LET, "let"),
                new (TokenType.IDENTIFIER, "add"),
                new (TokenType.ASSIGN, "="),
                new (TokenType.FUNCTION, "fn"),
                new (TokenType.LEFT_PAREN, "("),
                new (TokenType.IDENTIFIER, "x"),
                new (TokenType.COMMA, ","),
                new (TokenType.IDENTIFIER, "y"),
                new (TokenType.RIGHT_PAREN, ")"),
                new (TokenType.LEFT_BRACE, "{"),
                new (TokenType.IDENTIFIER, "x"),
                new (TokenType.PLUS, "+"),
                new (TokenType.IDENTIFIER, "y"),
                new (TokenType.SEMICOLON, ";"),
                new (TokenType.RIGHT_BRACE, "}"),
                new (TokenType.SEMICOLON, ";"),
                new (TokenType.LET, "let"),
                new (TokenType.IDENTIFIER, "result"),
                new (TokenType.ASSIGN, "="),
                new (TokenType.IDENTIFIER, "add"),
                new (TokenType.LEFT_PAREN, "("),
                new (TokenType.IDENTIFIER, "five"),
                new (TokenType.COMMA, ","),
                new (TokenType.IDENTIFIER, "ten"),
                new (TokenType.RIGHT_PAREN, ")"),
                new (TokenType.SEMICOLON, ";"),
                new (TokenType.EOF, ""),
            };

            Lexer lexer = new Lexer(input);
            for (int i = 0; i < expected.Length; i++)
            {
                var t = lexer.NextToken();
                Assert.That(t.Type, Is.EqualTo(expected[i].Type));
                Assert.That(t.Literal, Is.EqualTo(expected[i].Literal));
            }

            Assert.Pass();
        }

        [Test]
        public void Test3()
        {
            string input = @"!-/*5;[]:
5 < 10 > 5;
! != = ==";
            input += "\"hello\"";
            Token[] expected = {
                new(TokenType.BANG, "!"),
                new(TokenType.MINUS, "-"),
                new(TokenType.SLASH, "/"),
                new(TokenType.ASTERISK, "*"),
                new(TokenType.INT, "5"),
                new(TokenType.SEMICOLON, ";"),
                new(TokenType.LEFT_BRACKET, "["),
                new(TokenType.RIGHT_BRACKET, "]"),
                new(TokenType.COLON, ":"),
                new(TokenType.INT, "5"),
                new(TokenType.LESS, "<"),
                new(TokenType.INT, "10"),
                new(TokenType.GREATER, ">"),
                new(TokenType.INT, "5"),
                new(TokenType.SEMICOLON, ";"),
                new(TokenType.BANG, "!"),
                new(TokenType.NOT_EQUAL, "!="),
                new(TokenType.ASSIGN, "="),
                new(TokenType.EQUAL, "=="),
                new(TokenType.STRING, "hello"),
                new(TokenType.EOF, "")
            };

            Lexer lexer = new Lexer(input);
            for (int i = 0; i < expected.Length; i++)
            {
                var t = lexer.NextToken();
                Assert.That(t.Type, Is.EqualTo(expected[i].Type));
                Assert.That(t.Literal, Is.EqualTo(expected[i].Literal));
            }

            Assert.Pass();
        }
    }
}