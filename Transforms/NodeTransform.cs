using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SSMS.Nodes;

namespace SSMS
{
    public enum TransformAttributes
    {
        Simplify,
        Expand,
        Recursive,  // This tranform can just be applied at the top node.
    }

    public abstract class NodeTransform
    {
        //
        // A transform anylyzes a node, and if possible, transforms it to a new node.
        // The original node should be unchanged.
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
                new MergeTransform(),
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
            SymNode new_version = start_node.DeepClone();
            int change_count = 0;
            bool transformed;

            do
            {
                transformed = false;
                foreach (var t in transforms)
                {
                    SymNode updated;
                    if (t.Attributes.Contains(TransformAttributes.Recursive))
                    {
                        updated = t.Apply(new_version);
                    }
                    else
                    {
                        updated = ApplyTransformDFO(t, new_version);
                    }

                    if (updated != null)
                    {
                        new_version = updated;
                        change_count++;
                        transformed = true;
                        break;
                    }
                }
            } while (transformed == true);
           

            if (change_count > 0)
                return new_version;

            return null;
        }

        // Private helper - apply a transform to start_node which is allowed to be
        // modified. 
        // Return a new base node if any transform took place, or null if there
        // was no change.  
        SymNode ApplyTransformDFO(NodeTransform t, SymNode start_node)
        {
            var temp_parent = new PlusNode();
            temp_parent.AddChild(start_node);

            var it = new TreeIterator(temp_parent);

            while (it.Next())
            {
                SymNode node = it.Cur;
                for (int i = 0; i < node.ChildCount(); i++)
                {
                    var child = node.GetChild(i);
                    var new_child = t.Apply(child);

                    if (new_child != null)
                    {
                        //Debug.WriteLine("Transformed:");
                        //Debug.WriteLine(child.ToString());
                        //Debug.WriteLine(new_child.ToString());

                        node.ReplaceChild(child, new_child);
                        return temp_parent.GetChild(0);
                    }
                }
            }
            return null;

        }

    }

}
