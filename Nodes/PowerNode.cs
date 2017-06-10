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

        public override SymNode FoldConstants()
        {
            var new_exp = Exponent.FoldConstants();
            var new_base = Base.FoldConstants();


            if (new_exp.Type == NodeTypes.Constant)
            {
                if (((ConstNode)new_exp).Value == 1)
                    return new_base;
                if (((ConstNode)new_exp).Value == 0)
                    return new ConstNode(1);
            }

            if (new_base.Type == NodeTypes.Constant)
            {
                if (((ConstNode)new_base).Value == 1)
                    return new ConstNode(1);
            }

            return new PowerNode(new_base, new_exp);
        }


        public override SymNode Evaluate()
        {
            var new_exp = Exponent.Evaluate();
            var new_base = Base.Evaluate();

            if (new_exp.Type == NodeTypes.Constant &&
                new_base.Type == NodeTypes.Constant)
            {
                double pbase = ((ConstNode)new_base).Value;

                if (pbase == 1)
                    return new ConstNode(1);

                double exp = ((ConstNode)new_exp).Value;

                if ((pbase == 0 && exp <= 0) ||
                    (pbase <= 0 && (exp != ((int) exp))))
                { 
                    // Result is actually undefined, so can't do anything:
                    new PowerNode(new_base, new_exp);
                }

                if (exp == 1)
                    return new_base;

                if (exp == 2)
                    return new ConstNode(pbase * pbase);
                if (exp == -1)
                    return new ConstNode(1 / pbase);
                if (exp == -2)
                    return new ConstNode(1 / (pbase * pbase));

                return new ConstNode(Math.Pow(pbase, exp));
            }

            var temp_node = new PowerNode(new_base, new_exp);
            return temp_node.FoldConstants();
        }
        
    }
}
