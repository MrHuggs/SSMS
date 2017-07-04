using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SSMS.Nodes
{
    static class NodeExtensions
    {
        public static bool IsEqualNoReorder(this List<SymNode> value, List<SymNode> other)
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
        public static bool IsEqualReorder(this List<SymNode> value, List<SymNode> other)
        {
            if (value.Count != other.Count)
                return false;

            bool[] used = new bool[value.Count];

            for (int i = 0; ; i++)
            {
                if (i == value.Count)
                    return true;

                for (int j = 0; ; j++)
                {
                    if (j == value.Count)
                        return false;

                    if (used[j])
                        continue;
                    if (value[i].IsEqual(other[j]))
                    {
                        used[j] = true;
                        break;
                    }
                }
            }
        }

        public static List<SymNode> ShallowCopy(this List<SymNode> value)
        {
            var result = new List<SymNode>();
            value.ForEach(node => result.Add(node));
            return result;
        }


        public static bool IsFunction(this NodeTypes type)
        {
            switch(type)
            {
                case NodeTypes.Cos:
                case NodeTypes.Sin:
                case NodeTypes.Tan:
                    return true;
                default:
                    return false;
            }
        }
    }
}
