using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    public abstract class TrigNode : SymNode
    {
        public TrigNode(SymNode angle)
        {
            Angle = angle;
        }

        public SymNode Angle;

        public override int ChildCount() { return 1; }
        public override SymNode GetChild(int index)
        {
            Debug.Assert(index == 0);
            return Angle;
        }
        public override void ReplaceChild(SymNode existing_child, SymNode new_child)
        {
            Debug.Assert(existing_child == Angle);
            Debug.Assert(new_child != null);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != Type)
                return false;

            return Angle.IsEqual(((TrigNode)other).Angle);
        }


    }
}
