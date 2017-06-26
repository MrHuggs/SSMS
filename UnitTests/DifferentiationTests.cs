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
                Tuple.Create("sin(x)^2", "x", "2*cos(x)*sin(x)"),
                Tuple.Create("sin(x)^2", "y", "0"),
                Tuple.Create("a/b", "a", "b^-1"),
                Tuple.Create("a/b", "b", "-a*b^-2"),
                Tuple.Create("(x^2+y^2)^(1/2)", "x", "x*(x^2+y^2)^-.5"),
                Tuple.Create("(x^2+y^2)^(1/2)", "y", "y*(x^2+y^2)^-.5"),
                Tuple.Create("(1-(x^2+y^2)^(1/2))^3", "x", " -(3*x*(1-(x^2 + y^2)^.5)^2)*(x^2 + y^2)^-.5"),
                Tuple.Create("x^3+x+3", "x", "3*x^2+1"),
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
    }
}
