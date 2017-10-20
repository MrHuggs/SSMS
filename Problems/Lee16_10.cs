using System;
using System.Diagnostics;
using System.Collections.Generic;
using SSMS.Nodes;
using SSMS.Parser;
using SSMS.Transforms;

namespace SSMS.Problems
{
	class Lee16_10
	{
		// The goal is compute the cacluation of torus integrals for Problem 16-10 of Lee Smooth Manifolds.


		public static int Execute()
		{
			var x = SymNodeBuilder.ParseString("(cos(v) + 2) * cos(u)");
			var y = SymNodeBuilder.ParseString("(cos(v) + 2) * sin(u)");
			var z = SymNodeBuilder.ParseString("sin(v)");


			var dx = Differential.Compute(x);
			var dy = Differential.Compute(y);
			var dz = Differential.Compute(z);

			Console.WriteLine("dx = {0}\n", dx.ToString());
			Console.WriteLine("dy = {0}\n", dy.ToString());
			Console.WriteLine("dz = {0}\n", dz.ToString());

			var dx_dy = new WedgeNode(dx, dy);
			var dx_dy_s = PrepareNode(dx_dy);
			Console.WriteLine("dx_dy = {0}\n", dx_dy_s.ToString());
			
			var dx_dz = new WedgeNode(dx, dz);
			var dx_dz_s = PrepareNode(dx_dz);
			Console.WriteLine("dx_dz = {0}\n", dx_dz_s.ToString());

			var dy_dz = new WedgeNode(dy, dz);
			var dy_dz_s = PrepareNode(dy_dz);
			Console.WriteLine("dy_dz = {0}\n", dy_dz_s.ToString());


			// This is the volume form for the torus, computed by doing interior multiplication of the normal with the 
			// volume from of R^3.
			var omega = SymNodeBuilder.ParseString("sin(v)* dxdy + cos(u)*cos(v)*dydz - sin(u) *cos(v)*dxdz");
			Console.WriteLine("raw omega = {0}\n", omega.ToString());
	
			omega = Substitution.Substitute(omega, new VarNode("dxdy"), dx_dy_s);
			omega = Substitution.Substitute(omega, new VarNode("dxdz"), dx_dz_s);
			omega = Substitution.Substitute(omega, new VarNode("dydz"), dy_dz_s);

			var omega_e = TransformsList.Inst().TryExpand(omega);
			var omega_s = TransformsList.Inst().TrySimplify(omega_e);

			// This is the desired result for part A, namely the the integrand for finding the surface area:
			Console.WriteLine("omega = {0}\n", omega_s.ToString());

			return 0;
		}

		static SymNode du_dv = SymNodeBuilder.ParseString(@"d_u/\d_v");
		static SymNode c = new ConstNode(1);

		// Since we are ultimately computing a 2-form for a 2-d manifold, we know what the wedge product term will 
		// be. It will always by d_u/\ d_v. For readablity, we will remove this term:
		static SymNode PrepareNode(SymNode start)
		{
			var expanded = TransformsList.Inst().TryExpand(start);
			var simplified = TransformsList.Inst().TrySimplify(expanded);

			var wedge_removed = Substitution.Substitute(simplified, du_dv, c);
			wedge_removed = wedge_removed.FoldConstants();

			return wedge_removed;

		}
	}
}
