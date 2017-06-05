using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class PlusNode : CommutativeNode
    {
        public PlusNode()
        {
            Type = NodeTypes.Plus;
        }

        public override void Format(StringBuilder sb)
        {
            bool first = true;
            foreach (var node in Children)
            {
                if (first)
                    first = false;
                else
                    sb.Append("+");

                node.Format(sb);
            }
        }


        public override SymNode DeepClone()
        {
            var n = new PlusNode();
            n.DeepCloneChildren(this);

            return n;
        }

        public override bool IsZero()
        {
            foreach (var child in Children)
            {
                if (!child.IsZero())
                    return false;
            }
            return true;
        }
        public override bool IsOne()
        {
            int sum = 0;
            foreach (var child in Children)
            {
                if (child.IsOne())
                {
                    if (sum == 1)
                        return false;
                    sum++;
                }
            }
            return sum == 1;
        }
        public override bool Evaluate(StringBuilder report, out double result)
        {
            result = 0;
            if (Children.Count == 0)
            {
                report.Append("Cannot evaluate summatation node because it has no terms.");
                return false;
            }

            double temp;
            bool success = true;

            foreach (var child in Children)
            {
                if (child.Evaluate(report, out temp))
                    result += temp;
                else
                    success = false;
            }
            if (!success)
                report.Append("Cannot evaluate summatation node because one or more terms could not be evaluated.");

            return success;

        }

    }
}
