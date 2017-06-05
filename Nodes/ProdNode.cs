using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ProdNode : CommutativeNode
    {
        public ProdNode()
        {
            Type = NodeTypes.Prod;
        }

        public override void Format(StringBuilder sb)
        {
            bool first = true;
            foreach (var node in Children)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");

                if (node.Type > NodeTypes.Prod)
                {
                    // If its lower precedence, then surround it by ():
                    sb.Append("(");
                    node.Format(sb);
                    sb.Append(")");
                }
                else
                {
                    node.Format(sb);
                }
            }
        }


        public override SymNode DeepClone()
        {
            var n = new ProdNode();
            n.DeepCloneChildren(this);

            return n;
        }

        public override bool IsZero()
        {
            foreach (var child in Children)
            {
                if (child.IsZero())
                    return true;
            }
            return false;
        }
        public override bool IsOne()
        {
            if (Children.Count == 0)
                return false;

            foreach (var child in Children)
            {
                if (!child.IsOne())
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Evaluate(StringBuilder report, out double result)
        {
            result = 1;

            if (Children.Count == 0)
            {
                report.Append("Cannot evaluate product node because it has no terms.");
                return false;
            }

            double temp;
            bool success = true;

            foreach (var child in Children)
            {
                if (child.Evaluate(report, out temp))
                    result *= temp;
                else
                    success = false;
            }
            if (!success)
                report.Append("Cannot evaluate product node because one or more terms could not be evaluated.");

            return success;

        }
    }
}
