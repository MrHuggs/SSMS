﻿using System;
using NUnit.Framework;
using SSMS;


namespace UnitTests
{
    [TestFixture]
    public class ConstantFolding
    {

        [TestCase]
        public void FoldTest()
        {
            var c0 = new ConstNode(0);
            var c1 = new ConstNode(1);
            var c10 = new ConstNode(10);
            var cm13 = new ConstNode(-13);
            var c4 = new ConstNode(4);

            var a = new VarNode("a");
            var b = new VarNode("b");

            var plus = new PlusNode();

            plus.AddChild(c10);
            plus.AddChild(cm13);
            plus.AddChild(c4);

            var folded = plus.FoldConstants();
            Assert.AreEqual("1", folded.ToString());

            var prod = new ProdNode();

            prod.AddChild(c10);
            prod.AddChild(cm13);
            prod.AddChild(c4);

            folded = prod.FoldConstants();
            Assert.AreEqual("-520", folded.ToString());

            prod.AddChild(plus);
            folded = prod.FoldConstants();
            Assert.AreEqual("-520", folded.ToString());

            prod.RemoveChild(plus);
            plus.AddChild(prod);
            folded = plus.FoldConstants();
            Assert.AreEqual("-519", folded.ToString());

            prod.AddChild(b);
            plus.AddChild(a);
            folded = plus.FoldConstants();
            Assert.AreEqual("1+a-520*b", folded.ToString());

            var enode = new PowerNode(c10, c1);
            folded = enode.FoldConstants();
            Assert.AreEqual("10", folded.ToString());


            enode = new PowerNode(c10, c0);
            folded = enode.FoldConstants();
            Assert.AreEqual("1", folded.ToString());
        }

        [TestCase]
        public void TrigTest()
        {
            var c0 = new ConstNode(0);
            var c1 = new ConstNode(1);
            var cp5 = new ConstNode(.5);
            var cp = new ConstNode(Math.PI);
            var a = new VarNode("a");
            SymNode en;

            en = new CosNode(c0).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());
            en = new CosNode(c0).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());

            en = new SinNode(c0).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0", en.ToString());
            en = new SinNode(c0).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0", en.ToString());

            en = new CosNode(cp).FoldConstants();
            Assert.AreEqual(NodeTypes.Cos, en.Type);
            Assert.AreEqual("cos(3.14159265358979)", en.ToString());
            en = new CosNode(cp).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("-1", en.ToString());

            en = new SinNode(cp).FoldConstants();
            Assert.AreEqual(NodeTypes.Sin, en.Type);
            Assert.AreEqual("sin(3.14159265358979)", en.ToString());
            en = new SinNode(cp).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1.22460635382238E-16", en.ToString());

            var prod = new ProdNode(cp, cp5);

            en = new CosNode(prod).FoldConstants();
            Assert.AreEqual(NodeTypes.Cos, en.Type);
            Assert.AreEqual("cos(1.5707963267949)", en.ToString());
            en = new CosNode(prod).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("6.12303176911189E-17", en.ToString());

            en = new SinNode(prod).FoldConstants();
            Assert.AreEqual(NodeTypes.Sin, en.Type);
            Assert.AreEqual("sin(1.5707963267949)", en.ToString());
            en = new SinNode(prod).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());

            prod.AddChild(a);

            en = new CosNode(prod).FoldConstants();
            Assert.AreEqual(NodeTypes.Cos, en.Type);
            Assert.AreEqual("cos(1.5707963267949*a)", en.ToString());
            en = new CosNode(prod).Evaluate();
            Assert.AreEqual(NodeTypes.Cos, en.Type);
            Assert.AreEqual("cos(1.5707963267949*a)", en.ToString());

            en = new SinNode(prod).FoldConstants();
            Assert.AreEqual(NodeTypes.Sin, en.Type);
            Assert.AreEqual("sin(1.5707963267949*a)", en.ToString());
            en = new SinNode(prod).Evaluate();
            Assert.AreEqual(NodeTypes.Sin, en.Type);
            Assert.AreEqual("sin(1.5707963267949*a)", en.ToString());


        }

        [TestCase]
        public void ExponentTest()
        {
            var c0 = new ConstNode(0);
            var c1 = new ConstNode(1);
            var cm1 = new ConstNode(-1);
            var c2 = new ConstNode(2);
            var c2p5 = new ConstNode(2.5);
            var c4 = new ConstNode(4);
            var cm2 = new ConstNode(-2);
            var cm4 = new ConstNode(-4);
            var c10 = new ConstNode(10);
            var cm13 = new ConstNode(-13);
            var a = new VarNode("a");
            SymNode en;

            en = new PowerNode(c1, c10).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());

            en = new PowerNode(c1, c10).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());
            en = new PowerNode(c1, c10).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1", en.ToString());

            en = new PowerNode(c4, c10).FoldConstants();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("4^10", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("1048576", en.ToString());

            en = new PowerNode(c0, cm1).FoldConstants();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("0^-1", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("0^-1", en.ToString());

            en = new PowerNode(c2p5, c2).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("6.25", en.ToString());
            en = new PowerNode(c2p5, c2).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("6.25", en.ToString());

            en = new PowerNode(c2p5, c4).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("39.0625", en.ToString());
            en = new PowerNode(c2p5, c4).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("39.0625", en.ToString());

            en = new PowerNode(c2p5, cm1).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0.4", en.ToString());
            en = new PowerNode(c2p5, cm1).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0.4", en.ToString());

            en = new PowerNode(c2p5, cm2).FoldConstants();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0.16", en.ToString());
            en = new PowerNode(c2p5, cm2).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("0.16", en.ToString());


            en = new PowerNode(c2, c2p5).FoldConstants();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("2^2.5", en.ToString());
            en = new PowerNode(c2, c2p5).Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);
            Assert.AreEqual("5.65685424949238", en.ToString());

            en = new PowerNode(c4, a).FoldConstants();
            Assert.AreEqual("4^a", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("4^a", en.ToString());

            var plus = new PlusNode(c4, c2p5);
        
            en = new PowerNode(plus, a).FoldConstants();
            Assert.AreEqual("6.5^a", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("6.5^a", en.ToString());

            en = new PowerNode(a, plus).FoldConstants();
            Assert.AreEqual("a^6.5", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("a^6.5", en.ToString());

            var prod = new ProdNode(c10, c2p5, cm13);

            en = new PowerNode(prod, plus).FoldConstants();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("-325^6.5", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Power, en.Type);  // Doesn't evaluate cause negative base.
            Assert.AreEqual("-325^6.5", en.ToString());

            prod.AddChild(cm1);
            en = new PowerNode(prod, plus).FoldConstants();
            Assert.AreEqual(NodeTypes.Power, en.Type);
            Assert.AreEqual("325^6.5", en.ToString());
            en = en.Evaluate();
            Assert.AreEqual(NodeTypes.Constant, en.Type);  // Doesn't evaluate cause negative base.
            Assert.AreEqual("2.12442716630506E+16", en.ToString());

        }
    }
}
