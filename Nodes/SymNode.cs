using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    // List of all possible node types. Node there there are some intermediate helper subclasses
    // of SymNode that are not included.
    //
    // They are ordere by operation precedence.
    //
    // The convenetion will be to examine nodes by getthing their Type, instead of using
    // the C# type systme. I.e: if (node.Type == NodeTypes.Var)
    public enum NodeTypes
    {
        Constant,
        Var,
        Power,
        Cos,
        Sin,
        Tan,
        Div,
        Prod,
        Plus,
    };

    public struct NodeSortVal
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

    public abstract class SymNode
    {

        public abstract void Format(FormatBuilder fb);

        NodeTypes _Type;
        public NodeTypes Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        // Nodes can have children, and these method provide a way to traverse them.
        // Note that the order of the children depends on the type of node and lexial
        // sorting of a the nodes. 
        // The nodes should form a tree, so a child should never be added twice.
        public virtual int ChildCount() { return 0; }
        public virtual SymNode GetChild(int index) { Debug.Assert(false); return null; }
        // Replace one node with another. If the replacement is null, the original is simply
        // removed. If you replace a node with another, it may get sorted to a different spot:
        public virtual void ReplaceChild(SymNode existing_child, SymNode new_child) { Debug.Assert(false); }

        // Perforrm a deep equals check BY VALUE: Returns true if this and its children have the same values
        // as other and its children.
        public abstract bool IsEqual(SymNode other);


        // Compare another node for sorting. Return > 0 if this node should
        // come before the other.
        public virtual NodeSortVal GetSortVal()
        {
            return new NodeSortVal(Type);
        }

        public abstract SymNode DeepClone();

        public override string ToString()
        {
            var sb = new FormatBuilder();
            Format(sb);
            return sb.ToString();
        }

        public void Print()
        {
            Console.WriteLine(ToString());
        }

        public void PrintValue()
        {
            var node = Evaluate();
            node.Print();
        }

        // Helper functions that can be used for simplification. These should be conserviative, i.e.,
        // IsZero returns true if the result is GURANTEED to be 0, it could still be 0 even if the
        // function returns false.
        public virtual bool IsZero() { return false; }
        public virtual bool IsOne() { return false; }

        // Return a node representing this node if constant folding is allows. This means
        // addition, multiplication, and division are allowed.
        public abstract SymNode FoldConstants();

        // Return a node repesenting this node (and it's children) if numerial calculation is performed.
        // So, for example, a cos node with a constant argument of .1 would return a constant node
        // of value 0.99999847691328769880290124792571.
        // If nummerical errors would occur (for example, divide by 0), the arguments are left alone.
        public abstract SymNode Evaluate();
    
    }


    public class TreeIterator
    {
        // Iterate the the nodes of SymNode tree in Depth First, post-order
        // https://en.wikipedia.org/wiki/Tree_traversal#Post-order
        //


        public TreeIterator(SymNode root)
        {
            StackEntry entry = new StackEntry();
            entry.Node = root;
            entry.ChildIndex = 0;
            Stack.Add(entry);
        }

        class StackEntry
        {
            public SymNode Node;
            public int ChildIndex;
        };

        List<StackEntry> Stack = new List<StackEntry>();

        public SymNode Next()
        {
            if (Stack.Count == 0)
                return null;

            while (true)
            {

                SymNode result;
                StackEntry top = Stack.Last();

                if (top.ChildIndex == top.Node.ChildCount())
                {
                    result = top.Node;
                    Stack.Remove(top);
                    return result;
                }
                else
                {
                    var new_top = new StackEntry();
                    new_top.Node = top.Node.GetChild(top.ChildIndex);
                    new_top.ChildIndex = 0;
                    Stack.Add(new_top);

                    top.ChildIndex++;
                }
            }
        }
    }
}
