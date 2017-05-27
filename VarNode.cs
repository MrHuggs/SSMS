using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class VarNode : SymNode
    {
        public VarNode(String var)
        {
            Type = NodeTypes.Var;
            Var = var;
        }

        String Var;

        public override void Format(StringBuilder sb)
        {
            sb.Append(Var);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            return ((VarNode)other).Var == Var;
        }

        // Compare for lexcial and presentation purposes. Return < 0 of this node 
        // should come after other.
        override public int LexicalCompare(SymNode other)
        {
            if (other.Type < NodeTypes.Var)
            {
                return -1;
            }

            if (other.Type > NodeTypes.Var)
            {
                return 1;
            }

            return Var.CompareTo(((VarNode)other).Var);
        }
    }
}
