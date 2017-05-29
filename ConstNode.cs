using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ConstNode : SymNode
    {
        public ConstNode(double val)
        {
            Type = NodeTypes.Constant;
            Value = val;
        }

        double Value;

        public override void Format(StringBuilder sb)
        {
            sb.Append(Value);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            return ((ConstNode)other).Value == Value;
        }

        public override NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type, Value);
        }

        public override SymNode DeepClone()
        {
            return new ConstNode(Value);
        }
    }
}
