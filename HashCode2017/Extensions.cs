using System;
using System.Linq;

namespace HashCode2017 {
    public static class Extensions {
        public static int[] SplitByCharAndConvertToInts(this string str, char character) {
            return str
                .Split(new char[] { character })
                .Select(s => Convert.ToInt32(s))
                .ToArray();
        }
    }
}
