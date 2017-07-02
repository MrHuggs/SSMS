using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;
using SSMS.Parser;

namespace UnitTests.Parser
{
    [TestFixture]
    public class ExpressionBuilderTests
    {

        [TestCase]
        public void TestParse()
        {

            Tuple<string, string>[] tests =
            {
                Tuple.Create("f + 3", "3+f"),       // Note that constants are formatted first
                Tuple.Create("+3", "3"),
                Tuple.Create("+3--2", "3--2"),
                Tuple.Create("sin(2)", "sin(2)"),
                Tuple.Create("2+3", "2+3"),
                Tuple.Create("2^2", "2^2"),
                Tuple.Create("sin(x)^2", "sin(x)^2"),
                Tuple.Create("cos(f) + 3", "3+cos(f)"),
                Tuple.Create("sin(cos(4*f) + 3 - cos(4*x)/23)","sin(3+cos(4*f)-(cos(4*x)*23^-1))"),
                Tuple.Create("sin(x)^2 + cos(x)^2 / (x + y + x)", "sin(x)^2+cos(x)^2*(x+y+x)^-1"),

                Tuple.Create(@"d_a/\d_b", @"d_a/\d_b"),
                Tuple.Create(@"c*d*d_a/\(x+y)*d_b", @"(c*d)*d_a/\(x+y)*d_b"),
            };

            for (int i = tests.Length - 1; i >= 0; i--)
            {
                var s = tests[i];
                var node = SymNodeBuilder.ParseString(s.Item1);

                Assert.AreEqual(s.Item2, node.ToString());
            }

        }

        [TestCase]
        public void TestFailParse()
        {

            string[] tests =
            {
                "sin(13, 3)",
                "4*(x x)",
                "(x x)",
                "f**3",
                "x*(4+f",
                "x*4+f)",
                "d_8",
                "d_+",
                "d_",
                "d_d_",
                "d_x*d_y",
                "sin(d_x)",
                "d_x^2",
                "2^d_x",
             };


            foreach (var s in tests)
            {
                bool failed = false;
                try
                {
                    SymNodeBuilder.ParseString(s);
                }
                catch (Exception)
                {
                    failed = true;
                }

                Assert.AreEqual(true, failed);
            }

        }
    }
}
