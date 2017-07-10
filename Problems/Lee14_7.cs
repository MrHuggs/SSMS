using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using SSMS.Nodes;
using SSMS.Parser;
using SSMS.Transforms;

namespace SSMS.Problems
{
    class Lee14_7
    {
        public static int Execute()
        {
            var new_vars = new List<string> { "theta", "phi" };

            List<Tuple<string, SymNode>> substituions = new List<Tuple<string, SymNode>>
            {
                Tuple.Create("x", SymNodeBuilder.SimplifyString("(cos(phi) + 2)*cos(theta)")),
                Tuple.Create("y", SymNodeBuilder.SimplifyString("(cos(phi) + 2)*sin(theta)")),
                Tuple.Create("z", SymNodeBuilder.SimplifyString("sin(phi)")),
            };

            var omega = SymNodeBuilder.SimplifyString(@"y*d_z/\d_x");

            {
                // Take the target 2-form and pull it back to polar coordinates:

                var result = Pullback.Compute(new_vars, substituions, omega);

                var result_s = TransformsList.Inst().TrySimplify(result);
                var result_e = TransformsList.Inst().TryExpand(result_s);
                var result_ss = TransformsList.Inst().TrySimplify(result_e);

                Console.WriteLine(result_ss.BreakStringSorted());

                // Now take the exterior derivative in polar coordinates. The exterior derivative of a 2-form on a 2d manifold
                // should alway be 0:
                var domega_polar = Differential.ExteriorDerivative(result_ss);
                domega_polar = TransformsList.Inst().TryExpand(domega_polar);
                domega_polar = TransformsList.Inst().TrySimplify(domega_polar);
                Console.WriteLine(domega_polar.BreakStringSorted());

            }

            // Take the orignal 2-form, compute the exterior derivative in R^3, and then pullback
            // to polar coordinates. Since we are pulling a 3-form to a 2d space, the result should
            // be 0:
            {
                var domega = Differential.ExteriorDerivative(omega);
                domega = TransformsList.Inst().TrySimplify(domega);

                Debug.Assert(domega.IsEqual(SymNodeBuilder.SimplifyString(@"d_x/\d_y/\d_z")));

                var result = Pullback.Compute(new_vars, substituions, domega);

                var result_s = TransformsList.Inst().TrySimplify(result);
                var result_e = TransformsList.Inst().TryExpand(result_s);
                var result_ss = TransformsList.Inst().TrySimplify(result_e);

                Console.WriteLine(result_ss.BreakStringSorted());
            }

            return 1;
        }
    }
}
