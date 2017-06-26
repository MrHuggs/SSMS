using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    // Node for the differntial of a variable:
    class DNode : SymNode
    {
        public DNode(String var)
        {
            Type = NodeTypes.Differential;
            Var = var;
        }

        public String Var;


        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Differential)
                return false;

            return ((DNode)other).Var == Var;
        }

        public override void Format(FormatBuilder fb)
        {
            fb.Append(" d");
            fb.Append(Var);
        }

        public override SymNode DeepClone()
        {
            return new DNode(Var);
        }


        public override SymNode FoldConstants()
        {
            return new DNode(Var);
        }

        public override SymNode Evaluate()
        {
            return new DNode(Var);
        }

        public override SymNode Differentiate(string var)
        {
            throw new ApplicationException("Cannot differentiate a differential.");
        }
    }
}
