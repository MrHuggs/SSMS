using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    enum NodeTypes
    {
        Constant,
        Var,
        Power,
        Cos,
        Sin,
        Tan,
        Sum,        
        Div,
        Prod
    };

    abstract class SymNode
    {
        public abstract void Format(StringBuilder sb);

        public abstract bool IsEqual(SymNode other);

        NodeTypes _Type;
        public NodeTypes Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        // Nodes can have children.
        public virtual int ChildCount() { return 0;  }
        public virtual SymNode GetChild(int idx)
        {
            Debug.Assert(false); return null;
        }
        
        abstract public int LexicalCompare(SymNode b);

    }
}
