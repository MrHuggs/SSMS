using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SSMS.Nodes;

namespace SSMS
{
    class MergeTransform : NodeTransform
    {
        TransformAttributes[] _Attributes = { TransformAttributes.Simplify };
        public override TransformAttributes[] Attributes { get { return _Attributes; } }

        public override SymNode Apply(SymNode start_node)
        {
            var merged = start_node.Merge();
            return merged;
        }

    }
}
