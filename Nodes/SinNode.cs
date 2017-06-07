using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class SinNode : TrigNode
    {
        public SinNode(SymNode angle) : base (angle)
        {
            Type = NodeTypes.Sin;
        }

        public override void Format(StringBuilder sb)
        {
            sb.Append("sin(");
            if (ChildCount() > 0)
                Angle.Format(sb);
            sb.Append(")");
        }

        // Since we cannot represent pi/2 exactly, we cannot handle IsOne.
        public override bool IsZero()
        {
            return (Angle == null) ? false : Angle.IsOne();
        }


        public override SymNode FoldConstants()
        {
            var new_angle = Angle.FoldConstants();
            if (new_angle.IsZero())
                return new ConstNode(0);

            return new SinNode(new_angle);
        }

        public override SymNode Evaluate()
        {
            var new_angle = Angle.Evaluate();

            if (new_angle.Type == NodeTypes.Constant)
            {
                double s = Math.Sin(((ConstNode)new_angle).Value);
                return new ConstNode(s);
            }
            return new SinNode(new_angle);
        }
    }

}