using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
  
    class ConstFoldTransform : NodeTransform
    {
        TransformAttributes[] _Attributes = { TransformAttributes.Simplify };
        public override TransformAttributes[] Attributes { get { return _Attributes; } }

        public override SymNode Apply(SymNode start_node)
        {
            // Simply fold constants and see if the tree we get equals the one we started with.
            // with.
            //
            // This is somewhat inefficient if no simplification is done. We could address this
            // by changing SymNode to have a CanFoldConstants method or some such.
            //
            var folded = start_node.FoldConstants();

            if (folded.IsEqual(start_node))
            {
                return null;
            }
            return folded;
        }

    }
}
