using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    // Node for the differntial of a variable:
    class DNode : SymNode
    {
        public DNode(String var, SymNode prefix)
        {
            Type = NodeTypes.Differential;
            Var = var;
        }

        SymNode Prefix;
        public String Var;

        public override int ChildCount() { return 1; }
        public override SymNode GetChild(int index)
        {
            Debug.Assert(index == 0);
            return Prefix;
        }
        
        // Replace one node with another. If the replacement is null, the original is simply
        // removed. If you replace a node with another, it may get sorted to a different spot:
        public override void ReplaceChild(SymNode existing_child, SymNode new_child)
        {
            Debug.Assert(existing_child == Prefix);
            Prefix = new_child;
        }


        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Differential)
                return false;

            return ((DNode)other).Var == Var &&
                    ((DNode)other).Prefix.IsEqual(Prefix);
        }

        public override void Format(FormatBuilder fb)
        {
            Prefix.Format(fb);
            fb.Append(" d");
            fb.Append(Var);
        }

        public override SymNode DeepClone()
        {
            return new DNode(Var, Prefix.DeepClone());
        }

        public override bool IsZero() { return Prefix.IsZero();  }

        public override SymNode FoldConstants()
        {
            return new DNode(Var, Prefix.FoldConstants());
        }

        public override SymNode Evaluate()
        {
            return new DNode(Var, Prefix.Evaluate());
        }

        public override SymNode Merge()
        {
            SymNode merged = Prefix.Merge();
            if (merged != null)
                return merged;
            return null;
        }

        public override SymNode Differentiate(string var)
        {
            return new DNode(Var, Prefix.Differentiate(var));
        }
    }
}
