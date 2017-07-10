//
// Driver for ths Simple Symbolic Math System (SSMS)

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SSMS.Nodes;
using SSMS.Parser;

namespace SSMS
{
    class Program
    {


        static int Main(string[] args)
        {
            int rval;

            //rval = Problems.Lee14_6.Execute();
            rval = Problems.Lee14_7.Execute();


            return rval;
        }
    }
}
