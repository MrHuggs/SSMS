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

            for (i = 0; i < Children.Count; i++)
            {
                if (child.LexicalCompare(Children[i]) > 0)
                    break;
            }
            Children.Insert(i, child);
        }

        public void RemoveChild(SymNode node)
        {
            Children.Remove(node);
        }

        // Compare for lexcial and presentation purposes. Should be stable.
        public override int LexicalCompare(SymNode other)
        {
            if (other.Type < NodeTypes.Var)
            {
                return -1;
            }

            if (other.Type > NodeTypes.Var)
            {
                return 1;
            }

            var mycount = Children.Count;


            ProdNode pnode = (ProdNode)other;
            var ocount = pnode.Children.Count;

            if (mycount == 0)
                return mycount - ocount;

            if (ocount == 0)
                return 1;

            return Children[0].LexicalCompare(pnode.Children[0]);            
        }


        public override void Format(StringBuilder sb)
        {
            foreach (var node in Children)
            {
                node.Format(sb);
            }
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
    }
}
