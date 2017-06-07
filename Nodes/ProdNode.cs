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

        public override void Format(StringBuilder sb)
        {
            bool first = true;
            foreach (var node in Children)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");

                if (node.Type > NodeTypes.Prod)
                {
                    // If its lower precedence, then surround it by ():
                    sb.Append("(");
                    node.Format(sb);
                    sb.Append(")");
                }
                else
                {
                    node.Format(sb);
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
