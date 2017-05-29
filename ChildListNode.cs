using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    abstract class ChildListNode : SymNode
    {
        // A node that has multiple children kept in a list
        // Depending on the subclass, the list may or may not be able to be 
        // reordered. This comes up when a child is added.
        //

        public List<SymNode> Children = new List<SymNode>();

        public override int ChildCount() { return Children.Count; }
        public override SymNode GetChild(int idx) { return Children[idx]; }
        
        abstract public void AddChild(SymNode child);

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            CommutativeNode pnode = (CommutativeNode)other;
            var ocount = pnode.Children.Count;

            if (ocount != Children.Count)
                return false;

            for (int i = 0; i < ocount; i++)
            {
                if (!Children[i].IsEqual(pnode.Children[i]))
                    return false;
            }
            return true;
        }

        // Helper function so that derived classes can implment DeepClone.
        public void DeepCloneChildren(ChildListNode other)
        {
            for (int i = 0; i < other.Children.Count; i++)
            {
                Children.Add(other.Children[i].DeepClone());
            }
        }
    }
}
