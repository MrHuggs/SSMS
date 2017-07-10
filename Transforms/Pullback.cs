using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using SSMS.Nodes;

namespace SSMS.Transforms
{
    public class Pullback
    {
        // Compute the pullback of a differential form under a coordainte transform.
        // For example, Lee 14.8
        //
        public static SymNode Compute(
                List<string> newvars,                       // List of the new variables
                List<Tuple<string, SymNode>> substituions,  // How to express old variable in the new
                SymNode target                              // Differential form we are pulling back
            )
        {
            Debug.Assert(newvars.Count > 0);
            Debug.Assert(substituions.Count > 0);

            SymNode result = target.DeepClone();

            // Step 1: Subsiute the coefficent functions:
            foreach (var pair in substituions)
            {
                var varname = pair.Item1;
                var value = pair.Item2;

                var varnode = new VarNode(varname);

                var intermediate = Substitution.Substitute(result, varnode, value);

                result = intermediate;
            }

            foreach (var pair in substituions)
            {
                var varname = pair.Item1;
                var value = pair.Item2;

                var dnode = new DNode(varname);

                var differential = value.Differential(newvars);


                var intermediate = Substitution.Substitute(result, dnode, differential);

                result = intermediate;
            }

            return result;
        }
    }
}
