using System;
using System.Collections.Generic;
using System.Linq;
using SSMS.Nodes;

namespace SSMS
{
    public class Substitution
    {
        public static SymNode Substitute(SymNode root, SymNode target, SymNode replacement)
        {
            // Create a temporary plus node for the iterator. Since it has only a single child
            // it's not in a valid state.
            var temp_parent = new PlusNode();
            temp_parent.AddChild(root.DeepClone()); 

            var it = new TreeIterator(temp_parent);

            SymNode node;

            while (true)
            {
                it.Next();
                node = it.Cur;
                if (node == temp_parent)
                    break;

                if (node.IsEqual(target))
                {
                    it.Parent().ReplaceChild(node, replacement.DeepClone());
                }

            }

            return temp_parent.GetChild(0);
        }
    }

}
