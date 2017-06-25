﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SSMS.Nodes;

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

            PlusNode existing_plus = (PlusNode) prod_node.GetChild(index);
            Debug.Assert(existing_plus.ChildCount() > 1);


            PlusNode new_parent = new PlusNode();
            for (int i = 0; i < existing_plus.ChildCount(); i++)
            {
                var new_prod = new ProdNode();
                new_prod.AddChild(existing_plus.GetChild(i).DeepClone());

                for (int j = 0; j < prod_node.ChildCount(); j++)
                {
                    if (j == index)
                        continue;       // Ignore the existing child.

                    var child = prod_node.GetChild(j);
                    new_prod.AddChild(child.DeepClone());
                }
                new_parent.AddChild(new_prod);
            }
      
            return new_parent;
        }
    }
}
