using Interpreter.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Evaluator
{
    public class Evaluator
    {
        // キャッシュ
        static Dictionary<bool, EvalBoolean> evalBooleans = new Dictionary<bool, EvalBoolean>()
        {
            { true, new EvalBoolean(true) },
            { false, new EvalBoolean(false) },
        };
        static EvalNull evalNull = new EvalNull();

        // 組み込み関数保存用の環境
        static Environment builtInEnv = new Environment();

        // 組み込み関数定義
        static Evaluator()
        {
            List<Identifier> parameters = new List<Identifier>();
            builtInEnv.Store["len"] = new EvalBuiltIn<EvalInteger>((args) => {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count != 1) { throw new ArgumentException("引数の個数が不正です"); }
                if (args[0] is EvalString str)
                {
                    return new EvalInteger(str.Value.Length);
                }
                if (args[0] is EvalArray ary)
                {
                    return new EvalInteger(ary.Array.Count);
                }
                throw new ArgumentException("引数が文字列ではありませんでした");
            });
            builtInEnv.Store["first"] = new EvalBuiltIn<EvalObject>((args) =>
            {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count != 1) { throw new ArgumentException("引数の個数が不正です"); }
                if (args[0] is EvalArray ary)
                {
                    if (ary.Array.Count <= 0)
                    {
                        return evalNull;
                    }
                    return ary.Array[0];
                }
                throw new ArgumentException("引数が配列ではありませんでした");
            });
            builtInEnv.Store["last"] = new EvalBuiltIn<EvalObject>((args) =>
            {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count != 1) { throw new ArgumentException("引数の個数が不正です"); }
                if (args[0] is EvalArray ary)
                {
                    if (ary.Array.Count <= 0)
                    {
                        return evalNull;
                    }
                    return ary.Array[ary.Array.Count - 1];
                }
                throw new ArgumentException("引数が配列ではありませんでした");
            });
            builtInEnv.Store["rest"] = new EvalBuiltIn<EvalArray>((args) =>
            {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count != 1) { throw new ArgumentException("引数の個数が不正です"); }
                if (args[0] is EvalArray ary)
                {
                    if (ary.Array.Count <= 1)
                    {
                        return new EvalArray(new List<EvalObject>());
                    }
                    return new EvalArray(ary.Array.Skip(1).ToList());
                }
                throw new ArgumentException("引数が配列ではありませんでした");
            });
            builtInEnv.Store["push"] = new EvalBuiltIn<EvalArray>((args) =>
            {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count != 2) { throw new ArgumentException("引数の個数が不正です"); }
                if (args[0] is EvalArray ary)
                {
                    if (args[1] is EvalObject pushed)
                    {
                        var pushedAry = ary.Array.Concat(new[] { pushed }).ToList();
                        return new EvalArray(pushedAry);
                    }
                    else
                    {
                        throw new ArgumentException("第二引数が不正です");
                    }
                }
                else
                {
                    throw new ArgumentException("引数が配列ではありませんでした");
                }
            });
            builtInEnv.Store["print"] = new EvalBuiltIn<EvalNull>((args) =>
            {
                if (args == null) { throw new ArgumentException("引数が指定されていません"); }
                if (args.Count == 2) { throw new ArgumentException("引数の個数が不正です"); }
                for(int i = 0;i < args.Count; i++)
                {
                    Console.WriteLine((args[i] as EvalObject)?.Inspect());
                }
                return evalNull;
            });
        }


        // 本体
        public static EvalObject Eval(Node node, Environment env)
        {
            switch (node)
            {
                case Program program:
                    return evalProgram(program.Statements, env);
                case ExpressionStatement expressionStatement:
                    return Eval(expressionStatement.Expression, env);
                case IntegerLiteral integerLiteral:
                    return new EvalInteger(integerLiteral.Value);
                case BooleanLiteral booleanLiteral:
                    return evalBooleans[booleanLiteral.Value];
                case StringLiteral stringLiteral:
                    return new EvalString(stringLiteral.Value);
                case ArrayLiteral arrayLiteral:
                    return new EvalArray(arrayLiteral.Elements.Select(x => Eval(x, env)).ToList());
                case HashLiteral hashLiteral:
                    return evalHashLiteral(hashLiteral, env);
                case PrefixExpression prefixExpression:
                    var rightPrefix = Eval(prefixExpression.Right, env);
                    return evalPrefixExpression(prefixExpression.Operator, rightPrefix, env);
                case InfixExpression infixExpression:
                    var leftInfix = Eval(infixExpression.Left, env);
                    var rightInfix = Eval(infixExpression.Right, env);
                    return evalInfixExpression(infixExpression.Operator, leftInfix, rightInfix, env);
                case IfExpression ifExpression:
                    return evalIfExpression(ifExpression, env);
                case BlockStatement blockStatement:
                    return evalBlockStatement(blockStatement.Statements, env);
                case ReturnStatement returnStatement:
                    var retVal = Eval(returnStatement.ReturnValue, env);
                    return new EvalReturnValue(retVal);
                case LetStatement letStatement:
                    var letValue = Eval(letStatement.Value, env);
                    env.Store[letStatement.Name.Value] = letValue;
                    return letValue;
                case Identifier identifier:
                    return evalIdentifier(identifier, env);
                case FunctionLiteral functionLiteral:
                    return evalFunctionLiteral(functionLiteral, env);
                case CallExpression callExpression:
                    return evalCallExpression(callExpression, env);
                case IndexExpression indexExpression:
                    return evalIndexExpression(indexExpression, env);
                default:
                    return evalNull;
            }
        }


        static EvalObject evalProgram(List<Statement> statements, Environment env)
        {
            EvalObject evalObject = evalNull;
            foreach(var statement in statements)
            {
                evalObject = Eval(statement, env);
                // return文にであったら即座に返す
                // このままだとネスト1つまでしか戻せない(returnは通常、関数呼び出しの外まで即座にジャンプする)
                if(evalObject is EvalReturnValue retVal)
                {
                    return retVal.ReturnValue;
                }
            }
            return evalObject;
        }
        static EvalObject evalBlockStatement(List<Statement> statements, Environment env)
        {
            EvalObject evalObject = evalNull;
            foreach (var statement in statements)
            {
                evalObject = Eval(statement, env);
                // return文にであったら即座に返す
                // ネストしたブロック文の場合、一番上まで戻ってくる
                if (evalObject is EvalReturnValue retVal)
                {
                    return retVal;
                }
            }
            return evalObject;
        }

        static EvalObject evalPrefixExpression(string op, EvalObject value, Environment env)
        {
            switch (op)
            {
                case "!":
                    return evalBangOperatorExpression(value, env);
                case "-":
                    return evalMinusOperatorExpression(value, env);
                default:
                    throw new InvalidDataException("前置演算子式のオペレータが不正です");
            }
        }
        static EvalBoolean evalBangOperatorExpression(EvalObject value, Environment env)
        {
            switch (value)
            {
                case EvalBoolean boolean when boolean.Value == true:
                    return evalBooleans[false];
                case EvalBoolean boolean when boolean.Value == false:
                    return evalBooleans[true];
                case EvalInteger:
                    return evalBooleans[false];
                case EvalNull:
                    return evalBooleans[true];
                default:
                    throw new InvalidDataException("演算子!のオペランドが不正です");
            }
        }
        static EvalInteger evalMinusOperatorExpression(EvalObject value, Environment env)
        {
            switch (value)
            {
                case EvalInteger evalInteger:
                    return new EvalInteger(-evalInteger.Value);
                default:
                    throw new InvalidDataException("演算子-のオペランドが数値ではありません");
            }
        }

        static EvalObject evalInfixExpression(string op, EvalObject left, EvalObject right, Environment env)
        {
            switch (op)
            {
                case "+":
                    return evalPlusExpression(left, right, env);
                case "-":
                    return evalMinusExpression(left, right, env);
                case "*":
                    return evalProductExpression(left, right, env);
                case "/":
                    return evalDivideExpression(left, right, env);
                case "<":
                    return evalLessThanExpression(left, right, env);
                case ">":
                    return evalGreaterThanExpression(left, right, env);
                case "==":
                    return evalEqualExpression(left, right, env);
                case "!=":
                    return evalNotEqualExpression(left, right, env);
                default:
                    throw new InvalidDataException("中置演算子のオペレータが不正です");
            }
        }
        static EvalInteger evalPlusExpression(EvalObject left, EvalObject right, Environment env)
        {
            if(left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return new EvalInteger(leftInt.Value + rightInt.Value);
            }
            throw new InvalidDataException("中置演算子+の両辺は整数である必要があります");
        }
        static EvalInteger evalMinusExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return new EvalInteger(leftInt.Value - rightInt.Value);
            }
            throw new InvalidDataException("中置演算子-の両辺は整数である必要があります");
        }
        static EvalInteger evalProductExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return new EvalInteger(leftInt.Value * rightInt.Value);
            }
            throw new InvalidDataException("中置演算子*の両辺は整数である必要があります");
        }
        static EvalInteger evalDivideExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                if (rightInt.Value == 0)
                {
                    throw new DivideByZeroException("ゼロ除算が発生しました");
                }
                return new EvalInteger(leftInt.Value / rightInt.Value);
            }
            throw new InvalidDataException("中置演算子/の両辺は整数である必要があります");
        }
        static EvalBoolean evalLessThanExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return evalBooleans[leftInt.Value < rightInt.Value];
            }
            throw new InvalidDataException("中置演算子<の両辺は整数である必要があります");
        }
        static EvalBoolean evalGreaterThanExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return evalBooleans[leftInt.Value > rightInt.Value];
            }
            throw new InvalidDataException("中置演算子>の両辺は整数である必要があります");
        }
        static EvalBoolean evalEqualExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return evalBooleans[leftInt.Value == rightInt.Value];
            }
            else if (left is EvalBoolean leftBool && right is EvalBoolean rightBool)
            {
                // 実はValueを見なくても、leftBool == rightBoolで良い
                // なぜなら、TrueとFalseを表すEvalBooleanオブジェクトは1つずつしか存在しないからである
                return evalBooleans[leftBool == rightBool];
            }
            throw new InvalidDataException("中置演算子==の両辺は整数または真偽値である必要があります");
        }
        static EvalBoolean evalNotEqualExpression(EvalObject left, EvalObject right, Environment env)
        {
            if (left is EvalInteger leftInt && right is EvalInteger rightInt)
            {
                return evalBooleans[leftInt.Value != rightInt.Value];
            }
            else if (left is EvalBoolean leftBool && right is EvalBoolean rightBool)
            {
                return evalBooleans[leftBool != rightBool];
            }
            throw new InvalidDataException("中置演算子!=の両辺は整数または真偽値である必要があります");
        }

        static EvalObject evalIfExpression(IfExpression ifExpression, Environment env)
        {
            var condition = Eval(ifExpression.Condition, env);
            if (isTruthy(condition))
            {
                return Eval(ifExpression.Consequence, env);
            }
            else
            {
                if (ifExpression.Alternative != null)
                {
                    return Eval(ifExpression.Alternative, env);
                }
            }
            return evalNull;
        }
        static bool isTruthy(EvalObject evalObject)
        {
            switch (evalObject)
            {
                case EvalNull:
                    return false;
                case EvalBoolean b:
                    return b.Value;
                default:
                    return true;
            }
        }

        static EvalObject evalIdentifier(Identifier identifier, Environment env)
        {
            if (env.Get(identifier.Value) != null)
            {
                var referencedObj = env.Get(identifier.Value);
                return referencedObj;
            }
            if (builtInEnv.Get(identifier.Value) != null)
            {
                return builtInEnv.Get(identifier.Value);
            }
            throw new KeyNotFoundException($"識別子{identifier.Value}が環境に見つかりません");
        }
        static EvalFunctionObject evalFunctionLiteral(FunctionLiteral functionLiteral, Environment env)
        {
            var parameters = functionLiteral.Parameters;
            var body = functionLiteral.Body;
            return new EvalFunctionObject(parameters, null, body);
        }

        static List<EvalObject> evalExpressions(List<Expression> parameters, Environment env)
        {
            List<EvalObject> result = new List<EvalObject>();
            foreach(var param in parameters)
            {
                result.Add(Eval(param, env));
            }
            return result;
        }
        static EvalObject applyFunction(EvalFunctionObject func, Environment env)
        {
            var returnValue = Eval(func.Body, env);
            // 関数からreturnで返ってきた場合、関数で止める
            if(returnValue is EvalReturnValue evalReturn)
            {
                return evalReturn.ReturnValue;
            }
            // 普通に値が返ってきた場合はそれでOK
            return returnValue;
        }

        static EvalObject evalCallExpression(CallExpression callExpression, Environment env)
        {
            // 関数本体は式または識別子によって指定されている
            var evaluatedFunc = Eval(callExpression.Function, env);
            // 引数を呼び出し時に評価しておく
            var parameters = evalExpressions(callExpression.Arguments, env);

            // ユーザー定義関数なのか、組み込み関数なのかを判別する
            if (evaluatedFunc is EvalFunctionObject func)
            {
                // parametersによって拡張された新しい環境を作成する
                // OCamlとかだったらFunctionLiteralを評価するタイミングで環境のコピーを関数に保存しておく
                // Monekyだと評価時点の環境に依存するのでクロージャが使える
                var funcEnv = new Environment();
                funcEnv.Outer = env;
                // funcの引数に対応する名前の値を環境に追加する
                for (int i = 0; i < func.Parameters.Count; i++)
                {
                    string pName = func.Parameters[i].Value;
                    EvalObject pValue = parameters[i];
                    funcEnv.Store[pName] = pValue;
                }
                // 実際に関数を呼び出す
                return applyFunction(func, funcEnv);
            }
            // 組み込み関数の場合はC#で直接評価する
            else if (evaluatedFunc is EvalBuiltIn<EvalInteger> intFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return intFunc.Func(new List<object>(parameters));
            }
            else if (evaluatedFunc is EvalBuiltIn<EvalBoolean> boolFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return boolFunc.Func(new List<object>(parameters));
            }
            else if (evaluatedFunc is EvalBuiltIn<EvalString> strFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return strFunc.Func(new List<object>(parameters));
            }
            else if (evaluatedFunc is EvalBuiltIn<EvalArray> aryFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return aryFunc.Func(new List<object>(parameters));
            }
            else if (evaluatedFunc is EvalBuiltIn<EvalObject> objFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return objFunc.Func(new List<object>(parameters));
            }
            else if (evaluatedFunc is EvalBuiltIn<EvalNull> nullFunc)
            {
                // 単純に引数に指定されているオブジェクトを指定して関数を実行する
                return nullFunc.Func(new List<object>(parameters));
            }

            // ここまで来ることはないはず
            throw new NotSupportedException($"指定された関数{callExpression}が見つかりませんでした");
        }
        static EvalObject evalIndexExpression(IndexExpression indexExpression, Environment env)
        {
            var evaledLeft = Eval(indexExpression.Left, env);
            var evaledIndex = Eval(indexExpression.Index, env);
            if(evaledLeft is EvalArray ary && evaledIndex is EvalInteger idx)
            {
                if (ary == null)
                {
                    throw new InvalidDataException("添え字演算子の左辺は配列である必要があります");
                }
                if (idx == null)
                {
                    throw new InvalidDataException("添え字演算子の右辺は数値である必要があります");
                }
                if (ary.Array.Count <= idx.Value || ary.Array.Count < 0)
                {
                    throw new IndexOutOfRangeException("配列の添え字が配列の大きさの外を指定しました");
                }
                return ary.Array[idx.Value];
            }
            if (evaledLeft is EvalHash hash && evaledIndex is EvalObject obj)
            {
                if (hash == null)
                {
                    throw new InvalidDataException("添え字演算子の左辺はハッシュである必要があります");
                }
                if (!hash.KeyValuePairs.ContainsKey(obj))
                {
                    throw new KeyNotFoundException("指定されたキーはハッシュに存在しません");
                }
                return hash.KeyValuePairs[obj];
            }
            throw new InvalidDataException("インデックスの解釈に失敗しました");
        }

        static EvalHash evalHashLiteral(HashLiteral hashLiteral, Environment env)
        {
            var hash = new EvalHash(new Dictionary<EvalObject, EvalObject>());
            foreach(var kvPair in hashLiteral.KeyValuePairs)
            {
                var k = Eval(kvPair.Key, env);
                var v = Eval(kvPair.Value, env);
                // ReturnValueなどの一部を除いたEvalObjectはEqualsとGetHashCodeがオーバーライドされているため
                // Dictionaryで正しく検索できる
                hash.KeyValuePairs[k] = v;
            }
            return hash;
        }
    }
}
