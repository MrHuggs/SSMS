using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSMS
{
    static class NodeExtensions
    {
        public static bool IsEqual(this List<SymNode> value, List<SymNode> other)
        {
            if (value.Count != other.Count)
                return false;

            for (int i = 0; i < value.Count; i++)
            {
                if (value[i].IsEqual(other[i]) == false)
                    return false;
            }
            return true;
        }
    }
}
