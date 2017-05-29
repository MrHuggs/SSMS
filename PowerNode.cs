using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class PowerNode : SymNode
    {
        public PowerNode(SymNode ebase, SymNode power)
        {
            Type = NodeTypes.Power;

            Base = ebase;
            Power = power;
        }

        SymNode Base;
        SymNode Power;

        public override void Format(StringBuilder sb)
        {
            sb.Append("(");
            Base.Format(sb);
            sb.Append(")");
            sb.Append("^(");
            Power.Format(sb);
            sb.Append(")");
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            PowerNode pnode = (PowerNode)other;

            return Base.IsEqual(pnode.Base) && Power.IsEqual(pnode.Power);
        }

        public override NodeSortVal GetSortVal()
        {
            return Base.GetSortVal();
        }

        public override SymNode DeepClone()
        {
            return new PowerNode(Base.DeepClone(), Power.DeepClone());
        }
    }
}
