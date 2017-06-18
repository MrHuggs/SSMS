using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SSMS
{
    public abstract class CommutativeNode : ChildListNode
    {
        // This is a base class for a node that has a bunch children that are related
        // through a commutative operation (e.g. multiplication, addition).
        // As the operation is commutative, we can reorder the elements.

        public override void AddChild(SymNode child)
        {
            Debug.Assert(!Children.Contains(child));

            int i = 0;

            var sort_val = child.GetSortVal();

            for (i = 0; i < Children.Count; i++)
            {
                var child_val = Children[i].GetSortVal();
                if (NodeSortVal.Compare(sort_val, child_val) < 0)
                    break;
            }
            Children.Insert(i, child);
        }
        
        override public NodeSortVal GetSortVal()
        {
            if (Children.Count == 0)
                return new NodeSortVal(Type);


            // We should be sorted based on our first non-constant node.
            // So, for example, 4 * 3 * x * y is sorted based on x.
            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Type != NodeTypes.Constant)
                    return Children[i].GetSortVal();
            }

            return Children[0].GetSortVal();

        }

        public CommutativeNode MergeChildrenUp()
        {
            // If we have any children that are the same type, move their nodes into us.
            // Return null if no merging occured.

            CommutativeNode result = null;
            foreach (var v in Children)
            {
                if (v.Type == Type)
                {
                    if (result == null) 
                        result = (CommutativeNode) Activator.CreateInstance(GetType()); // Create new object of the same type.

                    ((CommutativeNode)v).Children.ForEach(node => { result.AddChild(node.DeepClone()); });
                }
            }

            if (result != null)
            {
                Children.ForEach(node => { if (node.Type != Type) result.AddChild(node.DeepClone()); });
            }

            return result;
        }

        public class ChildSplit
        {
            public ConstNode Constant;     // Can be null
            public List<SymNode> Others;   // Can be null;
        }

        // Helper method: Get the value of the first constant node, and a list with the other
        // children.
        // *    Since the constants node should alwasy sort to the front, this is really just the
        //      first node if it is a Constant.
        // *    If constants have not been folded, it is possible thath the may be more than one
        //      constant node, in which case Otherse may also start with constatns.

        public ChildSplit SplitConstChild()
        {
            var result = new ChildSplit();

            if (Children.Count == 0)
                return result;

            int index;

            if (Children[0].Type == NodeTypes.Constant)
            {
                result.Constant = (ConstNode) Children[0];

                if (Children.Count == 1)
                    return result;

                index = 1;
            }
            else
                index = 0;


            result.Others = new List<SymNode>();

            for (; index < Children.Count; index++)
                result.Others.Add(Children[index]);

            return result;
        }


    }
}
