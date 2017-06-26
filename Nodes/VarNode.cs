using System;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class VarNode : SymNode
    {
        public VarNode(String var)
        {
            Type = NodeTypes.Var;
            Var = var;
        }

        public String Var;

        public override void Format(FormatBuilder fb)
        {
            fb.Append(Var);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            return ((VarNode)other).Var == Var;
        }

        public override SymNode DeepClone()
        {
            return new VarNode(Var);
        }

        public override SymNode FoldConstants()
        {
            return new VarNode(Var);
        }

        public override SymNode Evaluate()
        {
            return new VarNode(Var);
        }

        public override SymNode Differentiate(string var)
        {
            if (var == Var)
                return new ConstNode(1);

            return new ConstNode(0);
        }

        public override void AssertValid()
        {
            base.AssertValid();
            Debug.Assert(Var != null);
            Debug.Assert(Var.Length >= 1);
            foreach (var c in Var)
                Debug.Assert(Char.IsLetter(c));
        }
    }
}
