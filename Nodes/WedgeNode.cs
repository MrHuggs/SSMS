using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class WedgeNode : CommutativeNode
    {
        public WedgeNode()
        {
            Type = NodeTypes.Wedge;
        }

        public WedgeNode(params SymNode[] node_list) : this()
        {
            Debug.Assert(node_list.Count() > 1);  // Product node must have at least 2 nodes.
            foreach (var node in node_list)
                AddChild(node);
        }

        public override void Format(FormatBuilder fb)
        {
            // Sorting is not allowed, so we just write elements in order.
            //
            // Have to pick a symbol for wedge product. We can't use caret (^) because that is 
            // for exponentiation. I'll use $.
            //
            

            int cnt = 0;

            foreach (var node in Children)
            {
                if (node.Type == NodeTypes.Constant)
                    continue;

                if (cnt++ > 0)
                    fb.Append('$');

                // Decide if the child needs to be parenthesized. We'll just use the
                // operator precendece. This does mean you could see results like
                // -34*-32.
                bool need_paren;
                if (node.Type >= NodeTypes.Wedge)
                {
                    need_paren = true;  // Child has lower precedence.
                }
                else need_paren = false;
                fb.Append(node.ToString(), need_paren);
            }
        }


        public override SymNode DeepClone()
        {
            var n = new WedgeNode();
            n.DeepCloneChildren(this);

            return n;
        }

        public override bool IsZero()
        {
            var diffs = new List<DNode>();
            foreach (var child in Children)
            {
                if (child.IsZero())
                    return true;

                if (child.Type == NodeTypes.Differential)
                {
                    var dnode = (DNode)child;
                    foreach(var od in diffs)
                    {
                        if (od.IsEqual(dnode))
                        {
                            // A wedge proudct with repeated differentials is 0:
                            return true;
                        }
                    }
                    diffs.Add(dnode);
                }
            }
            return false;
        }

        public override SymNode FoldConstants()
        {
            // If this node consists entirely of DNoodes, then put in standard
            // order.
            
            var new_node = new WedgeNode();

            bool all_differentials = true;
            foreach (var node in Children)
            {
                var new_child = node.FoldConstants();

                if (new_child.IsZero())
                    return new ConstNode(0);

                if (new_child.IsOne())
                    continue;

                new_node.AddChild(new_child);
                if (new_child.Type != NodeTypes.Differential)
                    all_differentials = false;
            }


            if (all_differentials)
            {
                if (new_node.IsZero())
                    return new ConstNode(0);

                int sgn = new_node.SortDNodes();
                if (sgn != 1)
                {
                    return new ProdNode(new ConstNode(sgn), new_node);
                }
            }

            return new_node;
        }


        public override SymNode Evaluate()
        {
            var new_node = new WedgeNode();

            foreach (var node in Children)
                new_node.AddChild(node.Evaluate());

            return new_node.FoldConstants();
        }

        // See if a node is a product node with a single differential factor.
        bool FactorProdNode(SymNode node, ref ProdNode prod, out DNode dnode)
        {
            dnode = null;

            if (node.Type != NodeTypes.Prod)
                return false;

            var parent = (ProdNode)node;
            foreach(var child in parent.Children)
            {
                if (child.Type == NodeTypes.Differential)
                {
                    if (dnode != null)
                    {
                        throw new ApplicationException(
                            string.Format("Expression {0} has multiple differential terms multiplied together.", parent.ToString())
                            );
                    }
                    dnode = (DNode) child.DeepClone();
                }
            }

            if (dnode == null)
            {
                // If we haven't found a DNode at this point, factorization is not possible:
                return false;
            }

            foreach (var child in parent.Children)
            {
                if (child.Type != NodeTypes.Differential)
                    prod.AddChild(child.DeepClone());
            }
            return true;
        }

        public int SortDNodes()
        {
            // If this node has all DNode terms, sort the DNodes and return sign of
            // the required permuation.
            //
            // Since we expect the number of dnodes to be small, we will just do a bubble sort.
            //
            int sgn = 1;
            bool swapped;
            do
            {
                swapped = false;

                for (int j = 1; j < Children.Count; j++)
                {
                    DNode cur = (DNode)Children[j];
                    DNode prev = (DNode)Children[j-1];

                    if (cur.Var.CompareTo(prev.Var) < 0)
                    {
                        sgn *= -1;
                        Children[j] = Children[j - 1];
                        Children[j - 1] = cur;

                        swapped = true;
                    }
                }
            }
            while (swapped);
            return sgn;
        }


        public override SymNode Merge()
        {
            // Attempt to pull out DNodes (differentials) from our children.
            var wedge = new WedgeNode();
            var prod = new ProdNode();

            bool factored = true;
            foreach(var child in Children)
            {
                DNode dnode;
                if (FactorProdNode(child, ref prod, out dnode))
                {
                    wedge.AddChild(dnode);
                }
                else
                {
                    factored = false;
                    wedge.AddChild(child);
                }
            }

            if (factored == true)
            {
                int sgn = wedge.SortDNodes();
                if (sgn != 1)
                    prod.AddChild(new ConstNode(sgn));
            }

            if (prod.ChildCount() > 0)
            {
                prod.AddChild(wedge);
                prod.AssertValid();
                return prod;
            }
            wedge.AssertValid();
            return wedge;
        }

        public override SymNode Differentiate(string var)
        {
            throw new ApplicationException("Cannot differentiate a wedge product.");
        }

    }
}
