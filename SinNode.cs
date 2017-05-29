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
    }
}