using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    abstract class FunctionNode : ChildListNode
    {

        public override void AddChild(SymNode child)
        {
            Children.Add(child);
        }

        public void RemoveChild(SymNode node)
        {
            Children.Remove(node);
        }

        override public NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type);
        }
    }
}
