using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ProdNode : SymNode
    {
        List<SymNode> Children = new List<SymNode>();

        public ProdNode()
        {
           Type = NodeTypes.Prod;
        }


        public override int ChildCount() { return Children.Count;  }
        public override SymNode GetChild(int idx) { return Children[idx];  }

        public void AddChild(SymNode child)
        {
            int i = 0;

            var sort_val = child.GetSortVal();

            for (i = 0; i < Children.Count; i++)
            {
                var child_val = Children[i].GetSortVal();
                if (NodeSortVal.Compare(sort_val, child_val) < 0)
                    break;
            }
            Children.Insert(i, child);
        }

        public void RemoveChild(SymNode node)
        {
            Children.Remove(node);
        }

        override public NodeSortVal GetSortVal()
        {
            if (Children.Count == 0)
                return new NodeSortVal(Type);


            // We should be sorted based on our first no-constant node.
            // So, for example, 4 * 3 * x * y is sorted based on x.

            for (int i = 0; i < Children.Count; i++)
            {
                if (Children[i].Type != NodeTypes.Constant)
                    return Children[i].GetSortVal();
            }

            return Children[0].GetSortVal();

        }

        public override void Format(StringBuilder sb)
        {
            sb.Append("(");
            bool first = true;
            foreach (var node in Children)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");

                node.Format(sb);
            }
            sb.Append(")");
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            ProdNode pnode = (ProdNode)other;
            var ocount = pnode.Children.Count;

            if (ocount != Children.Count)
                return false;

            for (int i = 0; i < ocount; i++)
            {
                if (!Children[i].IsEqual(pnode.Children[i]))
                    return false;
            }
            return true;
        }

        public override SymNode DeepCopy()
        {
            var n = new ProdNode();

            for (int i = 0; i < Children.Count; i++)
            {
                n.Children.Add(Children[i].DeepCopy());
            }
            return n;
        }



    }
}
