using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    // Node for the differntial of one or more varables. They are considered
    // to be a wedge product, so order matters, more precisely, d_a d_b = -1 * d_b d_a
    //
    public class DNode : SymNode
    {
        List<String> Vars = new List<string>();

        public string Var;
        
        public DNode(string var)
        {
            Type = NodeTypes.Differential;
            Var = var;
        }


        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Differential)
                return false;

            DNode node = (DNode)other;

            return Var == node.Var;
        }

        public override void Format(FormatBuilder fb)
        {
            fb.Append("d_");
            fb.Append(Var);
        }


        public override SymNode DeepClone()
        {
            var result = new DNode(Var);
            return result;
        }


        public override SymNode FoldConstants()
        {
            return DeepClone();
        }

        public override SymNode Evaluate()
        {
            return DeepClone();
        }

        public override SymNode Differentiate(string var)
        {
            throw new ApplicationException("Cannot differentiate a differential.");
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
