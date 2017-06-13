using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
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
        public SymNode Parent { get; private set; }

        public bool Next()
        {
            if (Stack.Count == 0)
            {
                Cur = null;
                Parent = null;
                return false;
            }

            while (true)
            {

                SymNode result;
                StackEntry top = Stack.Last();

                if (top.ChildIndex == top.Node.ChildCount())
                {
                    result = top.Node;
                    Stack.Remove(top);
                    Cur = result;

                    int cnt = Stack.Count;
                    if (cnt == 0)
                        Parent = null;
                    else
                        Parent = Stack[cnt - 1].Node;

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
}
