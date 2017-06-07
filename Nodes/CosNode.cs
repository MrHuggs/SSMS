using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    class CosNode : TrigNode
    {
        public CosNode(SymNode angle) : base (angle)
        {
            Type = NodeTypes.Cos;
        }

        public override void Format(StringBuilder sb)
        {
            sb.Append("cos(");
            if (ChildCount() > 0)
                Angle.Format(sb);
            sb.Append(")");
        }

        // Since we cannot represent pi/2 exactly, we cannot handle IsZero.
        public override bool IsOne()
        {
            return (Angle == null) ? false : Angle.IsOne();
        }

        public override SymNode FoldConstants()
        {
            var new_angle = Angle.FoldConstants();
            if (new_angle.IsZero())
                return new ConstNode(1);

            return new CosNode(new_angle);
        }

        public override SymNode Evaluate()
        {
            var new_angle = Angle.Evaluate();

            if (new_angle.Type == NodeTypes.Constant)
            {
                double c = Math.Cos(((ConstNode)new_angle).Value);
                return new ConstNode(c);
            }
            return new CosNode(new_angle);
        }
    }
}
