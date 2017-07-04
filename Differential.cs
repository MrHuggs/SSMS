using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using SSMS.Nodes;

namespace SSMS
{
    public class Differential
    {
        public static List<String> FindVariables(SymNode root)
        {
            var it = new TreeIterator(root);
            var result = new List<string>();

            while (it.Next())
            {
                if (it.Cur.Type == NodeTypes.Var)
                {
                    var var_node = (VarNode)it.Cur;
                    if (!result.Contains(var_node.Var))
                        result.Add(var_node.Var);
                }
            }
            return result;
        }

        public static SymNode Compute(SymNode root)
        {
            var var_list = FindVariables(root);

            if (var_list.Count == 0)
                return new ConstNode(0);

            var plus = new PlusNode();

            foreach(var s in var_list)
            {
                var diff = root.Differentiate(s);
                plus.AddChild(new ProdNode(diff, new DNode(s)));
            }

            var result = (plus.ChildCount() > 1) ? plus : plus.Children[0];
            var simple = TransformsList.Inst().TrySimplify(result);
            simple.Sort();

            return simple;
        }
    }
}
