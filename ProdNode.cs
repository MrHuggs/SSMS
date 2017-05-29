﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    class ProdNode : CommutativeNode
    {
        public ProdNode()
        {
           Type = NodeTypes.Prod;
        }

        public override void Format(StringBuilder sb)
        {
            bool first = true;
            foreach (var node in Children)
            {
                if (first)
                    first = false;
                else
                    sb.Append(" ");

                if (node.Type > NodeTypes.Prod)
                {
                    // If its lower precedence, then surround it by ():
                    sb.Append("(");
                    node.Format(sb);
                    sb.Append(")");
                }
                else
                {
                    node.Format(sb);
                }
            }
        }
           

        public override SymNode DeepClone()
        {
            var n = new ProdNode();
            n.DeepCloneChildren(this);

            return n;
        }
    }
}
