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
        // As the operation is commutative, we can reorder the elements if we wish.


        public override void Sort()
        {
            Children.ForEach(node => node.Sort());
            Children.Sort((a, b) => SymNode.CompareNodes(a, b));
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
        //  *   Since the list may not have a set order, we just find the first constant.
        //
        // *    If constants have not been folded, it is possible thath the may be more than one
        //      constant node, in which case Otherse may also start with constatns.
        //
        public ChildSplit SplitConstChild()
        {
            var result = new ChildSplit();

            foreach (var node in Children)
            {
                if (node.Type == NodeTypes.Constant)
                {
                    if (result.Constant == null)
                    {
                        result.Constant = (ConstNode) node;
                        continue;
                    }
                }
                if (result.Others == null)
                    result.Others = new List<SymNode>();

                result.Others.Add(node);
            }
            return result;
        }


    }
}
