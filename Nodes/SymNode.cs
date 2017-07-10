using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS.Nodes
{
    // List of all possible node types. Note that there are some intermediate helper subclasses
    // of SymNode that are not included.
    //
    // They are ordered approximately by operator precedence.
    //
    // The convention will be to examine nodes by getthing their Type, instead of using
    // the C# type system. I.e: if (node.Type == NodeTypes.Var) instead of node as Type
    //
    public enum NodeTypes
    {
        Var,
        Constant,
        Differential,
        Cos,
        Sin,
        Tan,
        Power,
        Prod,
        Wedge,
        Plus,
    };
    
    public abstract class SymNode
    {

        public abstract void Format(FormatBuilder fb);

        NodeTypes _Type;
        public NodeTypes Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        // Nodes can have children, and these methods provide a way to traverse them.
        // The order of the children does matter for some node types (e.g. wedge product)
        // A seperate a lexical order is used for printing.
        // The nodes should form a tree, so a child should never appear twice.
        //
        public virtual int ChildCount() { return 0; }
        public virtual SymNode GetChild(int index) { Debug.Assert(false); return null; }

        // Replace one node with another. If the replacement is null, the original is simply
        // removed.
        public virtual void ReplaceChild(SymNode existing_child, SymNode new_child) { Debug.Assert(false); }
        public bool HasChild(SymNode node)
        {
            for (int i = 0; i < ChildCount(); i++)
                if (GetChild(i) == node)
                    return true;
            return false;
        }
        // Perform a deep equality check BY VALUE: Returns true if this and its children have the same values
        // as other and its children. The node structure must match as well, so that  a*b*c != (a*b)*c.
        //
        public abstract bool IsEqual(SymNode other);

        // Reorder any nodes that can be reorded without affecting the meaning of the expression.
        // The function should recursively call its children and sort itself after:
        public virtual void Sort()
        {
            for (int i = 0; i < ChildCount(); i++)
                GetChild(i).Sort();
        }  

        // Create a recursive copy. The order of nodes must be preserved, even if the order doesn't matter:
        public abstract SymNode DeepClone();

        // Convert to a string without sorting:
        public override string ToString()
        {
            var sb = new FormatBuilder();
            Format(sb);
            return sb.ToString();
        }

        public string ToStringSorted()
        {
            Sort();
            return ToString();
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

        public string BreakStringSorted()
        {
            string s = "\t" + ToStringSorted();
            s = s.Replace("+", "\n\t+");
            s = s.Replace("-", "\n\t-");
            return s;
        }

        // Helper functions that can be used for simplification. These should be conserviative, i.e.,
        // IsZero returns true if the result is GURANTEED to be 0, it could still be 0 even if the
        // function returns false.
        public virtual bool IsZero() { return false; }
        public virtual bool IsOne() { return false; }

        // Return a new node representing this node if constant folding is allowed. This means
        // addition, multiplication, and division are allowed. For non-commuative nodes (e.g wedge),
        // this could also mean rordering into a standard order.
        // Acts recursively.
        public abstract SymNode FoldConstants();

        // Return a node repesenting this node (and its children) if numerial calculation is allowed.
        // For example, a cos node with a constant argument of .1 would return a constant node
        // of value 0.99999847691328769880290124792571.
        // If nummerical errors would occur (for example, divide by 0), the arguments are left alone.
        // Acts recursively.
        public abstract SymNode Evaluate();

        // Merge children together, or possibly into this node, and return a new node (with new children)
        // if any merging was done. Note that the type of node may change.
        // This is best done after constants have been folded.
        // Does NOT act recursively.
        public virtual SymNode Merge() { return null; }

        // Perform differntiation WRT to the supplied variable. Returns a new node:
        public abstract SymNode Differentiate(string var);

        // Compute the differntial WRT a series of variables given by name. Returns a new node.
        //      Differential( {"a","b"}, ab + b^2 ) --> b da + (a + 2b) db
        //
        public SymNode Differential(List<string> var_list)
        {
            var plus_node = new PlusNode();

            foreach(var name in var_list)
            {
                var diff = Differentiate(name);
                var node = new WedgeNode(diff, new DNode(name));
                plus_node.AddChild(node);
            }
            return plus_node;
        }

        // Does this node or any of its children have a differential (e.g. DNode) appearing as a linear term?
        // Note that non-linear combinations of differentials are not allowed. This will through an exception if 
        // such is found:
        public virtual bool HasDifferential() { return false; }

        // Check this node and its children for consistency:
        [Conditional("DEBUG")]
        public virtual void AssertValid()  { }

        // Make sure this tree doesn't have any doubly linked nodes:
        [Conditional("DEBUG")]
        public void CheckTree()
        {
            var ti = new TreeIterator(this);
            var hs = new HashSet<SymNode>();
            while (ti.Next())
            {
                Debug.Assert(!hs.Contains(ti.Cur));
                hs.Add(ti.Cur);
            }
        }

        // Make sure nodes aren't linked to two different trees:
        [Conditional("DEBUG")]
        public void CheckDisjoint(SymNode other)
        {
            var ti = new TreeIterator(this);
            var hs = new HashSet<SymNode>();
            while (ti.Next())
            {
                Debug.Assert(!hs.Contains(ti.Cur));
                hs.Add(ti.Cur);
            }
            ti = new TreeIterator(other);
            hs = new HashSet<SymNode>();
            while (ti.Next())
            {
                Debug.Assert(!hs.Contains(ti.Cur));
                hs.Add(ti.Cur);
            }
        }
    }
}
