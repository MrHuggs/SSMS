using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace SSMS
{
    public abstract class FunctionNode : ChildListNode
    {

        public override void AddChild(SymNode child)
        {
            Debug.Assert(!Children.Contains(child));

            Children.Add(child);
        }

        override public NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type);
        }
    }
}
