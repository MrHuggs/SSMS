using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    public enum TransformAttributes
    {
        Simplify,
        Expand,
    }

    public abstract class NodeTransform
    {
        //
        // A transform anylyzes a node, and if possible, transforms it to a new node.
        //
        // If transformation is possible, return the new node, otherwise, return null.
        // Regardless, start_node should be unchanged.
        //
        public abstract SymNode Apply(SymNode start_node);

        // Information about the transform:
        public virtual TransformAttributes[] Attributes { get; }
    }

    public class TransformsList
    {
        static public NodeTransform[] Transforms =
        {
                new ConstFoldTransform(),
                new DistributiveTransform(),
        };


        static TransformsList _Inst;

        static public TransformsList Inst()
        {
            if (_Inst == null) _Inst = new TransformsList();
            return _Inst;
        }


        NodeTransform[] Simplifiers;
        NodeTransform[] Exapnders;

        public TransformsList()
        {
            Simplifiers = Transforms.Where(t => t.Attributes.Contains(TransformAttributes.Simplify)).ToArray();
            Exapnders = Transforms.Where(t => t.Attributes.Contains(TransformAttributes.Expand)).ToArray();
        }

        public SymNode Simplify(SymNode start_node)
        {
            return ModifyTree(start_node, Simplifiers);
        }
        public SymNode Expand(SymNode start_node)
        {
            return ModifyTree(start_node, Exapnders);
        }

        SymNode ModifyTree(SymNode start_node, NodeTransform[] transforms)
        {
            var temp_parent = new PlusNode(start_node.DeepClone());
            var it = new TreeIterator(temp_parent);

            while (it.Next())
            {
                SymNode node = it.Cur;

                bool transformed;
                do
                {
                    transformed = false;
                    for (int i = 0; i < node.ChildCount(); i++)
                    {
                        var child = node.GetChild(i);
                        foreach (var t in transforms)
                        {
                            var new_child = t.Apply(child);

                            if (new_child != null)
                            {
                                node.ReplaceChild(child, new_child);
                                transformed = true;
                                break;
                            }
                        }
                        if (transformed)
                            break;
                    }
                }
                while (transformed);
            }

            return temp_parent.GetChild(0);
        }
    
    }

}
