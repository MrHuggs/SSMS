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

        public override bool Evaluate(StringBuilder report, out double result)
        {
            if (Angle == null)
            {
                report.Append("Cannot evaluate cosine because angle is missing.");
                result = 0;
                return false;
            }
            if (!Angle.Evaluate(report, out result))
            {
                report.Append("Cannot evaluate cosine because angle could not be evaluated.");
                return false;
            }
            result = Math.Cos(result);
            return true;
        }

    }
}
