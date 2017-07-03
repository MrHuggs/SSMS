using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public abstract class ChildListNode : SymNode
    {
        // A node that has multiple children kept in a list
        // Depending on the subclass, the order of the list may or may not
        // matter.
        //

        public List<SymNode> Children = new List<SymNode>();

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != Type)
                return false;

            ChildListNode node = (ChildListNode)other;
            var ocount = node.Children.Count;

            if (ocount != Children.Count)
                return false;

            // Order matters, so just compare in order:
            for (int i = 0; i < ocount; i++)
            {
                if (!Children[i].IsEqual(node.Children[i]))
                    return false;
            }
            return true;
        }

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


        // Helper function so that derived classes can implment DeepClone.
        public void DeepCloneChildren(ChildListNode other)
        {
            for (int i = 0; i < other.Children.Count; i++)
            {
                Children.Add(other.Children[i].DeepClone());
            }
        }

        public override void AssertValid()
        {
            base.AssertValid();
            Debug.Assert(Children.Count > 1);
            Children.ForEach(node => node.AssertValid());
        }

        public override bool HasDifferential()
        {
            foreach (var v in Children)
            {
                if (v.HasDifferential())
                    return true;
            }
            return false;
        }

        public ChildListNode MergeChildrenUp()
        {
            // If we have any children that are the same type, move their nodes into us.
            // Return null if no merging occured.
            bool can_merge = false;
            foreach (var v in Children)
            {
                if (v.Type == Type)
                {
                    can_merge = true;
                    break;
                }
            }
            if (!can_merge)
                return null;

                    
            ChildListNode result = (ChildListNode)Activator.CreateInstance(GetType()); // Create new object of the same type.

            foreach (var v in Children)
            {
                if (v.Type == Type)
                {
                    ((CommutativeNode)v).Children.ForEach(node => { result.AddChild(node.DeepClone()); });
                }
                else
                    result.AddChild(v.DeepClone());
            }

            result.AssertValid();

            return result;
        }
    }
}
