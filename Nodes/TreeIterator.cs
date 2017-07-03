using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace SSMS.Nodes
{

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

        public SymNode Cur { get; private set; }
        public SymNode Parent()
        {
            if (Stack.Count < 1)
                return null;
            return Stack[Stack.Count - 1].Node;
        }

        public bool Next()
        {
            if (Stack.Count == 0)
            {
                Cur = null;
                return false;
            }

            while (true)
            {

                StackEntry top = Stack.Last();

                if (top.ChildIndex == top.Node.ChildCount())
                {
                    Stack.Remove(top);
                    Cur = top.Node;

                    return true;
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

    public class TreeIteratorPre
    {
        // Iterate the the nodes of SymNode tree in Depth First, pre-ord
        // https://en.wikipedia.org/wiki/Tree_traversal#Post-order
        //

        public TreeIteratorPre(SymNode root)
        {
            StackEntry entry = new StackEntry();
            entry.Node = root;
            entry.ChildIndex = -1;
            Stack.Add(entry);
        }

        class StackEntry
        {
            public SymNode Node;
            public int ChildIndex;
        };

        List<StackEntry> Stack = new List<StackEntry>();

        public SymNode Cur { get; private set; }
        public SymNode Parent()
        {
            if (Stack.Count < 2)
                return null;
            return Stack[Stack.Count - 2].Node;
        }

        public bool Next()
        {
            while (true)
            {
                if (Stack.Count == 0)
                {
                    Cur = null;
                    return false;
                }

                StackEntry top = Stack.Last();

                if (top.ChildIndex == -1)
                {
                    Cur = top.Node;
                    top.ChildIndex++;
                    return true;
                }

                if (top.ChildIndex == top.Node.ChildCount())
                {
                    Stack.Remove(top);
                }
                else
                {
                    var new_top = new StackEntry();

                    new_top.Node = top.Node.GetChild(top.ChildIndex);
                    new_top.ChildIndex = -1;
                    Stack.Add(new_top);

                    top.ChildIndex++;
                }
            }
        }
    }

}
