using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

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

        public SymNode Base;
        public SymNode Power;

        public override int ChildCount() { return 2; }
        public override SymNode GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return Base;
                case 1:
                    return Power;
                default:
                    Debug.Assert(false);
                    return null;
            }
        }
        public override void ReplaceChild(SymNode existing_child, SymNode new_child)
        {
            // We must always have two arguments.
            Debug.Assert(new_child != null);
            if (existing_child == Base)
                Base = new_child;
            else
            {
                Debug.Assert(existing_child == Power);
                Power = new_child;
            }
        }
  
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

        // A helper: If this has a fixed exponent, return it.
        public bool GetFixedPower(out double power_val)
        {
            if (Power.Type != NodeTypes.Constant)
            {
                power_val = 0;
                return false;
            }

            power_val = ((ConstNode)Power).Value;
            return true;
        }


    }
}
