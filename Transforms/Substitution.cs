using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    public class Substitution
    {
        public static SymNode Substitute(SymNode root, SymNode target, SymNode replacement)
        {
            var temp_parent = new PlusNode(root.DeepClone());
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
                    it.Parent.ReplaceChild(node, replacement.DeepClone());
                }

            }

            return temp_parent.GetChild(0);
        }
    }

}
