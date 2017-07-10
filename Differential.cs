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

            return Compute(root, var_list);
        }

        public static SymNode Compute(SymNode root, List<string> var_list)
        {
            if (var_list.Count == 0)
                return new ConstNode(0);

            var plus = new PlusNode();

            foreach (var s in var_list)
            {
                var diff = root.Differentiate(s);
                plus.AddChild(new ProdNode(diff, new DNode(s)));
            }

            var result = (plus.Children.Count > 1) ? plus : plus.Children[0];
            var simple = TransformsList.Inst().TrySimplify(result);
            simple.Sort();

            return simple;
        }

        static bool IsBareWegeNode(SymNode node)
        {
            if (node.Type == NodeTypes.Differential)
                return true;

            if (node.Type != NodeTypes.Wedge)
                return false;

            var wedge_node = (WedgeNode)node;
            foreach (var child in wedge_node.Children)
            {
                if (child.Type != NodeTypes.Differential)
                    return false;
            }
            return true;
        }

        // Take the exterior derivative of a node of the form (stuff) * wedge product * (more stuff)
        public static SymNode ExteriorDerivative_Prod(ProdNode source, List<String> var_list)
        {
            Debug.Assert(source.Type == NodeTypes.Prod);

            var factors = new List<SymNode>();

            SymNode wedge = null;

            for (int i = 0; i < source.Children.Count; i++)
            {
                var child = source.Children[i];

                if (IsBareWegeNode(child))
                {
                    if (wedge != null)
                        throw new ApplicationException("Can not take exterior derivative of an expression with muiltple wedge products.");

                    wedge = child;
                    continue;
                }

                if (child.HasDifferential())
                    throw new ApplicationException("To take the exterior derivative, all differentials must appear in a pure wedge proudct.");

                factors.Add(child);
            }

            Debug.Assert(factors.Count > 0);
            SymNode differential;
            if (factors.Count == 1)
            {
                differential = Compute(factors[0], var_list);
            }
            else
            {
                var prod = new ProdNode();
                prod.Children = factors;
                differential = Compute(prod, var_list);
            }

            var result = new WedgeNode(differential, wedge.DeepClone());
            return result;
        }

        public static SymNode ExteriorDerivative_Plus(PlusNode source, List<String> var_list)
        {
            var result = new PlusNode();

            foreach (var child in source.Children)
            {
                SymNode new_child;
                if (!child.HasDifferential())
                {
                    new_child = Compute(child, var_list);
                    result.AddChild(new_child);
                    continue;
                }

                switch (child.Type)
                {
                    case NodeTypes.Prod:
                        new_child = ExteriorDerivative_Prod((ProdNode)child, var_list);
                        result.AddChild(new_child);
                        break;
                    case NodeTypes.Differential:
                        // Result is 0, so don't do anything:
                        break;
                    case NodeTypes.Wedge:
                        if (!IsBareWegeNode(child))
                            throw new ApplicationException("To take the exterior derivative, all wedge products must be of differentials only.");
                        break;
                    case NodeTypes.Plus:
                        // This should not happen if the starting node was properly simplified. But, it is
                        // easy to handle:
                        new_child = ExteriorDerivative_Plus((PlusNode)child, var_list);
                        result.AddChild(new_child);
                        break;
                    default:
                        throw new ApplicationException("Node type is unsupported for exterior derivative.");
                }
            }

            if (result.Children.Count == 0)
                return new ConstNode(0);

            return (result.Children.Count > 1) ? result : result.Children[0];
        }

        public static SymNode ExteriorDerivative(SymNode root)
        {
            var var_list = FindVariables(root);

            return ExteriorDerivative(root, var_list);
        }

        public static SymNode ExteriorDerivative(SymNode root, List<String> var_list)
        {
            SymNode result;

            if (!root.HasDifferential())
                return Compute(root, var_list);

            switch (root.Type)
            {
                case NodeTypes.Prod:
                    result = ExteriorDerivative_Prod((ProdNode)root, var_list);
                    break;
                case NodeTypes.Differential:
                    result = new ConstNode(0);
                    break;
                case NodeTypes.Wedge:       // Fall through
                    if (!IsBareWegeNode(root))
                        throw new ApplicationException("To take the exterior derivative, all wedge products must be of differentials only.");
                    result = new ConstNode(0);
                    break;
                case NodeTypes.Plus:
                    result = ExteriorDerivative_Plus((PlusNode)root, var_list);
                    break;
                default:
                    throw new ApplicationException("Root node type is unsupported for exterior derivative.");
            }

            result.AssertValid();
            result.CheckDisjoint(root);
            return result;
        }

    }
}
