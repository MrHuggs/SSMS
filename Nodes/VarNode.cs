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

        public override NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type, Var);
        }

        public override SymNode DeepClone()
        {
            return new VarNode(Var);
        }

    }
}
