using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Nodes;
using SSMS.Parser;
using SSMS.Transforms;

namespace UnitTests
{
    [TestFixture]
    public class PullbackTests
    {
        [TestCase]
        public void SimplePullbackTest()
        {
            var target = SymNodeBuilder.ParseString("y*d_x");

            var newars = new List<string>(new string[] { "s", "t" });

            Tuple<string, string>[] _subs =
            {
                Tuple.Create("x", "s*t"),
                Tuple.Create("y", "t^2"),
            };

            var subs = new List<Tuple<string, SymNode>>();
            foreach (var t in _subs)
                subs.Add(new Tuple<string, SymNode>(t.Item1, SymNodeBuilder.ParseString(t.Item2)));

            var result = Pullback.Compute(newars, subs, target);

            result = TransformsList.Inst().Expand(result);
            result = TransformsList.Inst().Simplify(result);
            Assert.AreEqual("t^3*d_s+s*t^2*d_t", result.ToStringSorted());
        }

        class TestCase
        {
            public string Target;
            public List<string> NewVars;
            string[,] Subs;
            public string Result;

            public TestCase(string target, List<string> vars, string[,] subs, string result)
            {
                Target = target;
                NewVars = vars;
                Subs = subs;
                Result = result;
            }

            public void RunTest()
            {
                SymNode target = SymNodeBuilder.ParseString(Target);

                var sublist = new List<Tuple<string, SymNode>>();
                for (int row = 0; row < Subs.Length / 2; row++)
                {
                    var exp = SymNodeBuilder.ParseString(Subs[row, 1]);
                    sublist.Add(Tuple.Create(Subs[row, 0], exp));
                }

                SymNode result = SymNodeBuilder.ParseString(Result);
                result = TransformsList.Inst().TrySimplify(result);


                var pullback_result  = Pullback.Compute(NewVars, sublist, target);

                var pullback_result_e = TransformsList.Inst().TryExpand(pullback_result);
                var pullback_result_s = TransformsList.Inst().TrySimplify(pullback_result_e);
                Assert.AreEqual(result.IsEqual(pullback_result_s), true);
            }
        }

        [TestCase]
        public void PullbackTest()
        {

            TestCase[] tests =
                {
                    new TestCase( "y*d_x", new List<string> {"s", "t"}, new string[3,2] { { "x", "s*t" }, { "y", "t^2" }, { "z", "s+t" } } , "t^3*d_s+s*t^2*d_t"),
                    new TestCase(       // Example 14.18 from Lee
                        @"y*d_x /\d_z+x*d_y/\d_z", 
                        new List<string> {"u", "v"}, 
                        new string[3,2] { { "x", "u" }, { "y", "v" }, { "z", "u^2-v^2" } } ,
                        @"-2*u^2*d_u/\d_v-2*v^2*d_u/\d_v"),
                };

            foreach (var tc in tests)
            {
                tc.RunTest();
            }

        }
    }
}
