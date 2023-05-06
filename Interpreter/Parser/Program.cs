namespace Interpreter.Parser
{
    public class Program : Node
    {
        public List<Statement> Statements;

        public string TokenLiteral()
        {
            if (Statements.Count == 0)
            {
                return "";
            }
            else
            {
                return Statements[0].TokenLiteral();
            }
        }

        public Program()
        {
            Statements = new List<Statement>();
        }

        public override string ToString()
        {
            var strings = Statements.Select(x => x.ToString());
            string ret = "";
            foreach(var statement in strings)
            {
                ret += statement + "\n";
            }
            return ret;
        }
    }
}