using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    // Node for the differntial of one or more varables. They are considered
    // to be a wedge product, so order matters, more precisely, d_a d_b = -1 * d_b d_a
    //
    public class DNode : SymNode
    {
        List<String> Vars = new List<string>();
        
        public DNode()
        {
            Type = NodeTypes.Differential;
        }

        public DNode(params String[] var_list) : this()
        {
            Debug.Assert(var_list.Length > 1);
            foreach (var v in var_list)
                Vars.Add(v);
        }

        public int SortVars()
        {
            // Sort the vars that make up the wedge product and return the sign
            // of the permutation needed for the sort.
            //
            // Since we expect the number of vars to be small, we will just do a bubble sort.

            int sgn = 1;
            bool swapped;
            do
            {
                swapped = false;

                for (int j = 1; j < Vars.Count; j++)
                {
                    if (Vars[j].CompareTo(Vars[j - 1]) < 0)
                    {
                        sgn *= -1;
                        var temp = Vars[j];
                        Vars[j] = Vars[j - 1];
                        Vars[j - 1] = temp;

                        swapped = true;
                    }
                }
            }
            while (swapped);
            return sgn;
        }

        // Result a node that is this (wedge product) other
        // If the result would be 0 (becuase of a repeated variable), return
        // null.
        public DNode WedgeProduct(DNode other)
        {
            foreach(var v in Vars)
            {
                if (other.Vars.Contains(v))
                {
                    // The wedge product would contain a duplicate vars, so it is 0:
                    return null;
                }
            }
            var result = new DNode();
            foreach (var v in Vars)
                result.Vars.Add(v);
            foreach (var v in other.Vars)
                result.Vars.Add(v);

            return result;
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Differential)
                return false;

            DNode node = (DNode)other;
            var ocount = node.Vars.Count;

            if (ocount != Vars.Count)
                return false;

            // Order matters, so compare in order:
            for (int i = 0; i < Vars.Count; i++)
            {
                if (Vars[i] != node.Vars[i])
                    return false;
            }
            return true;
        }

        public override void Format(FormatBuilder fb)
        {
            foreach (var v in Vars)
            {
                fb.Append("d_");
                fb.Append(v);
            }
        }

        DNode Clone()
        {
            var result = new DNode();
            foreach (var v in Vars)
                result.Vars.Add(v);
            return result;
        }


        public override SymNode DeepClone()
        {
            return Clone();
        }


        public override SymNode FoldConstants()
        {
            var result = Clone();
            int sgn = result.SortVars();

            if (sgn == -1)
            {
                return new ProdNode(new ConstNode(-1), result);
            }
            return result;
        }

        public override SymNode Evaluate()
        {
            return Clone();
        }

        public override SymNode Differentiate(string var)
        {
            throw new ApplicationException("Cannot differentiate a differential.");
        }

        public override void AssertValid()
        {
            base.AssertValid();
            Debug.Assert(Vars.Count > 1);
        }

    }
}
