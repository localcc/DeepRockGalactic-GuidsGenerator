using System;
using System.Text;

namespace DeepRockGalactic_GuidsGenerator
{
    internal static class GuidExtensions
    {
        public static string ToStringCustom(this Guid guid)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var b in guid.ToByteArray())
            {
                sb.Append(string.Format("{0:x2}", b));
            }
            return sb.ToString().ToUpper();
        }
    }
}
