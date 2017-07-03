//
// Driver for ths Simple Symbolic Math System (SSMS)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SSMS.Nodes;
using SSMS.Parser;

namespace SSMS
{
    class Program
    {
        static string BreakString(SymNode node)
        {
            string s = "\t" + node.ToStringSorted();
            s = s.Replace("+", "\n\t+");
            s = s.Replace("-", "\n\t-");
            return s;
        }

        // The goal is to calculation the expresion on page 375 of Lee Smooth Manifolds:
        static void Main(string[] args)
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
            Console.WriteLine(BreakString(dx_dy_c));


            var dx_dz = new WedgeNode(dx, dz);
            var dx_dz_e = TransformsList.Inst().TryExpand(dx_dz);
            var dx_dz_c = TransformsList.Inst().Simplify(dx_dz_e);

            Console.WriteLine(@"dx/\dz = ");
            Console.WriteLine(BreakString(dx_dz_c));

            var dy_dz = new WedgeNode(dy, dz);
            var dy_dz_e = TransformsList.Inst().TryExpand(dy_dz);
            var dy_dz_c = TransformsList.Inst().Simplify(dy_dz_e);

            Console.WriteLine(@"dy/\dz = ");
            Console.WriteLine(BreakString(dy_dz_c));

            var w = new PlusNode(
                        new ProdNode(x, dy_dz_c),
                        new ProdNode(y, new ConstNode(-1), dx_dz_c),
                        new ProdNode(z, dx_dy_c)
                        );
            var w_e = TransformsList.Inst().TryExpand(w);
            var w_c = TransformsList.Inst().Simplify(w_e);
            Console.WriteLine(@"w = ");
            Console.WriteLine(BreakString(w_c));
        }
    }
}
