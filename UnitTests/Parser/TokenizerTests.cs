using System;
using System.Collections.Generic;
using NUnit.Framework;
using SSMS;
using SSMS.Parser;

namespace UnitTests.Parser
{
    [TestFixture]
    public class TokenizerTests
    {

        [TestCase]
        public void TestTokenize()
        {


            Tokenizer t;
            List<Token> l;
            var functions = new HashSet<string>();
            functions.Add("sin");
            functions.Add("cos");


            string[] tests =
            {
                    "+3",
                    "+3--2",
                    "sin(2,3)",
                    "sin(,,)",
                    "sin(3,)",
                    "sin(cos(4))",
                    "sin(cos(4*f) + 3, cos(4*x)/23)",
                    "sin(2)",
                    "4+2",
                    "1+2+3",
                    "1^2^3",
                    "(4+2)",
                    "4+2*5",
                    "4+2*(5+3+3)",
                    "sin(,2,3)",
                    "a*b+d4..53(xs/tt)",    // Okay - tokenizes asd 4. and then .53
                    @"d_aa/\d_b",
                    "d_4d_4",               // Not valid to parse, but should tokenize
                    ""
                };

            foreach (var s in tests)
            {
                t = new Tokenizer(s, functions);
                l = t.GetTokenList();

                string st = s.Replace(" ", "");
                Assert.AreEqual(st, l.ConcatTokens());
            }

        }

        [TestCase]
        public void TestFailTokenize()
        {


            Tokenizer t;
            var functions = new HashSet<string>();
            functions.Add("sin");
            functions.Add("cos");


            string[] tests =
            {
                    "@",
                    "a*b+d..53(xs/tt)",     // Inavlid number
                    ".................",
                };

            foreach (var s in tests)
            {
                t = new Tokenizer(s, functions);
                Assert.Throws(typeof(ApplicationException), () => t.GetTokenList());
            }

        }
    }
}
