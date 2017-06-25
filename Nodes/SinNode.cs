using System;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class SinNode : TrigNode
    {
        public SinNode(SymNode angle) : base (angle)
        {
            Type = NodeTypes.Sin;
        }

        public override void Format(FormatBuilder fb)
        {
            fb.Append("sin(");
            if (ChildCount() > 0)
                Angle.Format(fb);
            fb.Append(')');
        }

        // Since we cannot represent pi/2 exactly, we cannot handle IsOne.
        public override bool IsZero()
        {
            return Angle.IsZero();
        }

        public override SymNode DeepClone()
        {
            var n = new SinNode(Angle.DeepClone());
            return n;
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

        public override SymNode Differentiate(string var)
        {
            SymNode adif = Angle.Differentiate(var);

            return new ProdNode(
                        adif,
                        new CosNode(Angle.DeepClone())
                        );
        }
    }

}