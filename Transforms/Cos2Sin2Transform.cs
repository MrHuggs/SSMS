using System;
using System.Collections.Generic;
using System.Linq;
using SSMS.Nodes;


namespace SSMS.Nodes
{
    public class Cos2Sin2Transform : NodeTransform
    {
        TransformAttributes[] _Attributes = { TransformAttributes.Simplify };
        public override TransformAttributes[] Attributes { get { return _Attributes; } }
        
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

        // Stores a potential of match of trig functions that can be combined.
        struct TrigMatch
        {
            public ProdNode Owner;      // = initial_node if not null.
            public int OwnerIndex;
            public int Index;
            public SymNode ArgNode;

            public bool InitMatch(SymNode initial_node, int initial_node_index, NodeTypes targ_type)
            {
                // See if initial_node could potentially be matched to another node.
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
                        OwnerIndex = initial_node_index;
                        return true;
                    }
                }
                return false;
            }

            // Check to see if start_node has a match for us. Note that the components
            // could be in a different order.
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
                    // the same number of components as Owner, and they all need
                    // to have match except for one which should be the corresponding
                    // trig node:
                    if (node.Type != NodeTypes.Prod)
                        return false;

                    if (node.ChildCount() != Owner.ChildCount())
                        return false;

                    bool[] used = new bool[node.ChildCount()];
                    for (int i = 0; i < Owner.ChildCount(); i++)
                    {
                        if (i == Index)
                        {
                            // Look for the trig node of the other type:
                            for (int j = 0; ; j++)
                            {
                                if (j == node.ChildCount())
                                    return false;
                                if (used[j])
                                    continue;

                                var other_child = node.GetChild(j);
                                other_arg = GetTrig2Arg(other_child, targ_type);
                                if (other_arg != null)
                                {
                                    if (ArgNode.IsEqual(other_arg))
                                    {
                                        used[j] = true;
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // Find a corresponding match:
                            for (int j = 0; ; j++)
                            {
                                if (j == node.ChildCount())
                                    return false;
                                if (used[j])
                                    continue;
                                var other_child = node.GetChild(j);
                                if (Owner.GetChild(i).IsEqual(other_child))
                                {
                                    used[j] = true;
                                    break;
                                }
                            }
                        }
                    }
                    return true;    // Arguments and nodes match.
                }
            }

        };

        // Helper: Search all of the children of plus_node for a match:
        static int FindMatch(PlusNode plus_node, TrigMatch match, int skip_index, NodeTypes targ_type)
        {
            for (int index = 0; index < plus_node.ChildCount(); index++)
            {
                if (index == skip_index)
                    continue;   // This is the node  we are matching to.

                if (match.CheckMatch(plus_node.GetChild(index), targ_type))
                {
                    return index;
                }
            }
            return -1;
        }

        static public SymNode _Apply(SymNode start_node)
        {
            // See if the cos^2 + sin^2 = 1 transform can be applied to two of the
            // children of the start_node. Return null if transform cannot be applied,
            // else a new node with the transform applied.
            //

            // Step 1: It has to be a plus start_node
            if (start_node.Type != NodeTypes.Plus)
                return null;

            var plus_node = (PlusNode)start_node;

            // Step 2: Look through the children for something that has a cos^2 factor
            //  This could be either a power node directly, or a product node that
            //  his a cos^2 factor.

            int index, match_index = -1;
            TrigMatch match = new TrigMatch();
            for (index = 0; index < plus_node.ChildCount(); index++)
            {
                var cnode = plus_node.GetChild(index);
                
                if (match.InitMatch(cnode, index, NodeTypes.Cos)) 
                {
                    // Step 3: This is a potential match. Now look for a sin^2 to go with it:

                    match_index = FindMatch(plus_node, match, index, NodeTypes.Sin);

                    if (match_index < 0)
                        continue;

                    break;
                }
            }
            if (match_index == -1)
                return null;

            // We know we have a match. But we are not supposed to change the start node, so clone it 
            // and apply the changes by index to the clone:
            var plus_node_result = (PlusNode) plus_node.DeepClone();

            if (match.Owner == null)
            {
                // These are bare cos^2 + sin^2 nodes. Replace with 1.
                if (index < match_index)    // Remove highest index first.
                {
                    plus_node_result.RemoveChild(match_index);
                    plus_node_result.RemoveChild(index);
                }
                else
                {
                    plus_node_result.RemoveChild(index);
                    plus_node_result.RemoveChild(match_index);
                }
                plus_node_result.AddChild(new ConstNode(1));
            }
            else
            {
                // This are two product nodes with a bunch of children. Delete the matched
                // sin^2 node, and remove the cos^2 term from the initial match.

                var match_owner = (ProdNode)plus_node_result.Children[match.OwnerIndex];
                match_owner.RemoveChild(match.Index);   // The cos^2 node
				if (match_owner.ChildCount() == 1)
				{
					// The match owner now has a single node, so, just use that. This would happen if we had somehting
					// like 2*cos^2(x).
					plus_node_result.ReplaceChild(match_owner, match_owner.GetChild(0));
				}

                plus_node_result.RemoveChild(match_index);  


                if (match.Owner.ChildCount() == 0)
                {
                    // We had a product start_node with only 1 element. Ideally, this would have
                    // be simplified away before.
                    match.Owner.AddChild(new ConstNode(1));
                }
            }
            if (plus_node_result.ChildCount() == 1)
                return plus_node_result.Children[0];

            return plus_node_result;
        }

        public override SymNode Apply(SymNode start_node)
        {
            return _Apply(start_node);
        }
    }
}
