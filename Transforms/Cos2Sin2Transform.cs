using System;
using System.Collections.Generic;
using System.Linq;
using SSMS.Nodes;


namespace SSMS.Nodes
{
    class Cos2Sin2Transform
    {
        // Helper: See if node is of the form trig^2(something)
        //
        static SymNode GetTrig2Arg(SymNode node, NodeTypes targ_type)
        {
            if (node.Type != NodeTypes.Power)
                return null;

            double power;
            if (!((PowerNode)node).GetFixedExponent(out power))
                return null;

            if (power != 2)
                return null;

            var arg_node = ((PowerNode)node).Base;

            if (arg_node.Type != targ_type)
                return null;

            return ((TrigNode)arg_node).Angle;
        }

        struct TrigMatch
        {
            public ProdNode Owner;      // = InitialNode if not null.
            public int Index;
            public SymNode ArgNode;

            public bool InitMatch(SymNode initial_node, NodeTypes targ_type)
            {
                // See if node is could potentially be matched to another node.

                ArgNode = GetTrig2Arg(initial_node, targ_type);
                if (ArgNode != null)
                {
                    Index = -1;
                    Owner = null;
                    return true;
                }
                if (initial_node.Type != NodeTypes.Prod)
                    return false;

                for (Index = 0; Index < initial_node.ChildCount(); Index++)
                {
                    ArgNode = GetTrig2Arg(initial_node.GetChild(Index), targ_type);
                    if (ArgNode != null)
                    {
                        Owner = (ProdNode)initial_node;
                        return true;
                    }
                }
                return false;
            }

            // Check to see if node has a match for us. If a match exists,
            // will have to be at the Index.
            public bool CheckMatch(SymNode node, NodeTypes targ_type)
            {
                SymNode other_arg;
                if (Owner == null)
                {
                    other_arg = GetTrig2Arg(node, targ_type);
                    return ArgNode.IsEqual(other_arg);
                }
                else
                {
                    // We have been passed a product node. Then it has to have
                    // the same number of components as Ownder, and the all need
                    // to match except for one which should be the corresponding
                    // trig node:
                    if (node.Type != NodeTypes.Prod)
                        return false;

                    if (node.ChildCount() != Owner.ChildCount())
                        return false;

                    for (int index = 0; Index < Owner.ChildCount(); Index++)
                    {
                        var other_child = node.GetChild(index);

                        if (index == Index)
                        {
                            other_arg = GetTrig2Arg(other_child, targ_type);
                            if (other_arg != null)
                            {
                                if (!ArgNode.IsEqual(other_arg))
                                    return false;
                            }
                        }
                        else
                        {
                            if (!Owner.GetChild(index).IsEqual(other_child))
                                return false;
                        }
                    }
                    return true;    // Arguments and nodes match.
                }

            }

        };

        static int FindMatch(PlusNode plus_node, TrigMatch match, int skip_index, NodeTypes targ_type)
        {
            for (int index = 0; index < plus_node.ChildCount(); index++)
            {
                if (index == skip_index)
                    continue;   // This is not node we are matching to.

                if (match.CheckMatch(plus_node.GetChild(index), targ_type))
                {
                    return index;
                }
            }
            return -1;
        }

        static public bool Transform(SymNode node)
        {
            // See of the cos^2 + sin^2 = 1 transform can be applies to two of the
            // children of this node.

            // Step 1: It has to be a plus node
            if (node.Type != NodeTypes.Plus)
                return false;

            var plus_node = (PlusNode)node;

            // Step 2: Look through the children for something that has a cos^2 factor
            //  This could be either a power node directory, or a product node that
            //  his a cos^2 factor.

            int index, match_index = -1;
            TrigMatch match = new TrigMatch();
            for (index = 0; index < plus_node.ChildCount(); index++)
            {
                var cnode = plus_node.GetChild(index);

                if (match.InitMatch(cnode, NodeTypes.Cos))
                {
                    match_index = FindMatch(plus_node, match, index, NodeTypes.Sin);

                    if (match_index < 0)
                        continue;

                    break;
                }
            }
            if (match_index == -1)
                return false;

            if (match.Owner == null)
            {
                // These are bare cos^2 + sin^2 nodes. Replace with 1.
                if (index < match_index)    // Remove highest index first.
                {
                    plus_node.RemoveChild(match_index);
                    plus_node.RemoveChild(index);
                }
                else
                {
                    plus_node.RemoveChild(index);
                    plus_node.RemoveChild(match_index);
                }
                plus_node.AddChild(new ConstNode(1));
            }
            else
            {
                // This are two product nodes with a bunch of children. Delete the matched
                // node, and remove the cos^2 term from the initial match.
                plus_node.RemoveChild(match_index);

                match.Owner.RemoveChild(match.Index);

                if (match.Owner.ChildCount() == 0)
                {
                    // We had a product node with only 1 element. Ideally, this would have
                    // be simplified away before.
                    match.Owner.AddChild(new ConstNode(1));
                }
            }
            return true;
        }
    }
}
