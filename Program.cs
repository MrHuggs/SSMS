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
            var dx_dy_c = TransformsList.Inst().TrySimplify(dx_dy_e);
            //var dx_dy_e_e = TransformsList.Inst().TryExpand(dx_dy_c);
            dx_dy_c.Sort();

            var s = dx_dy_c.ToString();
            var sp = s.Replace("+", "\n+");

            Cos2Sin2Transform.Transform(dx_dy_c);
            Cos2Sin2Transform.Transform(dx_dy_c);

            Console.WriteLine(@"dx/\dy = " + dx_dy_c.ToStringSorted());
            





        }




        static public void Cos2TransfromTest()
        {
            SymNode cos2, sin2;
            ProdNode cp, sp;
            PlusNode p;

            cos2 = new PowerNode(new CosNode(new VarNode("t")), new ConstNode(2));
            sin2 = new PowerNode(new SinNode(new VarNode("y")), new ConstNode(2));
            p = new PlusNode();
            p.AddChild(cos2);
            p.AddChild(sin2);
            p.Print();
            Debug.Assert(!Cos2Sin2Transform.Transform(p));

            cos2 = new PowerNode(new CosNode(new VarNode("t")), new ConstNode(2));
            sin2 = new PowerNode(new SinNode(new VarNode("t")), new ConstNode(2));
            p = new PlusNode();
            p.AddChild(cos2);
            p.AddChild(sin2);
            p.Print();
            Debug.Assert(Cos2Sin2Transform.Transform(p));
            p.Print();


            cos2 = new PowerNode(new CosNode(new VarNode("t")), new ConstNode(2));
            sin2 = new PowerNode(new SinNode(new VarNode("t")), new ConstNode(2));

            cp = new ProdNode();
            sp = new ProdNode();

            cp.AddChild(new ConstNode(4));
            sp.AddChild(new ConstNode(4));

            cp.AddChild(cos2);
            sp.AddChild(sin2);

            p = new PlusNode();
            p.AddChild(cp);
            p.AddChild(sp);
            p.Print();
            Debug.Assert(Cos2Sin2Transform.Transform(p));
            p.Print();
        }

        static public void ProdNodeTest()
        {
            var n = new ProdNode();

            n.AddChild(new VarNode("b"));
            n.AddChild(new VarNode("a"));
            n.AddChild(new VarNode("d"));
            n.AddChild(new VarNode("c"));

            n.Print();

            //Debug.Assert(s == "a b c d");

            n.AddChild(new ConstNode(24));
            n.AddChild(new ConstNode(-3));
            n.AddChild(new VarNode("ab"));

            n.Print();

            var p = new PlusNode();
            p.AddChild(new VarNode("b"));
            p.AddChild(new VarNode("z"));


            var power_node = new PowerNode(
                                    new CosNode(new VarNode("t")),
                                    new ConstNode(2)
                                    );

            n.AddChild(power_node);

            n.Print();

        }
    }
}
