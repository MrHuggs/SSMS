using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    public class PlusNode : CommutativeNode
    {
        public PlusNode()
        {
            Type = NodeTypes.Plus;
        }

        public override void Format(FormatBuilder fb)
        {
            FormatBuilder child_builder = new FormatBuilder();
            foreach (var node in Children)
            {
                fb.Append('+');
                node.Format(child_builder);
                string child_string = child_builder.ToString();

                fb.Append(child_string);
                child_builder.Clear();
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

            if (sum == 0)
            {
                return new_node;
            }

            new_node.AddChild(new ConstNode(sum));
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
    }

}
