using Interpreter.Evaluator;
using Interpreter.Lexer;
using Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Environment = Interpreter.Evaluator.Environment;

namespace InterpretTest
{
    public class EvaluatorTest
    {
        EvalObject testEval(string input)
        {
            var l = new Lexer(input);
            var p = new Parser(l);
            var prog = p.ParseProgram();
            Environment env = new Environment();
            return Evaluator.Eval(prog, env);
        }

        void testIntegerObject(EvalObject evaluated, int value)
        {
            Assert.That(evaluated, Is.InstanceOf<EvalInteger>());
            if (evaluated is EvalInteger evalInteger)
            {
                Assert.That(evalInteger.Value, Is.EqualTo(value));
            }
        }
        void testBooleanObject(EvalObject evaluated, bool value)
        {
            Assert.That(evaluated, Is.InstanceOf<EvalBoolean>());
            if (evaluated is EvalBoolean evalBoolean)
            {
                Assert.That(evalBoolean.Value, Is.EqualTo(value));
            }
        }
        void testStringObject(EvalObject evaluated, string value)
        {
            Assert.That(evaluated, Is.InstanceOf<EvalString>());
            if (evaluated is EvalString evalString)
            {
                Assert.That(evalString.Value, Is.EqualTo(value));
            }
        }
        void testNullObject(EvalObject evaluated)
        {
            Assert.That(evaluated, Is.InstanceOf<EvalNull>());
        }


        [Test]
        public void TestEvalIntegerExpression()
        {
            string[] inputs =
            {
                "5", "10", "-12", "--3",
                "5 + 5 + 5 + 5 - 10",
                "2 * 2 * 2 * 3",
                "-50 + 100 - 5",
                "5 * 2 - 10",
                "3 - 4 / 2",
                "(5 + 2) * 2",
                "-(2 + (3 - 3 * 2))",
            };
            int[] expecteds = {
                5, 10, -12, 3,
                10, // 5+5+5+5-10
                24, // 2*2*2*3
                45, // -50+100-5
                0,  // 5*2-10
                1,  // 3-4/2
                14, // (5+2)*2
                1  // -(2+(3-3*2))
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                testIntegerObject(evaluated, expecteds[i]);
            }
        }

        [Test]
        public void TestEvalBooleanExpression()
        {
            string[] inputs =
            {
                "true", "false", "!true", "!false", "!5", "!!true", "!!false", "!!12",
                "1 < 1", "1 > 2", "3 == 3", "4 != 2", "1 + 2 > 1",
                "true == true", "false != false", "1 + 3 == 2 == false"
            };
            bool[] expecteds =
            {
                true, false, false, true, false, true, false, true,
                false, false, true, true, true,
                true, false, true,
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                testBooleanObject(evaluated, expecteds[i]);
            }
        }

        [Test]
        public void TestEvalStringExpression()
        {
            string[] inputs = { "\"Hello! String\"" };
            string[] expecteds = { "Hello! String" };
            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                testStringObject(evaluated, expecteds[i]);
            }
        }

        [Test]
        public void TestIfExpression()
        {
            string[] inputs =
            {
                "if (true) { 10; };", // expect to return 10
                "if (false) { -5; };", // expect to return null
                "if (1) { 1 + 2; };", // expect to return 3
                "if (2 == 3) { 1 + 2; } else { -5; };", // expect to return -5
            };

            EvalObject[] expecteds =
            {
                new EvalInteger(10),
                null,
                new EvalInteger(3),
                new EvalInteger(-5),
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                if (expecteds[i] is EvalInteger expectedInt)
                {
                    testIntegerObject(evaluated, expectedInt.Value);
                }
                else if (expecteds[i] is EvalBoolean expectedBool)
                {
                    testBooleanObject(evaluated, expectedBool.Value);
                }
                else
                {
                    testNullObject(evaluated);
                }
            }
        }

        [Test]
        public void TestReturnStatement()
        {
            string[] inputs =
            {
                "return 10;",
                "return 10 + 2;",
                "return 2 * 5; 9;",
                "9; return 2 * 5;",
                @"if (10 > 1){
    if (10 > 2) { return 10; }
    return 2;
}"
            };
            int[] expecteds =
            {
                10,
                12,
                10,
                10,
                10
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                testIntegerObject(evaluated, expecteds[i]);
            }
        }

        [Test]
        public void TestLetStatement()
        {
            string[] inputs =
            {
                "let a = 5;a;",
                "let a = 5 * 5;a;",
                "let a = 5; let b = a;b;",
                "let a = 5; let y = a * 2 + 1;y;",
                "let a = 5; foobar;", // ここでは例外が発生するはずである
            };
            int[] expecteds =
            {
                5,
                25,
                5,
                11
            };

            for (int i = 0; i < 4; i++)
            {
                var evaluated = testEval(inputs[i]);
                testIntegerObject(evaluated, expecteds[i]);
            }
            Assert.That(() => testEval(inputs[4]), Throws.Exception);
        }

        [Test]
        public void TestFunctionStatement()
        {
            string input = "fn(x) { x + 2; };";
            var evaluated = testEval(input);
            Assert.That(evaluated, Is.InstanceOf<EvalFunctionObject>());
            if (evaluated is EvalFunctionObject func)
            {
                Assert.That(func.Parameters.Count, Is.EqualTo(1));
                Assert.That(func.Parameters[0].Value, Is.EqualTo("x"));
                Assert.That(func.Body.ToString()?.Replace(" ", ""), Is.EqualTo("{(x+2)}"));
            }
        }

        [Test]
        public void TestFunctionCallStatement()
        {
            string[] inputs =
            {
                "let identity = fn(x) { return x; }; identity(10);",
                "let identity = fn(x) {x}; identity(5);",
                "let add = fn(x, y) { return x + y; }; add(5, 5);",
                "fn(x, y){ x * y; }(2, 3);"
            };
            int[] expecteds =
            {
                10, 5, 10, 6
            };
            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                testIntegerObject(evaluated, expecteds[i]);
            }
        }

        [Test]
        public void TestLen()
        {
            string[] inputs =
            {
                "len(\"\")", "len(\"four\")", "len(\"hello world\")",
                "len(\"one\", \"two\")",
                "len([1, 2, \"hello\"])",
                "let a = [1, 2];3 * len([a]);"
            };
            int[] expecteds =
            {
                0, 4, 11,
                0, // placeholder
                3,
                3,
            };

            for (int i = 0; i < 3; i++)
            {
                var evaluated = testEval(inputs[i]);
                testIntegerObject(evaluated, expecteds[i]);
            }
            Assert.That(() => testEval(inputs[3]), Throws.Exception);
        }

        [Test]
        public void TestArrayIndex()
        {
            string[] inputs =
            {
                "[1, 2, 3][0]",
                "[1, \"hello\", 3][1]",
                "[true, false][2 - 1]",
                "let i = 0;[1][i]",
                "let myAry = [1, 2, 3];myAry[2];",
                "[1, 2, -2][3]", // should throw an Exception
                "[1, 2, -2][-1]", // should throw an Exception
                "let myAry = [-1, 2, 4];myAry[0] + myAry[1] + myAry[2]",
            };
            object[] expecteds =
            {
                1,
                "hello",
                false,
                1,
                3,
                null, // Placeholder for the Exception case
                null, // Placeholder for the Exception case
                5
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                if (i == 5 || i == 6) // Exception cases
                {
                    Assert.That(() => testEval(inputs[i]), Throws.Exception);
                }
                else
                {
                    var evaluated = testEval(inputs[i]);
                    if (evaluated is EvalInteger)
                    {
                        testIntegerObject(evaluated, (int)expecteds[i]);
                    }
                    else if (evaluated is EvalString)
                    {
                        testStringObject(evaluated, (string)expecteds[i]);
                    }
                    else if (evaluated is EvalBoolean)
                    {
                        testBooleanObject(evaluated, (bool)expecteds[i]);
                    }
                }
            }
        }

        [Test]
        public void TestArrayBuiltInFunction()
        {
            string[] inputs =
            {
                // test for "first"
                "first([1, 2, 4]);", // result: 1
                "first([\"apple\", \"banana\", \"cherry\"]);", // result: "apple"
                "first([false, true]);", // result: false
                
                // test for "last"
                "last([1, 2, 4]);", // result: 4
                "last([\"apple\", \"banana\", \"cherry\"]);", // result: "cherry"
                "last([false, true]);", // result: true

                // test for "push"
                "push([1, 2], 4);", // result: [1, 2, 4]

                // test for "rest"
                "let ary = [\"hello\", 3]; rest(ary);" // result: [3]
            };

            object[] expecteds =
            {
                1,
                "apple",
                false,
                4,
                "cherry",
                true,
                new EvalArray (new List<EvalObject> { new EvalInteger(1), new EvalInteger(2), new EvalInteger(4) } ),
                new EvalArray ( new List<EvalObject> { new EvalInteger(3) } ),
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                var evaluated = testEval(inputs[i]);
                if (evaluated is EvalInteger)
                {
                    testIntegerObject(evaluated, (int)expecteds[i]);
                }
                else if (evaluated is EvalString)
                {
                    testStringObject(evaluated, (string)expecteds[i]);
                }
                else if (evaluated is EvalBoolean)
                {
                    testBooleanObject(evaluated, (bool)expecteds[i]);
                }
                else if (evaluated is EvalArray)
                {
                    Assert.That(evaluated, Is.InstanceOf<EvalArray>());
                    if (evaluated is EvalArray evalArray)
                    {
                        EvalArray expectedArray = (EvalArray)expecteds[i];
                        Assert.That(evalArray.Array.Count, Is.EqualTo(expectedArray.Array.Count));

                        for (int j = 0; j < evalArray.Array.Count; j++)
                        {
                            EvalObject expectedElement = expectedArray.Array[j];
                            EvalObject actualElement = evalArray.Array[j];

                            if (expectedElement is EvalInteger && actualElement is EvalInteger)
                            {
                                Assert.That(((EvalInteger)actualElement).Value, Is.EqualTo(((EvalInteger)expectedElement).Value));
                            }
                            else if (expectedElement is EvalString && actualElement is EvalString)
                            {
                                Assert.That(((EvalString)actualElement).Value, Is.EqualTo(((EvalString)expectedElement).Value));
                            }
                            else if (expectedElement is EvalBoolean && actualElement is EvalBoolean)
                            {
                                Assert.That(((EvalBoolean)actualElement).Value, Is.EqualTo(((EvalBoolean)expectedElement).Value));
                            }
                            else
                            {
                                Assert.Fail("Unexpected element type in the array.");
                            }
                        }
                    }
                }
            }
        }

        [Test]
        public void TestArrayMap()
        {
            string input = @"
let map = fn(arr, f) {
    let iter = fn(arr, acc) {
        if (len(arr) == 0){
            return acc;
        } else {
            return iter(rest(arr), push(acc, f(first(arr))));
        }
    };
    iter(arr, []);
};
return map([1, 2, 3], fn(x) { 2 * x });
";
            EvalArray expected = new EvalArray(new List<EvalObject> {
                new EvalInteger(2),
                new EvalInteger(4),
                new EvalInteger(6)
            });

            var evaluated = testEval(input);
            Assert.That(evaluated, Is.InstanceOf<EvalArray>());

            if (evaluated is EvalArray evalArray)
            {
                Assert.That(evalArray.Array.Count, Is.EqualTo(expected.Array.Count));

                for (int i = 0; i < evalArray.Array.Count; i++)
                {
                    EvalObject expectedElement = expected.Array[i];
                    EvalObject actualElement = evalArray.Array[i];

                    if (expectedElement is EvalInteger && actualElement is EvalInteger)
                    {
                        Assert.That(((EvalInteger)actualElement).Value, Is.EqualTo(((EvalInteger)expectedElement).Value));
                    }
                    else
                    {
                        Assert.Fail("Unexpected element type in the array.");
                    }
                }
            }
        }

        [Test]
        public void TestHashLiteral()
        {
            string[] inputs =
            {
                "{\"foo\": 5}[\"foo\"]", // should be 5
                "{\"foo\": 5}[\"bar\"]", // should throw an Exception
                "let key = \"foo\";{\"foo\": 5}[key]", // should be 5
                "{}[12]", // should throw an Exception
                "{5: 15}[5]", // should be 15
                "{false: -4 * 2}[1 != 1]", // should be -8
                "[{\"name\": \"Alice\", \"age\": 25}, {\"name\": \"Bob\", \"age\": 28}][0][\"age\"]"
            };
            object[] expecteds =
            {
                5,
                null, // Placeholder for the Exception case
                5,
                null, // Placeholder for the Exception case
                15,
                -8,
                25
            };

            for (int i = 0; i < inputs.Length; i++)
            {
                if (i == 1 || i == 3) // Exception cases
                {
                    Assert.That(() => testEval(inputs[i]), Throws.Exception);
                }
                else
                {
                    var evaluated = testEval(inputs[i]);
                    if (evaluated is EvalInteger)
                    {
                        testIntegerObject(evaluated, (int)expecteds[i]);
                    }
                    else if (evaluated is EvalString)
                    {
                        testStringObject(evaluated, (string)expecteds[i]);
                    }
                    else if (evaluated is EvalBoolean)
                    {
                        testBooleanObject(evaluated, (bool)expecteds[i]);
                    }
                }
            }
        }
    }
}
