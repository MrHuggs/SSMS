using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class DistributiveTransform : NodeTransform
    {
        TransformAttributes[] _Attributes = { TransformAttributes.Expand };
        public override TransformAttributes[] Attributes { get; }

        public override SymNode Apply(SymNode start_node)
        {
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

                if (prod_node.GetChild(index).Type == NodeTypes.Plus)
                    break;
            }

            PlusNode new_parent = new PlusNode();
            PlusNode existing_plus = (PlusNode) prod_node.GetChild(index);
            prod_node.RemoveChild(index);

            SymNode child1 = existing_plus.GetChild(0);
            existing_plus.RemoveChild(0);

            SymNode child2;
            if (existing_plus.ChildCount() == 1)
            {
                child2 = existing_plus.GetChild(0);
            }
            else
                child2 = existing_plus;

            ProdNode new_prod = (ProdNode)prod_node.DeepClone();

            prod_node.AddChild(child1);
            new_prod.AddChild(child2);
            new_parent.AddChild(prod_node);
            new_parent.AddChild(new_prod);

            return new_parent;
        }
    }
}
