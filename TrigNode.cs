using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    abstract class TrigNode : SymNode
    {
        public TrigNode(SymNode angle)
        {
            Angle = angle;
        }

        public SymNode Angle;

        public override int ChildCount()
        {
            return 1;
        }

        public override SymNode GetChild(int idx)
        {
            Debug.Assert(idx == 0);
            return Angle;
        }

     
        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            return Angle.IsEqual(((CosNode)other).Angle);
        }

        public override SymNode DeepClone()
        {
            var n = new CosNode(Angle.DeepClone());

            return n;
        }

    }
}
