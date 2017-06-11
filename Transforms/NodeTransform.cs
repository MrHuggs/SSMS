using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        };
    
    }

}
