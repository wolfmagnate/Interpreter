using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lexer = Interpreter.Lexer.Lexer;
using Parser = Interpreter.Parser.Parser;
using Evaluator = Interpreter.Evaluator.Evaluator;

namespace Interpreter.REPL
{
    public class REPL
    {

        static void Main()
        {
            Console.WriteLine("Welcome to Monkey REPL");
            Console.WriteLine("type your code...");
            var env = new Evaluator.Environment();
            while (true)
            {
                Console.Write(">>");
                string input = Console.ReadLine() ?? "";
                if (input.StartsWith("exit()")) { break; }
                interpret(input, env);
            }
        }

        static void interpret(string input, Evaluator.Environment env)
        {
            Lexer.Lexer l = new Lexer.Lexer(input);
            Parser.Parser p = new Parser.Parser(l);
            var program = p.ParseProgram();
            var eval = Evaluator.Evaluator.Eval(program, env);
            Console.WriteLine(eval.Inspect());
        }
    }
}
