using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class CosNode : TrigNode
    {
        public CosNode(SymNode angle) : base (angle)
        {
            Type = NodeTypes.Cos;
        }

        public override void Format(FormatBuilder fb)
        {
            fb.Append("cos(");
            if (ChildCount() > 0)
                Angle.Format(fb);
            fb.Append(')');
        }

        // Since we cannot represent pi/2 exactly, we cannot handle IsZero.
        public override bool IsOne()
        {
            return Angle.IsZero();
        }

        public override SymNode DeepClone()
        {
            var n = new CosNode(Angle.DeepClone());
            return n;
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

        public override SymNode Differentiate(string var)
        {
            SymNode adif = Angle.Differentiate(var);

            return new ProdNode(
                        new ConstNode(-1),
                        adif,
                        new SinNode(Angle.DeepClone())
                        );
        }
    }
}
