﻿//
// Driver for ths Simple Symbolic Math System (SSMS)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SSMS.Nodes;

namespace SSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            EvaluateTest();
            DistributiveTransformTest();
            ConstFoldTransformTest();
            Cos2TransfromTest();
            ProdNodeTest();
        }

        static public void EvaluateTest()
        {
            PlusNode plus_node = new PlusNode();
            plus_node.AddChild(new ConstNode(4));
            plus_node.AddChild(new ConstNode(5));

            plus_node.PrintValue();

            ProdNode prod_node = new ProdNode();
            prod_node.AddChild(new ConstNode(6));
            prod_node.AddChild(new VarNode("x"));
            prod_node.AddChild(new ConstNode(-7));
            prod_node.PrintValue();

            plus_node.AddChild(prod_node);
            plus_node.PrintValue();

        }

        static  public void DistributiveTransformTest()
        {
            ProdNode p = new ProdNode();
            PlusNode plus_node = new PlusNode();
            plus_node.AddChild(new VarNode("a"));
            plus_node.AddChild(new VarNode("b"));
            plus_node.PrintValue();
            p.AddChild(plus_node);
            p.AddChild(new ConstNode(4));

            p.PrintValue();
            var dist_res = DistributiveTransform.TryDistributeProdNode(p);
            dist_res.Print();
            dist_res.PrintValue();

        }

        static public void ConstFoldTransformTest()
        {/*
            ProdNode p = new ProdNode();

            p.AddChild(new ConstNode(4));
            Debug.Assert(!ConstFoldTransform.Transform(p));

            p.AddChild(new ConstNode(-4));
            Debug.Assert(ConstFoldTransform.Transform(p));
            Debug.Assert(p.ToString() == "-16");

            p.AddChild(new VarNode("a"));
            Debug.Assert(!ConstFoldTransform.Transform(p));
            p.AddChild(new ConstNode(-1));
            Debug.Assert(ConstFoldTransform.Transform(p));
            Debug.Assert(p.ToString() == "16a");

            p.AddChild(new ConstNode(1.0/ 16.0));
            Debug.Assert(ConstFoldTransform.Transform(p));
            Debug.Assert(p.ToString() == "a");

            p.AddChild(new ConstNode(25.0));
            p.AddChild(new ConstNode(0));
            Debug.Assert(ConstFoldTransform.Transform(p));
            Debug.Assert(p.ToString() == "0");
            */
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
