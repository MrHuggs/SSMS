using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class WedgeNode : ChildListNode
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
                    fb.Append(@"/\");

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

            if (new_node.IsZero())
                return new ConstNode(0);

            if (new_node.Children.Count == 1)
                return new_node.Children[0];

            if (all_differentials)
            {
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

        void FactorWedges(SymNode child, List<SymNode> non_wedge, List<SymNode> wedge)
        {

            if (child.Type == NodeTypes.Wedge)
            {
                var wedge_node = (WedgeNode)child;
                foreach (var v in wedge_node.Children)
                {
                    if (v.HasDifferential())
                        wedge.Add(v);
                    else
                        non_wedge.Add(v);
                }
                return;
            }

            if (child.HasDifferential())
                wedge.Add(child);
            else
                non_wedge.Add(child);
        }

        void FactorChild(SymNode child, List<SymNode> non_wedge, List<SymNode> wedge)
        {
            if (child.Type == NodeTypes.Prod)
            {
                var prod = (ProdNode)child;
                foreach (var v in prod.Children)
                {
                    FactorWedges(v, non_wedge, wedge);
                }
                return;
            }
            FactorWedges(child, non_wedge, wedge);
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
            var factors = new List<SymNode>();
            var wedges = new List<SymNode>();

            foreach (var v in Children)
                FactorChild(v, factors, wedges);

            SymNode new_wedges;

            if (wedges.Count > 1)
            {
                var new_wedge = new WedgeNode();

                foreach (var v in wedges)
                    new_wedge.AddChild(v.DeepClone());

                new_wedges = new_wedge;
            }
            else if (wedges.Count == 1)
            {
                new_wedges = wedges[0].DeepClone();
            }
            else
                new_wedges = null;

            if (factors.Count == 0)
            {
                if (new_wedges.IsEqual(this))
                {
                    return null;    // It's possible no actual merging occured.
                }
                return new_wedges;
            }

            var prod = new ProdNode();
            foreach(var v in factors)
                prod.AddChild(v.DeepClone());

            prod.AddChild(new_wedges); // Know it's different at this point: A wedge node became a prod node.
            return prod;
        }

        public override SymNode Differentiate(string var)
        {
            throw new ApplicationException("Cannot differentiate a wedge product.");
        }
    }
}
