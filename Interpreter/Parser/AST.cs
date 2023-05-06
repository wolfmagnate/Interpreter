using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.Parser
{
    public interface Node
    {
        string TokenLiteral();
    }

    public interface Statement : Node
    {
        Node statementNode();
    }

    public interface Expression : Node
    {
        Node expressionNode();
    }
}
