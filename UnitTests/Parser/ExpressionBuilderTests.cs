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
            Tokenizer t;
            List<Token> l;
            var functions = new HashSet<string>();
            functions.Add("sin");
            functions.Add("cos");

            t = new Tokenizer("a*b+d4.53(xs/tt)", functions);
            l = t.GetTokenList();

            Tuple<string, string>[] tests =
            {
                Tuple.Create("f + 3", "3+f"),       // Note that constants are formatted first
                Tuple.Create("+3", "3"),
                Tuple.Create("+3--2", "3-(-2)"),
                Tuple.Create("sin(2)", "sin(2)"),
                Tuple.Create("2+3", "2+3"),
                Tuple.Create("cos(f) + 3", "3+cos(f)"),
                Tuple.Create("sin(cos(4*f) + 3 - cos(4*x)/23)","sin(3+cos(4*f)-(cos(4*x)*23^-1))"),
            };

            foreach (var s in tests)
            {
                t = new Tokenizer(s.Item1, functions);
                l = t.GetTokenList();

                ExpressionBuilder eb = new ExpressionBuilder(l);

                var node = SymNodeBuilder.CreateNodes(eb.Parse())[0];
                eb.Print();

                Assert.AreEqual(s.Item2, node.ToString());
            }

        }

        [TestCase]
        public void TestFailParse()
        {

            Tokenizer t;
            List<Token> l;
            var functions = new HashSet<string>();
            functions.Add("sin");
            functions.Add("cos");

            t = new Tokenizer("a*b+d4.53(xs/tt)", functions);
            l = t.GetTokenList();

            string[] tests =
            {
                "f**3",
                "x*(4+f",
                "x*4+f)",
                //"x x",
             };


            foreach (var s in tests)
            {
                t = new Tokenizer(s, functions);
                l = t.GetTokenList();

                ExpressionBuilder eb = new ExpressionBuilder(l);

                Assert.Throws(typeof(ApplicationException), () => eb.Parse());
            }

        }
    }
}
