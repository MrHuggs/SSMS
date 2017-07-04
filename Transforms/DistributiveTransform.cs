using System;
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
            if (start_node.Type == NodeTypes.Prod || start_node.Type == NodeTypes.Wedge)
            {
                return TryDistributeProdNode((ChildListNode)start_node);
            }

            return null;
        }


        static public PlusNode TryDistributeProdNode(ChildListNode parent_node)
        {
            // When passed in a node with children, see if contains a plus node that can be
            // distributed accross. If it can, distribute and return a new plus node.
            // 
            // Else return null.

            int index;
            for (index = 0; ; index++)
            {
                if (index == parent_node.ChildCount())
                {
                    // Didn't find a plus node, so nothing to do.
                    return null;
                }
                var child = parent_node.GetChild(index);
                if (child.Type == NodeTypes.Plus && child.ChildCount() > 1)
                {
                    break;
                }
            }

            PlusNode existing_plus = (PlusNode) parent_node.GetChild(index);
            Debug.Assert(existing_plus.ChildCount() > 1);


            PlusNode new_parent = new PlusNode();
            for (int i = 0; i < existing_plus.ChildCount(); i++)
            {
                var new_op = (ChildListNode)Activator.CreateInstance(parent_node.GetType()); // Create new object of the same type.

                for (int j = 0; j < parent_node.ChildCount(); j++)
                {
                    if (j == index)
                    {
                        new_op.AddChild(existing_plus.GetChild(i).DeepClone());
                        continue;       // Ignore the existing child.
                    }

                    var child = parent_node.GetChild(j);
                    new_op.AddChild(child.DeepClone());
                }
                new_parent.AddChild(new_op);
            }
      
            return new_parent;
        }
    }
}
