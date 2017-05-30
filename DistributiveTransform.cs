using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class DistributiveTransform
    {
        static public bool Transform(SymNode node)
        {
            if (node.Type != NodeTypes.Prod)
                return false;
            var prod_node = (ProdNode)node;

            int index;
            for (index = 0; ; index++)
            {
                if (index == prod_node.ChildCount())
                {
                    // Didn't fine a plus node, so nothing to do.
                    return false;
                }

                if (prod_node.GetChild(index).Type == NodeTypes.Plus)
                    break;
            }

        }
    }
}
