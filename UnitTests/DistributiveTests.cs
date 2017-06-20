using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;

namespace UnitTests
{
    [TestFixture]
    public class DistributiveTests
    {
        DistributiveTransform dist_trans = new DistributiveTransform();
        TransformsList tlist = new TransformsList();

        [TestCase]
        public void SimpleDistributive()
        {

            var a = new VarNode("a");
            var b = new VarNode("b");
            var c = new VarNode("c");

            var plus = new PlusNode(b, c);
            var prod = new ProdNode(a, plus);

            var result = dist_trans.Apply(prod);

            Assert.AreEqual("a*b+a*c", result.ToStringSorted());

        }

        [TestCase]
        public void E1Distributive()
        {
            var exp = new Exp1();
            SymNode result;

            // Starting expression is (a+b)*(c+d*e+f)*cos(g^h)
            result = dist_trans.Apply(exp.Root);
            Assert.AreEqual("a*(c+d*e+f)*cos(g^h)+b*(c+d*e+f)*cos(g^h)", result.ToString());

            var expanded  = tlist.Expand(exp.Root);
            var merged = tlist.Simplify(expanded);
            Assert.AreEqual("a*c*cos(g^h)+a*d*e*cos(g^h)+a*f*cos(g^h)+b*c*cos(g^h)+b*d*e*cos(g^h)+b*f*cos(g^h)", merged.ToStringSorted());
        }
    }
}
