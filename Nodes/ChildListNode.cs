using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    public abstract class ChildListNode : SymNode
    {
        // A node that has multiple children kept in a list
        // Depending on the subclass, the order of the list may or may not
        // matter.
        //

        public List<SymNode> Children = new List<SymNode>();

        public override int ChildCount() { return Children.Count; }
        public override SymNode GetChild(int index) { return Children[index]; }

        public override void ReplaceChild(SymNode existing_child, SymNode new_child)
        {
            Children.Remove(existing_child);

            if (new_child != null)
                AddChild(new_child);
        }

        public void AddChild(SymNode child)
        {
            Debug.Assert(!Children.Contains(child));

            Children.Add(child);
        }

        public void RemoveChild(int index) { Children.RemoveAt(index); }
        public void RemoveChild(SymNode node) { Children.Remove(node); }
        public void RemoveLastChild() { Children.RemoveAt(Children.Count - 1);  }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != Type)
                return false;

            CommutativeNode pnode = (CommutativeNode)other;
            var ocount = pnode.Children.Count;

            if (ocount != Children.Count)
                return false;

            // The other list might be in a different order.
            bool[] used = new bool[ocount];

            for (int i = 0; i < ocount; i++)
            {
                SymNode child = Children[i];
                for (int j = 0; ; j++)
                {
                    if (j == ocount)
                        return false;   // failed to find a match.
                    if (used[j])
                        continue;

                    if (child.IsEqual(pnode.Children[i]))
                        break;
                }
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
