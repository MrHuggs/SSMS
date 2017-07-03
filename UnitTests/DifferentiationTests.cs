using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;
using SSMS.Parser;


namespace UnitTests
{
    [TestFixture]
    public class DifferentiationTests
    {
        [TestCase]
        public void SimpleDiff()
        {

            Tuple<string, string, string>[] tests =
            {
                Tuple.Create("x^2", "x", "2*x"),
                Tuple.Create("(2*x)^2", "x", "8*x"),
                Tuple.Create("sin(x)^2", "x", "2*sin(x)*cos(x)"),
                Tuple.Create("cos(x)^2", "x", "-2*cos(x)*sin(x)"),
                Tuple.Create("sin(x)^2", "y", "0"),
                Tuple.Create("a/b", "a", "b^-1"),
                Tuple.Create("a/b", "b", "-a*b^-2"),
                Tuple.Create("(x^2+y^2)^(1/2)", "x", "x*(x^2+y^2)^-.5"),
                Tuple.Create("(x^2+y^2)^(1/2)", "y", "y*(x^2+y^2)^-.5"),
                Tuple.Create("(1-(x^2+y^2)^(1/2))^3", "x", " -(3*x*(1-(x^2 + y^2)^.5)^2)*(x^2 + y^2)^-.5"),
                Tuple.Create("x^3+x+3", "x", "3*x^2+1"),
                Tuple.Create("x^3+x", "x", "3*x^2+1"),
           };

            for (int i = tests.Length - 1; i >= 0; i--)
            {
                var s = tests[i];
                var node = SymNodeBuilder.ParseString(s.Item1);
                var snode = TransformsList.Inst().TrySimplify(node);
                var diff = snode.Differentiate(s.Item2);
                var sdiff = TransformsList.Inst().TrySimplify(diff);

                var target = SymNodeBuilder.ParseString(s.Item3);
                target = TransformsList.Inst().TrySimplify(target);

                Assert.AreEqual(target.ToStringSorted(), sdiff.ToStringSorted());
                Assert.AreEqual(target.IsEqual(sdiff), true);
            }
        }

        [TestCase]
        public void FindVarsTest()
        {
            SymNode root = new Exp1().Root;
            var list = Differential.FindVariables(root);

            for (char c = 'a'; c <= 'z'; c++)
                Assert.AreEqual(c <= 'h', list.Contains(c.ToString()));
        }

        [TestCase]
        public void DifferentialTestExp1()
        {
            SymNode root = new Exp1().Root;

            // Exp1 uses a non-const power, so get replace it:
            root = Substitution.Substitute(root, new VarNode("h"), new ConstNode(4));
            Assert.AreEqual("(a+b)*(c+d*e+f)*cos(g^4)", root.ToString());

            var diff = Differential.Compute(root);
            diff.Sort();
            Assert.AreEqual(diff.ToStringSorted(), "(c+d*e+f)*cos(g^4)*d_a+(c+d*e+f)*cos(g^4)*d_b+(a+b)*cos(g^4)*d_c+(a+b)*e*cos(g^4)*d_d+(a+b)*d*cos(g^4)*d_e+(a+b)*cos(g^4)*d_f-4*(a+b)*(c+d*e+f)*g^3*sin(g^4)*d_g");

        }

        [TestCase]
        public void DifferentialTest()
        {
            Tuple<string, string>[] tests =
            {
                Tuple.Create("0", "0"),
                Tuple.Create("0*x", "0"),
                Tuple.Create("x^0", "0"),
                Tuple.Create("x^1", "d_x"),
                Tuple.Create("y+x", "d_x+d_y"),
                Tuple.Create(".5*x^2", "x*d_x"),
                Tuple.Create("x*y", "y*d_x+x*d_y"),
                Tuple.Create("r*sin(phi)*cos(theta)", "r*cos(phi)*cos(theta)*d_phi+sin(phi)*cos(theta)*d_r-r*sin(phi)*sin(theta)*d_theta"),
                Tuple.Create("r*sin(phi)*sin(theta)", "r*cos(phi)*sin(theta)*d_phi+sin(phi)*sin(theta)*d_r+r*sin(phi)*cos(theta)*d_theta"),
                Tuple.Create("r*cos(phi)", "-r*sin(phi)*d_phi+cos(phi)*d_r"),
            };

            for (int i = tests.Length - 1; i >= 0; i--)
            {
                var s = tests[i].Item1;
                var node = SymNodeBuilder.ParseString(s);
                var diff = Differential.Compute(node);
                diff.Sort();
                Assert.AreEqual(tests[i].Item2, diff.ToString());
            }

        }
    }
}
