//
// Driver for ths Simple Symbolic Math System (SSMS)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            Cos2TransfromTest();
            ProdNodeTest();
        }

        static public void Cos2TransfromTest()
        {
            var cos2 = new PowerNode(
                                        new CosNode(new VarNode("t")),
                                        new ConstNode(2)
                                        );

            var sin2 = new PowerNode(
                                        new SinNode(new VarNode("t")),
                                        new ConstNode(2)
                                        );

            var p = new PlusNode();
            p.AddChild(cos2);
            p.AddChild(sin2);


            StringBuilder sb = new StringBuilder();

            p.Format(sb);
            var s = sb.ToString();
            Console.WriteLine(s);
        }

        static public void ProdNodeTest()
        {
            var n = new ProdNode();

            n.AddChild(new VarNode("b"));
            n.AddChild(new VarNode("a"));
            n.AddChild(new VarNode("d"));
            n.AddChild(new VarNode("c"));

            StringBuilder sb = new StringBuilder();

            n.Format(sb);
            var s = sb.ToString();
            Console.WriteLine(s);

            Debug.Assert(s == "a b c d");

            n.AddChild(new ConstNode(24));
            n.AddChild(new ConstNode(-3));
            n.AddChild(new VarNode("ab"));

            sb.Clear();
            n.Format(sb);
            s = sb.ToString();
            Console.WriteLine(s);

            var p = new PlusNode();
            p.AddChild(new VarNode("b"));
            p.AddChild(new VarNode("z"));


            var power_node = new PowerNode(
                                    new CosNode(new VarNode("t")),
                                    new ConstNode(2)
                                    );

            n.AddChild(power_node);

            sb.Clear();
            n.Format(sb);
            s = sb.ToString();
            Console.WriteLine(s);

        }
    }
}
