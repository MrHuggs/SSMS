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

    struct NodeSortVal
    {
        public NodeSortVal(NodeTypes type)
        {
            Type = type;
            Identifier = null;
            Value = 0;
        }
        public NodeSortVal(NodeTypes type, double value)
        {
            Type = type;
            Identifier = null;
            Value = value;
        }
        public NodeSortVal(NodeTypes type, string identifier)
        {
            Type = type;
            Identifier = identifier;
            Value = 0;
        }


        NodeTypes Type;
        String Identifier;
        double Value;

        // Compare to sort values. This uses the C# convention:
        // If a should come before b, return < 0.
        public static int Compare(NodeSortVal a, NodeSortVal b)
        {
            if (a.Type != b.Type)
            {
                return a.Type - b.Type;
            }

            if (a.Type == NodeTypes.Var)
            {
                if (a.Identifier == null)
                {
                    if (b.Identifier == null) return 0;
                    return -1;

                }
                if (b.Identifier == null)
                    return 1;

                return a.Identifier.CompareTo(b.Identifier);
            }
            Debug.Assert(a.Type == NodeTypes.Constant);

            double del = a.Value - b.Value;
            if (del < 0) return -1;
            if (del > 0) return 1;
            return 0;
        }
    }

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

        // Compare another node for sorting. Return > 0 if this node should
        // come before the other.
        public virtual NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type);
        }

        public abstract SymNode DeepCopy();

    }
}
