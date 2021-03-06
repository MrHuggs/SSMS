﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SSMS.Nodes
{
    public class PlusNode : CommutativeNode
    {
        public PlusNode()
        {
            Type = NodeTypes.Plus;
        }

        public PlusNode(params SymNode[] node_list) : this()
        {
            Debug.Assert(node_list.Count() > 1);  // Plus node must have at least 2 nodes.
            foreach (var node in node_list)
                AddChild(node);
        }

        // Helper: Given a list of nodes, clone the nodes and crate a plus node for their
        // sum, unless there is just one node, in which case just return the one.
        public static SymNode FromPlusList<T>(List<T> list) where T : SymNode
        {
            Debug.Assert(list.Count > 0);
            if (list.Count == 1)
                return list[0].DeepClone();
            PlusNode result = new PlusNode();
            list.ForEach(node => result.AddChild(node.DeepClone()));
            return result;
        }

        public override void Format(FormatBuilder fb)
        {
            // The sort process puts constants behind variables. However, we normally write the constant term
            // in front of variables. Handle this by writing our our children in two passes. First write
            // out constants, then other children.
            //
            // This we get 10+a instead of a+10
            //
            int cnt = 0;
            foreach (var node in Children)
            {
                if (node.Type != NodeTypes.Constant)
                    continue;
                if (cnt++ > 0)
                    fb.Append('+');
                fb.Append(node.ToString());
            }

            foreach (var node in Children)
            {
                if (node.Type == NodeTypes.Constant)
                    continue;

                if (cnt++ > 0)
                    fb.Append('+');
                fb.Append(node.ToString());
            }
        }


        public override SymNode DeepClone()
        {
            var n = new PlusNode();
            n.DeepCloneChildren(this);

            return n;
        }

        public override bool IsZero()
        {
            foreach (var child in Children)
            {
                if (!child.IsZero())
                    return false;
            }
            return true;
        }
        public override bool IsOne()
        {
            int one_count = 0;
            int non_zero_count = 0;
            foreach (var child in Children)
            {

                if (child.IsOne())
                    one_count++;
                else if (!child.IsZero())
                    non_zero_count++;
            }
            return one_count == 1 && non_zero_count == 0;
        }

        public override SymNode FoldConstants()
        {
            var new_node = new PlusNode();

            double sum = 0;
            foreach (var node in Children)
            {
                var new_child = node.FoldConstants();
                if (new_child.IsZero())
                    continue;

                if (new_child.Type == NodeTypes.Constant)
                {
                    sum += ((ConstNode)new_child).Value;
                }
                else
                {
                    new_node.AddChild(new_child);
                }
            }

            if (new_node.Children.Count == 0)
                return new ConstNode(sum);

            if (sum != 0)
                new_node.AddChild(new ConstNode(sum));

            if (new_node.Children.Count == 1)
            {
                // A plus node should have at least two operands:
                return new_node.Children[0];
            }

            return new_node;
        }


        public override SymNode Evaluate()
        {
            var new_node = new PlusNode();

            foreach (var node in Children)
            {
                new_node.AddChild(node.Evaluate());
            }

            return new_node.FoldConstants();
        }
        
        class MergePair
        {
            public MergePair()
            {
               
            }
            public double Sum;
            public List<SymNode> Nodes = new List<SymNode>();

            public bool AttemptMerge(MergePair other)
            {
                if (!Nodes.IsEqualReorder(other.Nodes))
                    return false;

                Sum += other.Sum;

                return true;
            }

            public SymNode CreatNode()
            {
                if (Sum == 0)
                    return null;
                if (Sum == 1 && Nodes.Count() == 1)
                    return Nodes[0];

                var prod = new ProdNode();
                prod.AddChild(new ConstNode(Sum));
                Nodes.ForEach(node => prod.AddChild(node.DeepClone()));

                return prod;
            }
        };

        SymNode MergeChildrenTogether()
        {
            // Merge children that differ only by a constant prefix.
            // e.g. 10*a+34*a = 34a
            var pairs = new List<MergePair>();
            bool merged = false;

            foreach (var v in Children)
            {
                MergePair cur_pair = new MergePair();
                if (v.Type == NodeTypes.Prod)
                {
                    ProdNode prod = (ProdNode)v;
                    Debug.Assert(prod.ChildCount() > 0);

                    var split = prod.SplitConstChild();
                    cur_pair.Sum = (split.Constant != null) ? split.Constant.Value : 1;
                    if (split.Others != null) cur_pair.Nodes = split.Others;
                }
                else
                {
                    cur_pair.Sum = 1;
                    cur_pair.Nodes.Add(v);
                }

                for (int i = 0; ; i++)
                {
                    if (i == pairs.Count)
                    {
                        pairs.Add(cur_pair);
                        break;
                    }

                    if (pairs[i].AttemptMerge(cur_pair))
                    {
                        merged = true;
                        break;
                    }
                }
            }

            if (!merged)
                return null;

            PlusNode result = new PlusNode();
            foreach (var p in pairs)
            {
                var new_node = p.CreatNode();
                if (new_node == null)
                    continue;
                result.AddChild(new_node);
            }

            if (result.ChildCount() == 1)
                return result.GetChild(0);

            if (result.ChildCount() == 0)
                return new ConstNode(0);


            result.AssertValid();
            return result;
        }


        public override SymNode Merge()
        {
            PlusNode r1 = (PlusNode) MergeChildrenUp();

            if (r1 != null)
            {
                var merge_childen = r1.MergeChildrenTogether();
                return (merge_childen != null) ? merge_childen : r1;
            }

            return MergeChildrenTogether();
        }

        public override SymNode Differentiate(string var)
        {
            PlusNode new_plus = new PlusNode();

            foreach(var v in Children)
            {
                var dv = v.Differentiate(var);
                new_plus.AddChild(dv);
            }

            return new_plus;
        }


    }

}
