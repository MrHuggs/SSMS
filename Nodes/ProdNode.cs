using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class ProdNode : CommutativeNode
    {
        public ProdNode()
        {
            Type = NodeTypes.Prod;
        }

        public ProdNode(params SymNode[] node_list) : this()
        {
            Debug.Assert(node_list.Count() > 1);  // Product node must have at least 2 nodes.
            foreach (var node in node_list)
                AddChild(node);
        }

        // Helper: Given a list of nodes, clone the nodes and create a product node for their
        // product, unless there is just one node, in which case just return the one.
        public static SymNode FromProdList<T> (List<T> list) where T : SymNode
        {
            Debug.Assert(list.Count > 0);
            if (list.Count == 1)
                return list[0].DeepClone();
            ProdNode result = new ProdNode();
            list.ForEach(node => result.AddChild(node.DeepClone()));
            return result;
        }

        public override void Format(FormatBuilder fb)
        {
            // The sort process puts constants behind variables. However, we normally write the constant term
            // in front of variables. Handle this by writing our our children in two passes. First write
            // out constants, then other children.
            //
            // This we get 10*a instead of a*10
            //
            int cnt = 0;
            foreach(var node in Children)
            {
                if (node.Type != NodeTypes.Constant)
                    continue;
                if (cnt++ > 0)
                    fb.Append('*');
                fb.Append(node.ToString());
            }

            foreach (var node in Children)
            {
                if (node.Type == NodeTypes.Constant)
                    continue;

                if (cnt++ > 0)
                    fb.Append('*');

                // Decide if the child needs to be parenthesized. We'll just use the
                // operator precendece. This does mean you could see results like
                // -34*-32.
                bool need_paren;
                if (node.Type >= NodeTypes.Prod)
                {
                    need_paren = true;  // Child has lower precedence.
                }
                else need_paren = false;
                fb.Append(node.ToString(), need_paren);
            }       
        }


        public override SymNode DeepClone()
        {
            var n = new ProdNode();
            n.DeepCloneChildren(this);

            return n;
        }

        public override bool IsZero()
        {
            foreach (var child in Children)
            {
                if (child.IsZero())
                    return true;
            }
            return false;
        }
        public override bool IsOne()
        {
            if (Children.Count == 0)
                return false;

            foreach (var child in Children)
            {
                if (!child.IsOne())
                {
                    return false;
                }
            }
            return true;
        }

        public override SymNode FoldConstants()
        {
            var new_node = new ProdNode();

            double product = 1;
            foreach(var node in Children)
            {
                var new_child = node.FoldConstants();
                if (new_child.IsZero())
                    return new ConstNode(0);
                if (new_child.IsOne())
                    continue;

                if (new_child.Type == NodeTypes.Constant)
                {
                    product *= ((ConstNode)new_child).Value;
                }
                else
                {
                    new_node.AddChild(new_child);
                }
            }

            if (new_node.Children.Count == 0)
                return new ConstNode(product);

            if (product != 1)
                new_node.AddChild(new ConstNode(product));

            if (new_node.Children.Count == 1)
            {
                // A product node should have at least two operands:
                return new_node.Children[0];
            }

            return new_node;
        }


        public override SymNode Evaluate()
        {
            var new_node = new ProdNode();

            foreach(var node in Children)
            {
                new_node.AddChild(node.Evaluate());
            }

            return new_node.FoldConstants();
        }

        class MergePair
        {
            public MergePair(SymNode node)
            {
                if (node.Type == NodeTypes.Power)
                {
                    Base = ((PowerNode)node).Base;
                    ExponentNodes.Add( ((PowerNode)node).Exponent);
                }
                else
                {
                    Base = node;
                    IntPower = 1;
                }
            }
            public SymNode Base;
            public int IntPower = 0;
            public List<SymNode> ExponentNodes = new List<SymNode>();

            public bool AttemptMerge(SymNode other)
            {
                if (other.Type == NodeTypes.Power)
                {
                    PowerNode op = (PowerNode)other;

                    if (op.Base.IsEqual(Base))
                    {
                        ExponentNodes.Add(op.Exponent);
                        return true;
                    }
                }
                else if (other.IsEqual(Base))
                {
                    IntPower++;
                    return true;
                }

                return false;
            }

            public SymNode CreateNode()
            {
                if (ExponentNodes.Count == 0)
                {
                    Debug.Assert(IntPower > 0);
                    return (IntPower == 1) ?  
                                    Base.DeepClone() : 
                                    new PowerNode(Base.DeepClone(), new ConstNode((double)IntPower));
                }

                if (IntPower > 0)
                    ExponentNodes.Add(new ConstNode((double)IntPower));

                return new PowerNode(Base.DeepClone(), PlusNode.FromPlusList(ExponentNodes));
            }
        };
        
        public SymNode MergeChildrenTogether()
        {
            // Merge children together that have a the same base: e.g. a^2*a^(x+2) -> a^{x+4})
            var pairs = new List<MergePair>();
            bool merged = false;        // Did any merging occur.

            foreach (var v in Children)
            {
                
                for (int i = 0; ; i++)
                {
                    if (i == pairs.Count)
                    {
                        pairs.Add(new MergePair(v));
                        break;
                    }

                    if (pairs[i].AttemptMerge(v))
                    {
                        merged = true;
                        break;
                    }
                }
            }

            if (!merged)
                return null;

            if (pairs.Count == 1)
                return pairs[0].CreateNode();

            ProdNode result = new ProdNode();
            pairs.ForEach(pair => result.AddChild(pair.CreateNode()));

            result.AssertValid();
            return result;
        }


        public override SymNode Merge()
        {
            ProdNode r1 = (ProdNode) MergeChildrenUp();

            if (r1 != null)
            {
                var merge_childen = r1.MergeChildrenTogether();
                return (merge_childen != null) ? merge_childen : r1;
            }

            return MergeChildrenTogether();
        }

        public override SymNode Differentiate(string var)
        {
            PlusNode result = new PlusNode();

            for (int i = 0; i < Children.Count; i++)
            {
                SymNode dif = Children[i].Differentiate(var);
                ProdNode prod = new ProdNode();

                prod.AddChild(dif);
                for (int j = 0; j < Children.Count; j++)
                {
                    if (j == i)
                        continue;
                    prod.AddChild(Children[j].DeepClone());
                }

                result.AddChild(prod);
            }
            return result;
        }

        public override void AssertValid()
        {
            base.AssertValid();
            int cnt = 0;
            foreach (var v in Children)
            {
                if (v.HasDifferential())
                    cnt++;
            }
            // We cannot have powers of differentials.
            Debug.Assert(cnt <= 1);
        }

    }
}
