using System;
using System.Collections.Generic;

namespace BitParallelismLibrary
{
    public class LevenshteinDistanceNFASimulator
    {
        public int AcceptInput(string pattern, int k, string input)
        {
            int matches = 0;
            ulong[,] r = new ulong[k + 1, input.Length + 1];
            Dictionary<char, ulong> d = new Dictionary<char, ulong>();
            SortedSet<char> mAlphabet = new SortedSet<char>();
            for (char c = (char)000; c <= (char)255; c++)
            {
                mAlphabet.Add(c);
            }
            foreach (var a in mAlphabet)
            {
                ulong v = 0;
                for (int j = pattern.Length - 1; j >= 0; j--)
                {
                    if (pattern[pattern.Length - j - 1] == a)
                    {
                        v |= (ulong)0 << j;
                    }
                    else
                    {
                        v |= (ulong)1 << j;
                    }
                }
                d.Add(a, v);
            }
            ulong r0 = 0;
            for (int j = pattern.Length - 1; j >= 0; j--)
            {
                r0 |= (ulong)1 << j;
            }
            for (int l = 0; l <= k; l++)
            {
                r[l, 0] = r0;
            }
            for (int i = 0; i < input.Length; i++)
            {
                ulong ti;
                if (d.TryGetValue(input[i], out ti))
                {
                    r[0, i + 1] = (r[0, i] >> 1) | ti;
                    if ((k == 0) && ((r[0, i + 1] & 1) == 0))
                    {
                        matches++;
                    }
                }
            }
            for (int l = 1; l <= k; l++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    ulong ti;
                    if (d.TryGetValue(input[i], out ti))
                    {
                        r[l, i + 1] = ((r[l, i] >> 1) | ti) & ((r[l - 1, i] & r[l - 1, i + 1]) >> 1) & (r[l - 1, i] | 1);
                        if ((l == k) && ((r[l, i + 1] & 1) == 0))
                        {
                            matches++;
                        }
                    }
                }
            }
            return matches;
        }
    }
}
