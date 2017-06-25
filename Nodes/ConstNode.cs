using System;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class ConstNode : SymNode
    {
        public ConstNode(double val)
        {
            Type = NodeTypes.Constant;
            Value = val;
        }

        public double Value;

        public override void Format(FormatBuilder fb)
        {
            fb.Append(Value);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Constant)
                return false;

            return ((ConstNode)other).Value == Value;
        }

        public override SymNode DeepClone()
        {
            return new ConstNode(Value);
        }

        public override bool IsZero() { return Value == 0; }
        public override bool IsOne() { return Value == 1; }

        public override SymNode FoldConstants()
        {
            return new ConstNode(Value);
        }

        public override SymNode Evaluate()
        {
            return new ConstNode(Value);
        }

        public override SymNode Differentiate(string var)
        {
            return new ConstNode(0);
        }

    }
}
