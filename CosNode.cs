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
    }
}
