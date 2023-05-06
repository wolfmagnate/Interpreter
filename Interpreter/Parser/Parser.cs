using Interpreter.Lexer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public class Parser
    {
        Lexer.Lexer lexer;
        Token curToken;
        Token peekToken;

        Dictionary<TokenType, Func<Expression>> prefixFunctions;
        Dictionary<TokenType, Func<Expression, Expression>> infixFunctions;
        Dictionary<TokenType, CalcPriority> precedences;
        enum CalcPriority
        {
            Lowest, Equals, LessGreater, Sum, Product, Prefix, Call
        }

        public Parser(Lexer.Lexer lexer)
        {
            this.lexer = lexer;
            nextToken();
            nextToken();

            precedences = new Dictionary<TokenType, CalcPriority>();
            // 中置演算子になるトークンとそれぞれの優先順位
            precedences[TokenType.EQUAL] = CalcPriority.Equals;
            precedences[TokenType.NOT_EQUAL] = CalcPriority.Equals;
            precedences[TokenType.LESS] = CalcPriority.LessGreater;
            precedences[TokenType.GREATER] = CalcPriority.LessGreater;
            precedences[TokenType.PLUS] = CalcPriority.Sum;
            precedences[TokenType.MINUS] = CalcPriority.Sum;
            precedences[TokenType.ASTERISK] = CalcPriority.Product;
            precedences[TokenType.SLASH] = CalcPriority.Product;
            precedences[TokenType.LEFT_PAREN] = CalcPriority.Call;
            precedences[TokenType.LEFT_BRACKET] = CalcPriority.Call;

            prefixFunctions = new Dictionary<TokenType, Func<Expression>>();
            prefixFunctions[TokenType.IDENTIFIER] = parseIdentifier;
            prefixFunctions[TokenType.INT] = parseInteger;
            prefixFunctions[TokenType.TRUE] = parseBoolean;
            prefixFunctions[TokenType.FALSE] = parseBoolean;
            prefixFunctions[TokenType.STRING] = parseString;
            // 前置演算
            prefixFunctions[TokenType.BANG] = parsePrefixExpression;
            prefixFunctions[TokenType.MINUS] = parsePrefixExpression;
            prefixFunctions[TokenType.LEFT_PAREN] = parseGroupedExpression;
            prefixFunctions[TokenType.IF] = parseIfExpression;
            prefixFunctions[TokenType.FUNCTION] = parseFunction;
            prefixFunctions[TokenType.LEFT_BRACKET] = parseArrayLiteral;
            prefixFunctions[TokenType.LEFT_BRACE] = parseHashLiteral;
            // 中置演算
            infixFunctions = new Dictionary<TokenType, Func<Expression, Expression>>();
            infixFunctions[TokenType.PLUS] = parseInfixExpression;
            infixFunctions[TokenType.MINUS] = parseInfixExpression;
            infixFunctions[TokenType.ASTERISK] = parseInfixExpression;
            infixFunctions[TokenType.SLASH] = parseInfixExpression;
            infixFunctions[TokenType.EQUAL] = parseInfixExpression;
            infixFunctions[TokenType.NOT_EQUAL] = parseInfixExpression;
            infixFunctions[TokenType.GREATER] = parseInfixExpression;
            infixFunctions[TokenType.LESS] = parseInfixExpression;
            // "("は関数呼び出しのための中置演算子の役割を担う
            infixFunctions[TokenType.LEFT_PAREN] = parseCallExpression;
            // "["は配列・ハッシュのインデックスのための中置演算子の役割を担う
            infixFunctions[TokenType.LEFT_BRACKET] = parseIndexExpression;
        }

        void nextToken()
        {
            curToken = peekToken;
            peekToken = lexer.NextToken();
        }

        bool expectPeek(TokenType t)
        {
            if (peekToken.Type == t)
            {
                nextToken();
                return true;
            }
            else
            {
                return false;
            }
        }

        int curPrecedence()
        {
            if (precedences.ContainsKey(curToken.Type))
            {
                return (int)precedences[curToken.Type];
            }
            else
            {
                return (int)CalcPriority.Lowest;
            }
        }

        int peekPrecedence()
        {
            if (precedences.ContainsKey(peekToken.Type))
            {
                return (int)precedences[peekToken.Type];
            }
            else
            {
                return (int)CalcPriority.Lowest;
            }
        }

        public Program ParseProgram()
        {
            var program = new Program();
            while(curToken.Type != TokenType.EOF)
            {
                var statement = parseStatement();
                if(statement != null)
                {
                    program.Statements.Add(statement);
                }
                // parseStatementは文末のセミコロンは読み残す
                nextToken();
            }
            return program;
        }

        Statement parseStatement()
        {
            switch (curToken.Type)
            {
                case TokenType.LET:
                    return parseLetStatement();
                case TokenType.RETURN:
                    return parseReturnStatement();
                default:
                    return parseExpressionStatement();
            }
        }

        LetStatement parseLetStatement()
        {
            LetStatement statement = new LetStatement(curToken);
            if (!expectPeek(TokenType.IDENTIFIER))
            {
                throw new InvalidDataException("let文の識別子が来るべきところに別のトークンが来ました");
            }
            statement.Name = new Identifier(curToken, curToken.Literal);
            if (!expectPeek(TokenType.ASSIGN))
            {
                throw new InvalidDataException("let文の識別子の後に来るのは代入演算子のはずです");
            }
            // ASSIGNの次のトークンは式の先頭のトークンである
            nextToken();
            statement.Value = parseExpression((int)CalcPriority.Lowest);
            if (!expectPeek(TokenType.SEMICOLON))
            {
                throw new InvalidDataException("let文の末尾はセミコロンであるはずです");
            }
            return statement;
        }

        ReturnStatement parseReturnStatement()
        {
            var statement = new ReturnStatement(curToken);
            // returnトークンを読み終わる
            nextToken();
            statement.ReturnValue = parseExpression((int)CalcPriority.Lowest);
            if (!expectPeek(TokenType.SEMICOLON))
            {
                throw new InvalidDataException("return文の末尾はセミコロンであるはずです");
            }
            return statement;
        }

        ExpressionStatement parseExpressionStatement()
        {
            var expressionStatement = new ExpressionStatement(curToken, parseExpression((int)CalcPriority.Lowest));
            // 式文の末尾のセミコロンは任意
            if(peekToken.Type == TokenType.SEMICOLON)
            {
                nextToken();
            }
            return expressionStatement;
        }

        BlockStatement parseBlockStatement()
        {
            var blockStatement = new BlockStatement(curToken, new List<Statement>());
            nextToken();
            // curTokenはblockstatementの最初の文
            while(curToken.Type != TokenType.RIGHT_BRACE)
            {
                if (curToken.Type == TokenType.EOF)
                {
                    throw new InvalidDataException("ブロック文が閉じられない間にプログラムが終端に到達しました");
                }
                blockStatement.Statements.Add(parseStatement());
                // parseStatement()はcurTokenが文の先頭であるときに、文末までパースする
                // というわけで、ここで1つ読み進めて次の文の先頭およびブロックの終わりの中かっこまで読み進める
                nextToken();
            }
            return blockStatement;
        }

        Expression parseExpression(int precedence)
        {
            var prefix = prefixFunctions[curToken.Type];
            if (prefix == null)
            {
                throw new InvalidDataException($"{curToken.Type}に対応するprefix関数がありません");
            }
            var leftExp = prefix();

            // 優先順位が上がり続けるかぎり、最初の中置演算子の右辺の式になる
            while (peekToken.Type != TokenType.SEMICOLON && precedence < peekPrecedence())
            {
                var infix = infixFunctions[peekToken.Type];
                nextToken();
                // infixはcurTokenが演算子のときに、式の最後の項まで読む(1+5なら、+からスタートして5まで読む)
                leftExp = infix(leftExp);
            }

            return leftExp;
        }

        Identifier parseIdentifier()
        {
            return new Identifier(curToken, curToken.Literal);
        }

        IntegerLiteral parseInteger()
        {
            return new IntegerLiteral(curToken, int.Parse(curToken.Literal));
        }

        BooleanLiteral parseBoolean()
        {
            return new BooleanLiteral(curToken, bool.Parse(curToken.Literal));
        }

        StringLiteral parseString()
        {
            return new StringLiteral(curToken, curToken.Literal);
        }

        FunctionLiteral parseFunction()
        {
            var exp = new FunctionLiteral(curToken, new List<Identifier>(), null);
            if (!expectPeek(TokenType.LEFT_PAREN))
            {
                throw new InvalidDataException("関数リテラルのfnキーワードの直後は左括弧である必要があります");
            }
            // 1個だけの引数だとカンマがないのでここだけは特殊にやる
            if (peekToken.Type != TokenType.RIGHT_PAREN)
            {
                nextToken();
                exp.Parameters.Add(parseIdentifier());
            }
            while(peekToken.Type != TokenType.RIGHT_PAREN)
            {
                // x ,yのうち、この時点だとxを見ている
                nextToken();
                nextToken();
                // この時点だとxと,を読み込んでyを見ている
                exp.Parameters.Add(parseIdentifier());
            }
            // 閉じ括弧を読む
            nextToken();
            // この時点でcurrentがrightParentになるはず
            if (!expectPeek(TokenType.LEFT_BRACE))
            {
                throw new InvalidDataException("関数リテラルの本体はは左中括弧から始まる必要があります");
            }
            exp.Body = parseBlockStatement();
            return exp;
        }

        PrefixExpression parsePrefixExpression()
        {
            var exp = new PrefixExpression(curToken, curToken.Literal, null);
            nextToken();
            exp.Right = parseExpression((int)CalcPriority.Prefix);
            return exp;
        }

        Expression parseGroupedExpression()
        {
            // 左括弧を読む
            nextToken();

            // 最も広く結合をとる。(左側はかっこなので、結合力が最も弱い)
            var exp = parseExpression((int)CalcPriority.Lowest);
            if (!expectPeek(TokenType.RIGHT_PAREN))
            {
                throw new InvalidDataException("かっこの内部の式の終了後には閉じ括弧をおいてください");
            }
            return exp;
        }

        InfixExpression parseInfixExpression(Expression left)
        {
            var infix = new InfixExpression(curToken, left, null, curToken.Literal);
            int precedence = curPrecedence();
            // ここで演算子から進めて右の項の最初のトークンへ
            nextToken();
            infix.Right = parseExpression(precedence);
            return infix;
        }

        IfExpression parseIfExpression()
        {
            var exp = new IfExpression(curToken, null, null, null);
            if (!expectPeek(TokenType.LEFT_PAREN))
            {
                throw new InvalidDataException("if式の条件式は左括弧から始まるべきです");
            }
            // if ()の条件式部分を読む
            exp.Condition = parseGroupedExpression(); // curTokenが(からスタートする
            // この時点でcurTokenが)で、peekTokenが{のはず
            if (!expectPeek(TokenType.LEFT_BRACE))
            {
                throw new InvalidDataException("if式のブロックは左中括弧で開始するべきです");
            }
            // この時点でcurTokenが{である
            exp.Consequence = parseBlockStatement();
            // この時点でcurTokenが}であるはず
            if (peekToken.Type == TokenType.ELSE)
            {
                // else節が存在するので読み込む
                if (!expectPeek(TokenType.ELSE))
                {
                    throw new InvalidDataException("else節の先頭はelseトークンから始まるべきです");
                }
                if (!expectPeek(TokenType.LEFT_BRACE))
                {
                    throw new InvalidDataException("else節のブロックは左中括弧で開始するべきです");
                }
                // curTokenは{である
                exp.Alternative = parseBlockStatement();
                // この時点でcurTokenは}である。
            }
            // Expressionのパースは、curTokenが式の末尾にあればよいのでここで終了する
            return exp;
        }

        CallExpression parseCallExpression(Expression exp)
        {
            // curTokenは(なので、次のトークンを読んでいく
            var callExpression = new CallExpression(curToken, exp, new List<Expression>());
            
            // 最初の1個はカンマがないので特別扱い
            if (peekToken.Type != TokenType.RIGHT_PAREN)
            {
                // 最初の式の先頭まで読む(左括弧を飛ばす)
                nextToken();
                callExpression.Arguments.Add(parseExpression((int)CalcPriority.Lowest));
            }
            while (peekToken.Type != TokenType.RIGHT_PAREN)
            {
                if (!expectPeek(TokenType.COMMA))
                {
                    throw new InvalidDataException("引数をカンマ区切りで指定してください");
                }
                // curTokenはカンマの次(つまり次の引数を表す式の先頭)へ動かす
                nextToken();
                callExpression.Arguments.Add(parseExpression((int)CalcPriority.Lowest));
            }
            // ここでRightParenまで読む(curTokenがRightParenになる)
            nextToken();
            return callExpression;
        }

        IndexExpression parseIndexExpression(Expression left)
        {
            var exp = new IndexExpression(curToken, left, null);
            // curTokenが[なので1文字読む
            nextToken();
            var index = parseExpression((int)CalcPriority.Lowest);
            if (!expectPeek(TokenType.RIGHT_BRACKET))
            {
                throw new InvalidDataException("添え字演算子が]で閉じられていません");
            }
            // curTokenが]になる
            exp.Index = index;
            return exp;
        }

        ArrayLiteral parseArrayLiteral()
        {
            // この時点で見ているトークンは[である
            var array = new ArrayLiteral(curToken, new List<Expression>());
            if (peekToken.Type != TokenType.RIGHT_BRACKET)
            {
                nextToken();
                array.Elements.Add(parseExpression((int)CalcPriority.Lowest));
            }
            while (peekToken.Type == TokenType.COMMA)
            {
                // この時点では直前のparseExpressionによりExpressionの末尾を見ているわけなので、
                nextToken(); // カンマを見ている
                nextToken(); // 式の先頭を見ている
                array.Elements.Add(parseExpression((int)CalcPriority.Lowest));
            }
            nextToken(); // 式の末尾の]を見ている
            return array;
        }

        HashLiteral parseHashLiteral()
        {
            var hash = new HashLiteral(curToken, new Dictionary<Expression, Expression>());
            if (peekToken.Type != TokenType.RIGHT_BRACE)
            {
                nextToken(); // curTokenが式の最初になる
                var left = parseExpression((int)CalcPriority.Lowest);
                if (!expectPeek(TokenType.COLON))
                {
                    throw new InvalidDataException("ハッシュのキーと値はコロンで挟んでください");
                }
                nextToken();
                var right = parseExpression((int)CalcPriority.Lowest);
                hash.KeyValuePairs[left] = right;
            }
            while(peekToken.Type != TokenType.RIGHT_BRACE)
            {
                nextToken(); // curTokenがカンマ
                nextToken(); // curTokenが式の最初
                var left = parseExpression((int)CalcPriority.Lowest);
                if (!expectPeek(TokenType.COLON)) // curTokenがコロン
                {
                    throw new InvalidDataException("ハッシュのキーと値はコロンで挟んでください");
                }
                nextToken(); // curTokenが式の最初
                var right = parseExpression((int)CalcPriority.Lowest);
                hash.KeyValuePairs[left] = right;
            }
            if (!expectPeek(TokenType.RIGHT_BRACE))
            {
                throw new InvalidDataException("ハッシュの終了トークンが見つかりませんでした");
            }
            return hash;
        }

    }
}
