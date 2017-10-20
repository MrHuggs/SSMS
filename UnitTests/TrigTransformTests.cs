using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;
using SSMS.Parser;

namespace UnitTests
{
    [TestFixture]
    public class TrigTransformTests
    {

        [TestCase]
        public void SimpleTrig()
        {

            Tuple<string, string>[] tests =
            {
                Tuple.Create(@"sin(x)^2+cos(x)^2", @"1"),
                Tuple.Create(@"a*sin(x)^2+cos(x)^2*a", @"a"),
                Tuple.Create(@"(1+x)*sin(x)^2+cos(x)^2*(x+1)", @"1+x"),
				Tuple.Create(@"2*sin(x)^2+2*cos(x)^2", @"2"),
			};

            for (int i = tests.Length - 1; i >= 0; i--)
            {
                var s = tests[i];
                var node = SymNodeBuilder.ParseString(s.Item1);

                var simple = Cos2Sin2Transform._Apply(node);
				simple.AssertValid();
                if (simple == null) simple = node;
                Assert.AreEqual(s.Item2, simple.ToString());

            }
        }
    }
}
