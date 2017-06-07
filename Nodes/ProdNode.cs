using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ProdNode : CommutativeNode
    {
        public ProdNode()
        {
            Type = NodeTypes.Prod;
        }

        public override void Format(FormatBuilder fb)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                var node = Children[i];

                bool need_paren;

                if (node.Type > NodeTypes.Prod && node.ChildCount() > 1)
                    need_paren = true;
                if (node.Type == NodeTypes.Constant && i > 0)
                    need_paren = true;
                else need_paren = false;

                if (need_paren)
                {
                    // If its lower precedence, then surround it by ():
                    fb.Append("(");
                    node.Format(fb);
                    fb.Append(")");
                }
                else
                {
                    node.Format(fb);
                }
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

            var const_node = new ConstNode(product);

            if (new_node.Children.Count == 0)
                return const_node;

            new_node.AddChild(const_node);
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

    }
}
