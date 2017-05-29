using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    abstract class CommutativeNode : ChildListNode
    {
        // This is a base class for a node that has a bunch children that are related
        // through a commutative operation (e.g. multiplication, addition).
        // As the operation is commutative, we can reorder the elements.

        public override void AddChild(SymNode child)
        {
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

    }
}
