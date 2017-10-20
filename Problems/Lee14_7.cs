using System;
using System.Diagnostics;
using System.Collections.Generic;
using SSMS.Nodes;
using SSMS.Parser;
using SSMS.Transforms;

namespace SSMS.Problems
{
    class Lee14_7
    {
        public static int PartB()
        {
            var new_vars = new List<string> { "theta", "phi" };

            List<Tuple<string, SymNode>> substituions = new List<Tuple<string, SymNode>>
            {
                Tuple.Create("x", SymNodeBuilder.SimplifyString("(cos(phi) + 2)*cos(theta)")),
                Tuple.Create("y", SymNodeBuilder.SimplifyString("(cos(phi) + 2)*sin(theta)")),
                Tuple.Create("z", SymNodeBuilder.SimplifyString("sin(phi)")),
            };

            var omega = SymNodeBuilder.SimplifyString(@"y*d_z/\d_x");
            SymNode domega_polar, domega_rect_pullback;
            {
                // Take the target 2-form and pull it back to polar coordinates:

                var result = Pullback.Compute(new_vars, substituions, omega);

                var result_s = TransformsList.Inst().TrySimplify(result);
                var result_e = TransformsList.Inst().TryExpand(result_s);
                var result_ss = TransformsList.Inst().TrySimplify(result_e);

                //Console.WriteLine(result_ss.BreakStringSorted());

                // Now take the exterior derivative in polar coordinates. The exterior derivative of a 2-form on a 2d manifold
                // should alway be 0:
                domega_polar = Differential.ExteriorDerivative(result_ss);
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
                domega_rect_pullback = TransformsList.Inst().TrySimplify(result_e);

                Console.WriteLine(domega_rect_pullback.BreakStringSorted());
            }

            if (domega_rect_pullback.IsEqual(domega_polar))
                Console.WriteLine("Confirmed that exterior derivative commutes with the pullback.");

            return 1;
        }

        public static int Execute()
        {
            var sn = SymNodeBuilder.SimplifyString("x*(x^2+y^2+z^2)^-1.5");

            sn.Print();

            var sn_x = sn.Differentiate("x");
            var sn_xs = TransformsList.Inst().TrySimplify(sn_x);

            var sym = SymNodeBuilder.SimplifyString("x^2+y^2+z^2");
            var sn_p = new ProdNode(sym, sn_xs);
            var sn_pe = TransformsList.Inst().TryExpand(sn_p);
            var sn_pes = TransformsList.Inst().TrySimplify(sn_pe);


            var vlist = Differential.FindVariables(sn);
            var sn_d = sn.Differential(vlist);
            var sn_ds = TransformsList.Inst().TrySimplify(sn_d);

            return 1;
        }


        public static int PartC()
        {
            var new_vars = new List<string> { "u", "v" };

            List<Tuple<string, SymNode>> substituions = new List<Tuple<string, SymNode>>
            {
                Tuple.Create("x", SymNodeBuilder.SimplifyString("u")),
                Tuple.Create("y", SymNodeBuilder.SimplifyString("v")),
                Tuple.Create("z", SymNodeBuilder.SimplifyString("(1-u^2-v^2)^1.5")),
            };

            var omega = SymNodeBuilder.SimplifyString(@"(x*d_y/\d_z+y*d_z/\d_x+z*d_x/\d_y)*(x^2+y^2+z^2)^-1.5");

            SymNode domega_polar, domega_rect_pullback;
            {
                // Take the target 2-form and pull it back to polar coordinates:

                var result = Pullback.Compute(new_vars, substituions, omega);

                var result_s = TransformsList.Inst().TrySimplify(result);
                var result_e = TransformsList.Inst().TryExpand(result_s);
                var result_ss = TransformsList.Inst().TrySimplify(result_e);

                Console.WriteLine(result_ss.BreakStringSorted());

                // Now take the exterior derivative in polar coordinates. The exterior derivative of a 2-form on a 2d manifold
                // should alway be 0:
                domega_polar = Differential.ExteriorDerivative(result_ss);
                domega_polar = TransformsList.Inst().TryExpand(domega_polar);
                domega_polar = TransformsList.Inst().TrySimplify(domega_polar);
                Console.WriteLine(domega_polar.BreakStringSorted());

            }

            // Take the orignal 2-form, compute the exterior derivative in R^3, and then pullback
            // to polar coordinates. Since we are pulling a 3-form to a 2d space, the result should
            // be 0:
            {
                omega = TransformsList.Inst().TryExpand(omega);
                omega = TransformsList.Inst().TrySimplify(omega);
                var domega = Differential.ExteriorDerivative(omega);
                domega = TransformsList.Inst().TrySimplify(domega);

                //Debug.Assert(domega.IsEqual(SymNodeBuilder.SimplifyString(@"d_x/\d_y/\d_z")));

                var result = Pullback.Compute(new_vars, substituions, domega);

                var result_s = TransformsList.Inst().TrySimplify(result);
                var result_e = TransformsList.Inst().TryExpand(result_s);
                domega_rect_pullback = TransformsList.Inst().TrySimplify(result_e);

                Console.WriteLine(domega_rect_pullback.BreakStringSorted());
            }

            if (domega_rect_pullback.IsEqual(domega_polar))
                Console.WriteLine("Confirmed that exterior derivative commutes with the pullback.");

            return 1;
        }
    }
}
