using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    class PowerNode : SymNode
    {
        public PowerNode(SymNode ebase, SymNode power)
        {
            Type = NodeTypes.Power;

            Base = ebase;
            Exponent = power;
        }

        public SymNode Base;
        public SymNode Exponent;

        public override int ChildCount() { return 2; }
        public override SymNode GetChild(int index)
        {
            switch (index)
            {
                case 0:
                    return Base;
                case 1:
                    return Exponent;
                default:
                    Debug.Assert(false);
                    return null;
            }
        }
        public override void ReplaceChild(SymNode existing_child, SymNode new_child)
        {
            // We must always have two arguments.
            Debug.Assert(new_child != null);
            if (existing_child == Base)
                Base = new_child;
            else
            {
                Debug.Assert(existing_child == Exponent);
                Exponent = new_child;
            }
        }
  
        public override void Format(StringBuilder sb)
        {
            sb.Append("(");
            Base.Format(sb);
            sb.Append(")");
            sb.Append("^(");
            Exponent.Format(sb);
            sb.Append(")");
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            PowerNode pnode = (PowerNode)other;

            return Base.IsEqual(pnode.Base) && Exponent.IsEqual(pnode.Exponent);
        }

        public override NodeSortVal GetSortVal()
        {
            return Base.GetSortVal();
        }

        public override SymNode DeepClone()
        {
            return new PowerNode(Base.DeepClone(), Exponent.DeepClone());
        }

        // Helper If this has a fixed exponent, return it. This is usefull for some symplifications.
        public bool GetFixedExponent(out double exponent)
        {
            if (Exponent.IsOne())
            {
                exponent = 1;
                return true;
            }
            if (Exponent.IsZero())
            {
                exponent = 0;
                return true;
            }

            if (Exponent.Type != NodeTypes.Constant)
            {
                exponent = 0;
                return false;
            }

            exponent = ((ConstNode)Exponent).Value;
            return true;
        }

        public override bool Evaluate(StringBuilder report, out double result)
        {
            result = 0;
            double exp, pbase;
            if (Base == null)
            {
                report.Append("Cannot evaluate power because base is missing.");
                return false;
            }
            if (Exponent == null)
            {
                report.Append("Cannot evaluate power because exponent is missing.");
                return false;
            }

            if (!Base.Evaluate(report, out pbase))
            {
                report.Append("Cannot evaluate power because base could not be evaluated.");
                return false;
            }
            if (!Exponent.Evaluate(report, out exp))
            {
                report.Append("Cannot evaluate power because expononent could not be evaluated.");
                return false;
            }

            if (exp < 0)
            {
                if (pbase == 0)
                {
                    report.Append("Cannot evaluate power because base is 0 and exponent is negative.");
                    return false;
                }
            }
            if (exp != (int) exp)
            {
                if (pbase < 0)
                {
                    report.Append("Cannot evaluate power because base is negative and exponent is factional.");
                    return false;
                }
            }

            // Handle some special cases ot improve accuracy:
            switch (exp)
            {
                case 0:
                    result = 1;
                    return true;
                case 1:
                    result = pbase;
                    return true;
                case 2:
                    result = pbase * pbase;
                    return true;
                case -1:
                    result = 1 / pbase;
                    return true;
                case -2:
                    result = 1 / (pbase * pbase);
                    return true;
                default:
                    result = Math.Pow(pbase, exp);
                    return true;
            }
        }

    }
}
