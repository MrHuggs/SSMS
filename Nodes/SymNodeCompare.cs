using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS.Nodes
{
    // The compare function used to sort nodes when we convert them to a string.
    //
    class SymNodeCompare
    {

        // Special iterator the looks through a node and its children to determine what note
        // should be used for comparison.
        //
        // For example (a+b) should use a.
        //
        public class CompIterator
        {
            TreeIteratorPre TreeIter;
            public CompIterator(SymNode start)
            {
                TreeIter = new TreeIteratorPre(start);
            }

            public SymNode Cur
            {
                get
                {
                    return TreeIter.Cur;
                }
                //private set;
            }

            public bool Next()
            {
                while (TreeIter.Next())
                {
                    NodeTypes type = TreeIter.Cur.Type;
                    if (type.IsFunction())
                    {
                        return true;
                    }

                    if (type == NodeTypes.Var)
                        return true;
                    if (type == NodeTypes.Constant)
                        return true;
                    if (type == NodeTypes.Differential)
                        return true;
                }
                return false;
            }
        }

        static readonly int[] _SortOrder =
        {
            0, //Var
            1, //Constant
            -1, //Differential  - these should 
            4, //Cos
            4, //Sin
            4, //Tan
            3, //Power
            5, //Prod
            6, //Wedge
            7, //Plus
        };

        static int SortOrder(NodeTypes type)
        {
            Debug.Assert(_SortOrder.Length == Enum.GetNames(typeof(NodeTypes)).Length);
            return _SortOrder[(int)type];
        }


        // Comparison function used where we are ordering nodes with Sort():
        static public int CompareNodes(SymNode a, SymNode b)
        {
            var ca = new CompIterator(a);
            var cb = new CompIterator(b);

            while (true)
            {
                bool an = ca.Next();
                bool bn = cb.Next();

                if (an == false)
                {
                    if (bn == false)
                        return 0;
                    return -1;
                }
                if (bn == false)
                    return 1;

                int del = SortOrder(ca.Cur.Type) - SortOrder(cb.Cur.Type);
                if (del != 0)
                    return del;

                switch (ca.Cur.Type)
                {
                    case NodeTypes.Constant:
                        double ddel = ((ConstNode)ca.Cur).Value - ((ConstNode)cb.Cur).Value;
                        if (ddel < 0)
                            return -1;
                        if (ddel > 0)
                            return 1;
                        break; //  ddel == 0
                    case NodeTypes.Var:
                        {
                            int sdel = ((VarNode)ca.Cur).Var.CompareTo(((VarNode)cb.Cur).Var);
                            if (sdel != 0)
                                return sdel;
                            // else strings are equal.
                            break;
                        }
                    case NodeTypes.Differential:
                        {
                            int sdel = ((DNode)ca.Cur).Var.CompareTo(((DNode)cb.Cur).Var);
                            if (sdel != 0)
                                return sdel;
                            // else strings are equal.
                            break;
                        }
                    default:
                        //Debug.Assert(false);
                        break;
                }
                // Continue searching.
            }

        }
    }
}
