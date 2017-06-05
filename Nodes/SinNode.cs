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

        public override bool Evaluate(StringBuilder report, out double result)
        {
            if (Angle == null)
            {
                report.Append("Cannot evaluate sine because angle is missing.");
                result = 0;
                return false;
            }
            if (!Angle.Evaluate(report, out result))
            {
                report.Append("Cannot evaluate sine because angle could not be evaluated.");
                return false;
            }
            result = Math.Sin(result);
            return true;
        }
    }

}