using System;
using System.Diagnostics;

namespace SSMS.Nodes
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

        public override void AssertValid()
        {
            base.AssertValid();
            Debug.Assert(Angle != null);
            Angle.AssertValid();

            Debug.Assert(!Angle.HasDifferential());
        }

        public override bool HasDifferential()
        {
            if (Angle.HasDifferential())
                throw new ApplicationException(string.Format("Trig node {0} has differentials.", ToString()));

            return false;
        }

    }
}
