using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ConstFoldTransform
    {
        static public bool Transform(SymNode node)
        {
            // See if any of the childrend of this node constants that can be folded together.

            if (node.Type != NodeTypes.Prod)
                return false;
            var prod_node = (ProdNode)node;

            if (prod_node.ChildCount() < 2)
                return false;

            // Since a product node keeps all its children sorted, with the constants
            // in the front, we only need examines the starting nodes.
            var first_child = prod_node.Children[0];

            if (first_child.Type != NodeTypes.Constant)
                return false;

            var first_const = (ConstNode)first_child;

            int removed = 0;
            while (prod_node.ChildCount() > 1)
            {
                var child = prod_node.GetChild(1);
                if (child.Type != NodeTypes.Constant)
                    break;

                first_const.Value *= ((ConstNode)child).Value;

                prod_node.RemoveChild(1);
                removed++;
            }

            // Special cases:
            if (first_const.Value == 1)
            {
                prod_node.RemoveChild(0);
                removed++;
            }
            else if (first_const.Value == 0)
            {
                while (prod_node.ChildCount() > 1)
                {
                    prod_node.RemoveLastChild();
                    removed++;
                }
            }

            return removed > 0;
        }

    }
}
