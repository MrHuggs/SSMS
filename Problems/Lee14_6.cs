using System;
using System.Diagnostics;
using SSMS.Nodes;
using SSMS.Parser;

namespace SSMS.Problems
{
    class Lee14_6
    {
        // The goal is the calculation of the expresion on page 375 of Lee Smooth Manifolds.
        //
        // We have x, y, z defined in terms of static coordinates.
        //
        // w = x d_y /\ d_z + y d_z /\ d_z + z dz /\ d_y
        //
        //
        // Then, in rectangular coordinates, dw = 3 d_x /\ d_y /\ d_z
        //
        // Want to check that this matches the result of pullback of w to spherical coordinates.
        //

        public static int Execute()
        {
            var x = SymNodeBuilder.ParseString("r*sin(phi)*cos(theta)");
            var y = SymNodeBuilder.ParseString("r*sin(phi)*sin(theta)");
            var z = SymNodeBuilder.ParseString("r*cos(phi)");

            var dx = Differential.Compute(x);
            var dy = Differential.Compute(y);
            var dz = Differential.Compute(z);

            var dx_dy = new WedgeNode(dx, dy);
            var dx_dy_e = TransformsList.Inst().TryExpand(dx_dy);
            var dx_dy_c = TransformsList.Inst().Simplify(dx_dy_e);

            Console.WriteLine(@"dx/\dy = ");
            Console.WriteLine(dx_dy_c.BreakStringSorted());

            var dx_dz = new WedgeNode(dx, dz);
            var dx_dz_e = TransformsList.Inst().TryExpand(dx_dz);
            var dx_dz_c = TransformsList.Inst().Simplify(dx_dz_e);

            Console.WriteLine(@"dx/\dz = ");
            Console.WriteLine(dx_dz_c.BreakStringSorted());

            var dy_dz = new WedgeNode(dy, dz);
            var dy_dz_e = TransformsList.Inst().TryExpand(dy_dz);
            var dy_dz_c = TransformsList.Inst().Simplify(dy_dz_e);

            Console.WriteLine(@"dy/\dz = ");
            Console.WriteLine(dy_dz_c.BreakStringSorted());

            var w = new PlusNode(
                        new ProdNode(x, dy_dz_c),
                        new ProdNode(y, new ConstNode(-1), dx_dz_c),
                        new ProdNode(z, dx_dy_c)
                        );
            var w_e = TransformsList.Inst().TryExpand(w);
            var w_c = TransformsList.Inst().Simplify(w_e);
            Console.WriteLine(@"w = ");
            Console.WriteLine(w_c.BreakStringSorted());

            // w_c has the pullbakc of w. Inspection shows that it has only a d_phi/\d_theta term, so only the 
            // r derrivative contributes to dw.
            var w_cb = Substitution.Substitute(w_c, SymNodeBuilder.ParseString(@"d_phi/\d_theta"), new ConstNode(1));
            var w_cbs = TransformsList.Inst().Simplify(w_cb);

            // Check that this removed the differentials:
            Debug.Assert(w_cbs.HasDifferential() == false);

            // Take r derivative, and mulitiply back in the correct wedge product.
            var dwbare = TransformsList.Inst().Simplify(w_cbs.Differentiate("r"));
            SymNode dw = new WedgeNode(dwbare, SymNodeBuilder.ParseString(@"d_r/\d_phi/\d_theta"));
            dw = TransformsList.Inst().Expand(dw);
            dw = TransformsList.Inst().Simplify(dw);
            Console.WriteLine(@"In spherical dw = ");
            Console.WriteLine(dw.BreakStringSorted());

            // Rectangular coordinate calc:
            var dw_rect = new ProdNode(new ConstNode(3), new WedgeNode(dx_dy_c, dz));
            var dw_rect_e = TransformsList.Inst().TryExpand(dw_rect);
            var dw_rect_c = TransformsList.Inst().Simplify(dw_rect_e);
            Console.WriteLine(@"In rectangular dw = ");
            Console.WriteLine(dw_rect_c.BreakStringSorted());

            if (dw_rect_c.IsEqual(dw))
            {
                Console.WriteLine("Calculation verified!");
                return 1;
            }
            Console.WriteLine("Verification failed!");
            return 0;
        }
    }
}
