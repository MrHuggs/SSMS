using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    public class DistributiveTransform : NodeTransform
    {
        TransformAttributes[] _Attributes = { TransformAttributes.Expand };
        public override TransformAttributes[] Attributes { get { return _Attributes; } }

        public override SymNode Apply(SymNode start_node)
        {
            if (start_node.Type == NodeTypes.Prod)
            {
                return TryDistributeProdNode((ProdNode)start_node);
            }

            return null;
        }


        static public PlusNode TryDistributeProdNode(ProdNode prod_node)
        {
            // When passed in a product node, see if contains a plus node that can be
            // distributed accross. If it can, distribute and return a new plus node.
            // 
            // Else return null.

            int index;
            for (index = 0; ; index++)
            {
                if (index == prod_node.ChildCount())
                {
                    // Didn't fine a plus node, so nothing to do.
                    return null;
                }
                var child = prod_node.GetChild(index);
                if (child.Type == NodeTypes.Plus && child.ChildCount() > 1)
                {
                    break;
                }
            }

            PlusNode new_parent = new PlusNode();
            PlusNode existing_plus = (PlusNode) prod_node.GetChild(index);
            prod_node.RemoveChild(index);

            for (int i = 0; i < existing_plus.ChildCount(); i++)
            {
                var prod = new ProdNode(existing_plus.GetChild(i).DeepClone(),
                                prod_node.DeepClone()
                                );
                new_parent.AddChild(prod);
            }
            return new_parent;
        }
    }
}
