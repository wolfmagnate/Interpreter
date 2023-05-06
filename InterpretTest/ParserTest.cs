using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter.Lexer;
using Interpreter.Parser;

namespace InterpretTest
{
    public class ParserTest
    {
        [Test]
        public void LetTest()
        {
            string input = @"
let x = 5;
let y = 10;
let foobar = 838;";
            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            Assert.That(program, Is.Not.Null);
            Assert.That(program.Statements.Count, Is.EqualTo(3));
            string[] expected = {
                "x", "y", "foobar"
            };

            for (int i = 0; i < program.Statements.Count; i++)
            {
                var statement = program.Statements[i];
                testLetStatement(statement, expected[i]);
            }
        }

        void testLetStatement(Statement s, string name)
        {
            // 最初のトークンはletである
            Assert.That(s.TokenLiteral(), Is.EqualTo("let"));
            Assert.That(s is LetStatement);
            if (s is LetStatement letS)
            {
                Assert.That(letS.Name.Value, Is.EqualTo(name));
                Assert.That(letS.Name.TokenLiteral(), Is.EqualTo(name));
            }
        }

        [Test]
        public void ReturnTest()
        {
            string input = @"
return 5;
return 10;
return 838;
";
            var l = new Lexer(input);
            var p = new Parser(l);
            var program = p.ParseProgram();
            Assert.That(program, Is.Not.Null);
            Assert.That(program.Statements.Count, Is.EqualTo(3));

            for (int i = 0; i < program.Statements.Count; i++)
            {
                var statement = program.Statements[i];
                testReturnStatement(statement);
            }
        }

        void testReturnStatement(Statement s)
        {
            Assert.That(s.TokenLiteral(), Is.EqualTo("return"));
            Assert.That(s is ReturnStatement retS);
        }


        [Test]
        public void TestIdentifierExpression()
        {
            string input = "foobar;";
            var l = new Lexer(input);
            var p = new Parser(l);
            var prog = p.ParseProgram();
            Assert.That(prog.Statements, Is.Not.Null);
            Assert.That(prog.Statements.Count, Is.EqualTo(1));
            var statement = prog.Statements[0];
            Assert.That(statement, Is.InstanceOf(typeof(ExpressionStatement)));
            if (statement is ExpressionStatement expressionStatement)
            {
                Assert.That(expressionStatement.Expression, Is.InstanceOf(typeof(Identifier)));
                if (expressionStatement.Expression is Identifier id)
                {
                    Assert.That(id.Value, Is.EqualTo("foobar"));
                    Assert.That(id.TokenLiteral(), Is.EqualTo("foobar"));
                }
            }
        }

        [Test]
        public void TestIntegerLiteralExpression()
        {
            string input = "5;";
            var l = new Lexer(input);
            var p = new Parser(l);
            var prog = p.ParseProgram();
            Assert.That(prog.Statements, Is.Not.Null);
            Assert.That(prog.Statements.Count, Is.EqualTo(1));
            Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog.Statements[0] is ExpressionStatement expressionStatement)
            {
                TestLiteralExpression(expressionStatement.Expression, 5);
            }
        }

        [Test]
        public void TestPrefixExpression1()
        {
            string input = "!5;";
            var l = new Lexer(input);
            var p = new Parser(l);
            var prog = p.ParseProgram();
            Assert.That(prog.Statements, Is.Not.Null);
            Assert.That(prog.Statements.Count, Is.EqualTo(1));
            Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog.Statements[0] is ExpressionStatement expressionStatement)
            {
                Assert.That(expressionStatement.Expression, Is.InstanceOf<PrefixExpression>());
                if (expressionStatement.Expression is PrefixExpression prefixExpression)
                {
                    Assert.That(prefixExpression.Operator, Is.EqualTo("!"));
                    TestLiteralExpression(prefixExpression.Right, 5);
                }
            }
        }
        [Test]
        public void TestPrefixExpression2()
        {
            string input = "-89;";
            var l = new Lexer(input);
            var p = new Parser(l);
            var prog = p.ParseProgram();
            Assert.That(prog.Statements, Is.Not.Null);
            Assert.That(prog.Statements.Count, Is.EqualTo(1));
            Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog.Statements[0] is ExpressionStatement expressionStatement)
            {
                Assert.That(expressionStatement.Expression, Is.InstanceOf<PrefixExpression>());
                if (expressionStatement.Expression is PrefixExpression prefixExpression)
                {
                    Assert.That(prefixExpression.Operator, Is.EqualTo("-"));
                    TestLiteralExpression(prefixExpression.Right, 89);
                }
            }
        }

        void TestLiteralExpression(Expression exp, int value)
        {
            Assert.That(exp, Is.InstanceOf<IntegerLiteral>());
            if (exp is IntegerLiteral integerLiteral)
            {
                Assert.That(integerLiteral.Value, Is.EqualTo(value));
                Assert.That(integerLiteral.TokenLiteral(), Is.EqualTo($"{value}"));
            }
        }
        void TestLiteralExpression(Expression exp, string value)
        {
            Assert.That(exp, Is.InstanceOf<Identifier>());
            if (exp is Identifier identifier)
            {
                Assert.That(identifier.TokenLiteral(), Is.EqualTo(value));
                Assert.That(identifier.Value, Is.EqualTo(value));
            }
        }
        void TestLiteralExpression(Expression exp, bool value)
        {
            Assert.That(exp, Is.InstanceOf<BooleanLiteral>());
            if (exp is BooleanLiteral booleanLiteral)
            {
                Assert.That(booleanLiteral.Value, Is.EqualTo(value));
                Assert.That(booleanLiteral.TokenLiteral(), Is.EqualTo($"{value}"));
            }
        }
        void TestStringExpression(Expression exp, string value)
        {
            Assert.That(exp, Is.InstanceOf<StringLiteral>());
            if (exp is StringLiteral stringLiteral)
            {
                Assert.That(stringLiteral.TokenLiteral(), Is.EqualTo(value));
                Assert.That(stringLiteral.Value, Is.EqualTo(value));
            }
        }
        void TestInfixExpression(Expression exp, string left, string @operator, string right)
        {
            Assert.That(exp, Is.InstanceOf<InfixExpression>());
            if (exp is InfixExpression infixExpression)
            {
                TestLiteralExpression(infixExpression.Left, left);
                Assert.That(@operator, Is.EqualTo(infixExpression.Operator));
                TestLiteralExpression(infixExpression.Right, right);
            }
        }
        void TestInfixExpression(Expression exp, int left, string @operator, int right)
        {
            Assert.That(exp, Is.InstanceOf<InfixExpression>());
            if (exp is InfixExpression infixExpression)
            {
                TestLiteralExpression(infixExpression.Left, left);
                Assert.That(@operator, Is.EqualTo(infixExpression.Operator));
                TestLiteralExpression(infixExpression.Right, right);
            }
        }
        void TestInfixExpression(Expression exp, int left, string @operator, string right)
        {
            Assert.That(exp, Is.InstanceOf<InfixExpression>());
            if (exp is InfixExpression infixExpression)
            {
                TestLiteralExpression(infixExpression.Left, left);
                Assert.That(@operator, Is.EqualTo(infixExpression.Operator));
                TestLiteralExpression(infixExpression.Right, right);
            }
        }
        void TestInfixExpression(Expression exp, string left, string @operator, int right)
        {
            Assert.That(exp, Is.InstanceOf<InfixExpression>());
            if (exp is InfixExpression infixExpression)
            {
                TestLiteralExpression(infixExpression.Left, left);
                Assert.That(@operator, Is.EqualTo(infixExpression.Operator));
                TestLiteralExpression(infixExpression.Right, right);
            }
        }
        void TestBlockStatement(Statement s, int numStatements, string[] expecteds)
        {
            Assert.That(s, Is.InstanceOf<BlockStatement>());
            if (s is BlockStatement blockStatement)
            {
                Assert.That(blockStatement.Statements.Count, Is.EqualTo(numStatements));
                Assert.That(expecteds.Length, Is.EqualTo(numStatements));

                for (int i = 0; i < numStatements; i++)
                {
                    Assert.That(blockStatement.Statements[i].ToString()?.Replace(" ", ""), Is.EqualTo(expecteds[i]));
                }
            }
        }
        void TestExpressionStatement(Statement s, string expected)
        {
            Assert.That(s, Is.InstanceOf<ExpressionStatement>());
            if (s is ExpressionStatement expressionStatement)
            {
                Assert.That(expressionStatement.Expression.ToString()?.Replace(" ", ""), Is.EqualTo(expected));
            }
        }

        // add(1, 2)のように、関数が識別子で指定されている場合に使えるヘルパー
        void TestCallExpression(Expression exp, string func, int[] arguments)
        {
            Assert.That(exp, Is.InstanceOf<CallExpression>());
            if (exp is CallExpression callExpression)
            {
                TestLiteralExpression(callExpression.Function, func);
                Assert.That(callExpression.Arguments.Count, Is.EqualTo(arguments.Length));

                for (int i = 0; i < arguments.Length; i++)
                {
                    TestLiteralExpression(callExpression.Arguments[i], arguments[i]);
                }
            }
        }

        [Test]
        public void TestInfixExpression()
        {
            string[] inputs =
            {
                "1 + 3;", "2 - 1;", "83 * 4;", "5 / 5;", "12 < 9;", "46 > 2;", "6 == 2;", "4 != 4;"
            };
            int[] leftValues = { 1, 2, 83, 5, 12, 46, 6, 4 };
            int[] rightValues = { 3, 1, 4, 5, 9, 2, 2, 4 };
            string[] operators = { "+", "-", "*", "/", "<", ">", "==", "!=" };
            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    TestInfixExpression(expressionStatement.Expression, leftValues[i], operators[i], rightValues[i]);
                }
            }
        }

        [Test]
        public void TestNextedExpression()
        {
            string[] inputs =
            {
                "-a + b;", "!-a;", "a + b + c;", "a * b - c;",
                "a + b * c;", "5 < 4 != 3 > 4;", "3 + 4 * 5 == 3 * 1 + 4 / 2;"
            };
            string[] expects =
            {
                "((-a)+b)", "(!(-a))", "((a+b)+c)", "((a*b)-c)", "(a+(b*c))",
                "((5<4)!=(3>4))", "((3+(4*5))==((3*1)+(4/2)))"
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                string input = inputs[i];
                string expected = expects[i];
                var l = new Lexer(input);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    Assert.That(expressionStatement.Expression.ToString()?.Replace(" ", ""), Is.EqualTo(expected));
                }
            }
        }

        [Test]
        public void TestBooleanExpression()
        {
            string[] inputs = { "true;", "false;" };
            bool[] expectedValues = { true, false };
            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    TestLiteralExpression(expressionStatement.Expression, expectedValues[i]);
                }
            }
        }

        [Test]
        public void TestBooleanNestedExpression()
        {
            string[] inputs = { "3 > 5 == false;", "a == b == false;", "!true == 2;" };
            string[] expects = { "((3>5)==false)", "((a==b)==false)", "((!true)==2)" };

            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    Assert.That(expressionStatement.Expression.ToString()?.Replace(" ", ""), Is.EqualTo(expects[i]));
                }
            }
        }

        [Test]
        public void TestOperatorPrecedenceParsing()
        {
            string[] inputs = { "1 + (2 + 3) + 4", "(5 + 5) * 2", "-(5 + 5)" };
            string[] expects = { "((1+(2+3))+4)", "((5+5)*2)", "(-(5+5))" };

            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    Assert.That(expressionStatement.Expression.ToString()?.Replace(" ", ""), Is.EqualTo(expects[i]));
                }
            }
        }

        [Test]
        public void TestIfExpression()
        {
            string[] inputs =
            {
                "if (x < y) { x }", "if (x == y) { let a = 15; } else { 3 * y }"
            };

            var l1 = new Lexer(inputs[0]);
            var p1 = new Parser(l1);
            var prog1 = p1.ParseProgram();
            Assert.That(prog1.Statements, Is.Not.Null);
            Assert.That(prog1.Statements.Count, Is.EqualTo(1));
            Assert.That(prog1.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog1.Statements[0] is ExpressionStatement expressionStatement1)
            {
                Assert.That(expressionStatement1.Expression, Is.InstanceOf<IfExpression>());
                if (expressionStatement1.Expression is IfExpression ifExpression1)
                {
                    TestInfixExpression(ifExpression1.Condition, "x", "<", "y");
                    TestBlockStatement(ifExpression1.Consequence, 1, new[] { "x" });
                    Assert.That(ifExpression1.Alternative, Is.Null);
                }
            }

            var l2 = new Lexer(inputs[1]);
            var p2 = new Parser(l2);
            var prog2 = p2.ParseProgram();
            Assert.That(prog2.Statements, Is.Not.Null);
            Assert.That(prog2.Statements.Count, Is.EqualTo(1));
            Assert.That(prog2.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog2.Statements[0] is ExpressionStatement expressionStatement2)
            {
                Assert.That(expressionStatement2.Expression, Is.InstanceOf<IfExpression>());
                if (expressionStatement2.Expression is IfExpression ifExpression2)
                {
                    TestInfixExpression(ifExpression2.Condition, "x", "==", "y");
                    TestBlockStatement(ifExpression2.Consequence, 1, new[] { "a" });
                    Assert.That(ifExpression2.Alternative, Is.Not.Null);
                    TestBlockStatement(ifExpression2.Alternative, 1, new[] { "(3*y)" });
                }
            }
        }

        [Test]
        public void TestFunctionExpression()
        {
            string[] inputs =
            {
                "fn() { 42; }",
                "fn(x) { x * 2; }",
                "fn(x, y) { x + y; x - y; }",
                "fn(x, y) { x + y; }",
                "let a = fn(x, y) { x * y };"
            };

            var l1 = new Lexer(inputs[0]);
            var p1 = new Parser(l1);
            var prog1 = p1.ParseProgram();
            Assert.That(prog1.Statements, Is.Not.Null);
            Assert.That(prog1.Statements.Count, Is.EqualTo(1));
            Assert.That(prog1.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog1.Statements[0] is ExpressionStatement expressionStatement1)
            {
                Assert.That(expressionStatement1.Expression, Is.InstanceOf<FunctionLiteral>());
                if (expressionStatement1.Expression is FunctionLiteral functionLiteral1)
                {
                    Assert.That(functionLiteral1.Parameters.Count, Is.EqualTo(0));
                    TestBlockStatement(functionLiteral1.Body, 1, new[] { "42" });
                }
            }

            var l2 = new Lexer(inputs[1]);
            var p2 = new Parser(l2);
            var prog2 = p2.ParseProgram();
            Assert.That(prog2.Statements, Is.Not.Null);
            Assert.That(prog2.Statements.Count, Is.EqualTo(1));
            Assert.That(prog2.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog2.Statements[0] is ExpressionStatement expressionStatement2)
            {
                Assert.That(expressionStatement2.Expression, Is.InstanceOf<FunctionLiteral>());
                if (expressionStatement2.Expression is FunctionLiteral functionLiteral2)
                {
                    Assert.That(functionLiteral2.Parameters.Count, Is.EqualTo(1));
                    TestLiteralExpression(functionLiteral2.Parameters[0], "x");
                    TestBlockStatement(functionLiteral2.Body, 1, new[] { "(x*2)" });
                }
            }

            var l3 = new Lexer(inputs[2]);
            var p3 = new Parser(l3);
            var prog3 = p3.ParseProgram();
            Assert.That(prog3.Statements, Is.Not.Null);
            Assert.That(prog3.Statements.Count, Is.EqualTo(1));
            Assert.That(prog3.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog3.Statements[0] is ExpressionStatement expressionStatement3)
            {
                Assert.That(expressionStatement3.Expression, Is.InstanceOf<FunctionLiteral>());
                if (expressionStatement3.Expression is FunctionLiteral functionLiteral3)
                {
                    Assert.That(functionLiteral3.Parameters.Count, Is.EqualTo(2));
                    TestLiteralExpression(functionLiteral3.Parameters[0], "x");
                    TestLiteralExpression(functionLiteral3.Parameters[1], "y");
                    TestBlockStatement(functionLiteral3.Body, 2, new[] { "(x+y)", "(x-y)" });
                }
            }

            var l4 = new Lexer(inputs[3]);
            var p4 = new Parser(l4);
            var prog4 = p4.ParseProgram();
            Assert.That(prog4.Statements, Is.Not.Null);
            Assert.That(prog4.Statements.Count, Is.EqualTo(1));
            Assert.That(prog4.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog4.Statements[0] is ExpressionStatement expressionStatement4)
            {
                Assert.That(expressionStatement4.Expression, Is.InstanceOf<FunctionLiteral>());
                if (expressionStatement4.Expression is FunctionLiteral functionLiteral4)
                {
                    Assert.That(functionLiteral4.Parameters.Count, Is.EqualTo(2));
                    TestLiteralExpression(functionLiteral4.Parameters[0], "x");
                    TestLiteralExpression(functionLiteral4.Parameters[1], "y");
                    TestBlockStatement(functionLiteral4.Body, 1, new[] { "(x+y)" });
                }
            }
            var l5 = new Lexer(inputs[4]);
            var p5 = new Parser(l5);
            var prog5 = p5.ParseProgram();
            Assert.That(prog5.Statements, Is.Not.Null);
            Assert.That(prog5.Statements.Count, Is.EqualTo(1));
            Assert.That(prog5.Statements[0], Is.InstanceOf<LetStatement>());
            if (prog5.Statements[0] is LetStatement letStatement5)
            {
                Assert.That(letStatement5.Name.Value, Is.EqualTo("a"));
                Assert.That(letStatement5.Value, Is.InstanceOf<FunctionLiteral>());
                if (letStatement5.Value is FunctionLiteral functionLiteral5)
                {
                    Assert.That(functionLiteral5.Parameters.Count, Is.EqualTo(2));
                    TestLiteralExpression(functionLiteral5.Parameters[0], "x");
                    TestLiteralExpression(functionLiteral5.Parameters[1], "y");
                    TestBlockStatement(functionLiteral5.Body, 1, new[] { "(x*y)" });
                }
            }
        }

        [Test]
        public void TestFunctionCall()
        {
            string[] inputs =
            {
                "add(1, 2);", "sub(3 - 1 * 2 + 1, add(2, x));"
            };

            var l1 = new Lexer(inputs[0]);
            var p1 = new Parser(l1);
            var prog1 = p1.ParseProgram();
            Assert.That(prog1.Statements, Is.Not.Null);
            Assert.That(prog1.Statements.Count, Is.EqualTo(1));
            Assert.That(prog1.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog1.Statements[0] is ExpressionStatement expressionStatement1)
            {
                TestCallExpression(expressionStatement1.Expression, "add", new[] { 1, 2 });
            }

            var l2 = new Lexer(inputs[1]);
            var p2 = new Parser(l2);
            var prog2 = p2.ParseProgram();
            Assert.That(prog2.Statements, Is.Not.Null);
            Assert.That(prog2.Statements.Count, Is.EqualTo(1));
            Assert.That(prog2.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog2.Statements[0] is ExpressionStatement expressionStatement2)
            {
                Assert.That(expressionStatement2.Expression, Is.InstanceOf<CallExpression>());
                if (expressionStatement2.Expression is CallExpression callExpression2)
                {
                    TestLiteralExpression(callExpression2.Function, "sub");
                    Assert.That(callExpression2.Arguments.Count, Is.EqualTo(2));
                    Assert.That(callExpression2.Arguments[0], Is.InstanceOf<InfixExpression>());
                    if (callExpression2.Arguments[0] is InfixExpression infixExpression1)
                    {
                        Assert.That(infixExpression1.Left, Is.InstanceOf<InfixExpression>());
                        if (infixExpression1.Left is InfixExpression infixExpression2)
                        {
                            TestLiteralExpression(infixExpression2.Left, 3);
                            Assert.That(infixExpression2.Operator, Is.EqualTo("-"));

                            Assert.That(infixExpression2.Right, Is.InstanceOf<InfixExpression>());
                            if (infixExpression2.Right is InfixExpression infixExpression3)
                            {
                                TestLiteralExpression(infixExpression3.Left, 1);
                                Assert.That(infixExpression3.Operator, Is.EqualTo("*"));
                                TestLiteralExpression(infixExpression3.Right, 2);
                            }
                        }

                        Assert.That(infixExpression1.Operator, Is.EqualTo("+"));
                        TestLiteralExpression(infixExpression1.Right, 1);
                    }
                    Assert.That(callExpression2.Arguments[1], Is.InstanceOf<CallExpression>());
                    if (callExpression2.Arguments[1] is CallExpression callExpression3)
                    {
                        TestLiteralExpression(callExpression3.Function, "add");
                        Assert.That(callExpression3.Arguments.Count, Is.EqualTo(2));
                        TestLiteralExpression(callExpression3.Arguments[0], 2);
                        TestLiteralExpression(callExpression3.Arguments[1], "x");
                    }
                }
            }
        }

        [Test]
        public void TestArrayLiteral()
        {
            string[] inputs =
            {
                "[1, 2 + 2, 3 * 2]"
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    Assert.That(expressionStatement.Expression, Is.InstanceOf<ArrayLiteral>());
                    if (expressionStatement.Expression is ArrayLiteral arrayLiteral)
                    {
                        Assert.That(arrayLiteral.Elements.Count, Is.EqualTo(3));
                        TestLiteralExpression(arrayLiteral.Elements[0], 1);
                        TestInfixExpression(arrayLiteral.Elements[1], 2, "+", 2);
                        TestInfixExpression(arrayLiteral.Elements[2], 3, "*", 2);
                    }
                }
            }
        }

        [Test]
        public void TestIndexExpression()
        {
            string[] inputs =
            {
                "[1, 2, 3][0]", "ary[1 * 3]"
            };
            var l1 = new Lexer(inputs[0]);
            var p1 = new Parser(l1);
            var prog1 = p1.ParseProgram();
            Assert.That(prog1.Statements, Is.Not.Null);
            Assert.That(prog1.Statements.Count, Is.EqualTo(1));
            Assert.That(prog1.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog1.Statements[0] is ExpressionStatement expressionStatement1)
            {
                Assert.That(expressionStatement1.Expression, Is.InstanceOf<IndexExpression>());
                if (expressionStatement1.Expression is IndexExpression indexExpression1)
                {
                    Assert.That(indexExpression1.Left, Is.InstanceOf<ArrayLiteral>());
                    if (indexExpression1.Left is ArrayLiteral arrayLiteral1)
                    {
                        Assert.That(arrayLiteral1.Elements.Count, Is.EqualTo(3));
                        TestLiteralExpression(arrayLiteral1.Elements[0], 1);
                        TestLiteralExpression(arrayLiteral1.Elements[1], 2);
                        TestLiteralExpression(arrayLiteral1.Elements[2], 3);
                    }
                    TestLiteralExpression(indexExpression1.Index, 0);
                }
            }

            var l2 = new Lexer(inputs[1]);
            var p2 = new Parser(l2);
            var prog2 = p2.ParseProgram();
            Assert.That(prog2.Statements, Is.Not.Null);
            Assert.That(prog2.Statements.Count, Is.EqualTo(1));
            Assert.That(prog2.Statements[0], Is.InstanceOf<ExpressionStatement>());
            if (prog2.Statements[0] is ExpressionStatement expressionStatement2)
            {
                Assert.That(expressionStatement2.Expression, Is.InstanceOf<IndexExpression>());
                if (expressionStatement2.Expression is IndexExpression indexExpression2)
                {
                    Assert.That(indexExpression2.Left, Is.InstanceOf<Identifier>());
                    TestLiteralExpression(indexExpression2.Left, "ary");
                    TestInfixExpression(indexExpression2.Index, 1, "*", 3);
                }
            }
        }

        [Test]
        public void TestHashLiteral()
        {
            string[] inputs =
            {
                "{\"key\":16, true: \"world\"}",
                "{\"one\":1, \"two\":2, \"three\":3}",
                "{array:[1, 2, 3], hash:{nested: 42}}"
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                var l = new Lexer(inputs[i]);
                var p = new Parser(l);
                var prog = p.ParseProgram();
                Assert.That(prog.Statements, Is.Not.Null);
                Assert.That(prog.Statements.Count, Is.EqualTo(1));
                Assert.That(prog.Statements[0], Is.InstanceOf<ExpressionStatement>());
                if (prog.Statements[0] is ExpressionStatement expressionStatement)
                {
                    Assert.That(expressionStatement.Expression, Is.InstanceOf<HashLiteral>());
                    if (expressionStatement.Expression is HashLiteral hashLiteral)
                    {
                        switch (i)
                        {
                            case 0:
                                Assert.That(hashLiteral.KeyValuePairs.Count, Is.EqualTo(2));
                                TestStringExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(0), "key");
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Values.ElementAt(0), 16);
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(1), true);
                                TestStringExpression(hashLiteral.KeyValuePairs.Values.ElementAt(1), "world");
                                break;
                            case 1:
                                Assert.That(hashLiteral.KeyValuePairs.Count, Is.EqualTo(3));
                                TestStringExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(0), "one");
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Values.ElementAt(0), 1);
                                TestStringExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(1), "two");
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Values.ElementAt(1), 2);
                                TestStringExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(2), "three");
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Values.ElementAt(2), 3);
                                break;
                            case 2:
                                Assert.That(hashLiteral.KeyValuePairs.Count, Is.EqualTo(2));
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(0), "array");
                                Assert.That(hashLiteral.KeyValuePairs.Values.ElementAt(0), Is.InstanceOf<ArrayLiteral>());
                                if (hashLiteral.KeyValuePairs.Values.ElementAt(0) is ArrayLiteral arrayLiteral)
                                {
                                    Assert.That(arrayLiteral.Elements.Count, Is.EqualTo(3));
                                    TestLiteralExpression(arrayLiteral.Elements[0], 1);
                                    TestLiteralExpression(arrayLiteral.Elements[1], 2);
                                    TestLiteralExpression(arrayLiteral.Elements[2], 3);
                                }
                                TestLiteralExpression(hashLiteral.KeyValuePairs.Keys.ElementAt(1), "hash");
                                Assert.That(hashLiteral.KeyValuePairs.Values.ElementAt(1), Is.InstanceOf<HashLiteral>());
                                if (hashLiteral.KeyValuePairs.Values.ElementAt(1) is HashLiteral nestedHashLiteral)
                                {
                                    Assert.That(nestedHashLiteral.KeyValuePairs.Count, Is.EqualTo(1));
                                    TestLiteralExpression(nestedHashLiteral.KeyValuePairs.Keys.ElementAt(0), "nested");
                                    TestLiteralExpression(nestedHashLiteral.KeyValuePairs.Values.ElementAt(0), 42);
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}
