using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    public class PowerNode : SymNode
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
  
        public override void Format(FormatBuilder fb)
        {
            fb.Append(Base.ToString(), Base.Type >= NodeTypes.Power);

            fb.Append("^");

            fb.Append(Exponent.ToString(), Exponent.Type >= NodeTypes.Power);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Power)
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

        public override SymNode FoldConstants()
        {
            var new_exp = Exponent.FoldConstants();
            var new_base = Base.FoldConstants();

            if (new_exp.IsOne())
                return new_base;

            if (new_exp.IsZero() || new_base.IsOne())
                return new ConstNode(1);


            if (new_exp.Type != NodeTypes.Constant || new_base.Type != NodeTypes.Constant)
            {
                return new PowerNode(new_base, new_exp);
            }

            double exp = ((ConstNode)new_exp).Value;
            double nbase = ((ConstNode)new_base).Value;
            Debug.Assert(exp != 0 && exp != 1);

            // See if we can evaluated the power to a rational result. We will try common
            // powers.
            double result;
            switch (exp)
            {
                case 2:
                    result = nbase * nbase;
                    break;
                case 3:
                    result = nbase * nbase * nbase;
                    break;
                case 4:
                    result = nbase * nbase * nbase * nbase;
                    break;
                case -1:
                    result = 1 / nbase;
                    break;
                case -2:
                    result = 1 / (nbase * nbase);
                    break;
                default:
                    return new PowerNode(new_base, new_exp);
            }

            if (Double.IsInfinity(result) || Double.IsNaN(result))
                return new PowerNode(new_base, new_exp);
            return new ConstNode(result);
        }


        public override SymNode Evaluate()
        {
            var new_exp = Exponent.Evaluate();
            var new_base = Base.Evaluate();

            // Try to use the fold constants logic and see of the values can be folded to constant.
            // This will be more accurate that using power.
            var raw_new_node = new PowerNode(new_base, new_exp);
            var new_node = raw_new_node.FoldConstants();

            if (new_node.Type == NodeTypes.Constant)
                return new_node;

            Debug.Assert(new_node.Type == NodeTypes.Power);
            

            if (new_exp.Type == NodeTypes.Constant &&
                new_base.Type == NodeTypes.Constant)
            {
                double pbase = ((ConstNode)new_base).Value;
                double exp = ((ConstNode)new_exp).Value;

                double result = Math.Pow(pbase, exp);
                if (!Double.IsInfinity(result) && !Double.IsNaN(result))
                {
                    return new ConstNode(result);
                }
            }

            return new_node;
        }
        
    }
}
