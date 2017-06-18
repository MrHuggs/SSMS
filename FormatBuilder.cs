using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SSMS
{
    // This is a replacement for string builder, used when we format expressions:
    public class FormatBuilder
    {
        StringBuilder Builder = new StringBuilder();

        public void Append(string str, bool parenthesize = false)
        {
            if (parenthesize)
            {
                Builder.Append('(');
                Builder.Append(str);
                Builder.Append(')');
            }
            else
                Builder.Append(str);
        }

        public void Append<T>(T t, bool parenthesize = false)
        {
            Append(t.ToString(), parenthesize);
        }


        public void Clear()
        {
            Builder.Clear();
        }

        public override string ToString()
        {
            string raw = Builder.ToString();

            StringBuilder final_string = new StringBuilder();

            // Do some clean up to remove redundant characters.
            // This would be faster stepping through the string, but less clear.
            raw = raw.Replace("+-", "-");
            raw = raw.Replace("-+", "+");
            raw = raw.Replace("(+", "(");
            raw = raw.Replace("-1*", "-");
            if (raw[0] == '+')
                raw = raw.Remove(0, 1);

            return raw;
        }
    }
}
