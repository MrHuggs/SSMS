using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    public class VarNode : SymNode
    {
        public VarNode(String var)
        {
            Type = NodeTypes.Var;
            Var = var;
        }

        public String Var;

        public override void Format(FormatBuilder fb)
        {
            fb.Append(Var);
        }

        public override bool IsEqual(SymNode other)
        {
            if (other.Type != NodeTypes.Var)
                return false;

            return ((VarNode)other).Var == Var;
        }

        public override SymNode DeepClone()
        {
            return new VarNode(Var);
        }

        public override SymNode FoldConstants()
        {
            return new VarNode(Var);
        }

        public override SymNode Evaluate()
        {
            return new VarNode(Var);
        }

        public override SymNode Differentiate(string var)
        {
            if (var == Var)
                return new ConstNode(1);

            return new ConstNode(0);
        }
    }
}
