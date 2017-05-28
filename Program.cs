//
// Driver for ths Simple Symbolic Math System (SSMS)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    class Program
    {
        static void Main(string[] args)
        {
            ProdNodeTest();
        }

        static public void ProdNodeTest()
        {
            var n = new ProdNode();

            n.AddChild(new VarNode("b"));
            n.AddChild(new VarNode("a"));
            n.AddChild(new VarNode("d"));
            n.AddChild(new VarNode("c"));

            StringBuilder sb = new StringBuilder();

            n.Format(sb);
            var s = sb.ToString();
            Console.WriteLine(s);

            Debug.Assert(s == "(a b c d)");

            n.AddChild(new ConstNode(24));
            n.AddChild(new ConstNode(-3));
            n.AddChild(new VarNode("ab"));

            sb.Clear();
            n.Format(sb);
            s = sb.ToString();
            Console.WriteLine(s);
        }
    }
}
