using System.Collections.Generic;
using System.IO;

namespace BitParallelismLibrary
{
    /// <summary>
    /// Hamming distance NFA simulator.
    /// </summary>
    public class HammingDistanceNFASimulator
    {
        /// <summary>
        /// Simulates run of sigma version NFA based on <see cref="pattern"/>, <see cref="k"/> and <see cref="input"/> 
        /// parameters. Finds and returns count of matches in input string by using bit parallelism simulation method.
        /// </summary>
        /// <param name="pattern">The pattern of automaton.</param>
        /// <param name="k">Maximum number of errors.</param>
        /// <param name="input">Input text for automaton.</param>
        /// <returns>Count of matches.</returns>
        public int AcceptInput(string pattern, int k, string input)
        {
            int matches = 0;
            ulong[,] r = new ulong[k + 1, input.Length + 1];
            ulong[] d = new ulong[256];
            SortedSet<char> mAlphabet = new SortedSet<char>();
            for (char c = (char)000; c <= (char)255; c++)
            {
                mAlphabet.Add(c);
            }
            foreach (var a in mAlphabet)
            {
                ulong v = 0;
                for (int j = 0; j < pattern.Length; j++)
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
                d[a] = v;
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
                ulong ti = d[input[i]];
                r[0, i + 1] = (r[0, i] >> 1) | ti;
                if ((k == 0) && ((r[0, i + 1] & 1) == 0))
                {
                    matches++;
                }
            }
            for (int l = 1; l <= k; l++)
            {
                for (int i = 0; i < input.Length; i++)
                {
                    ulong ti = d[input[i]];
                    r[l, i + 1] = ((r[l, i] >> 1) | ti) & (r[l - 1, i] >> 1);
                    if ((l == k) && ((r[l, i + 1] & 1) == 0))
                    {
                        matches++;
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Simulates run of sigma version NFA based on <see cref="pattern"/>, <see cref="k"/> and <see cref="filePath"/> 
        /// parameters. Finds and returns count of matches in file by using bit parallelism simulation method.
        /// </summary>
        /// <param name="pattern">The pattern of automaton.</param>
        /// <param name="k">Maximum number of errors.</param>
        /// <param name="filePath">Path of the file.</param>
        /// <returns>Count of matches.</returns>
        public int AcceptFile(string pattern, int k, string filePath)
        {
            int matches = 0;
            ulong[] d = new ulong[256];
            SortedSet<char> mAlphabet = new SortedSet<char>();
            for (char c = (char)000; c <= (char)255; c++)
            {
                mAlphabet.Add(c);
            }
            foreach (var a in mAlphabet)
            {
                ulong v = 0;
                for (int j = 0; j < pattern.Length; j++)
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
                d[a] = v;
            }
            ulong r0 = 0;
            for (int j = pattern.Length - 1; j >= 0; j--)
            {
                r0 |= (ulong)1 << j;
            }
            ulong[] rs = new ulong[k + 1];
            for (int l = 0; l <= k; l++)
            {
                rs[l] = r0;
            }
            using (StreamReader reader = new StreamReader(filePath))
            {
                char[] buffer = new char[1024];
                int read;
                while ((read = reader.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    ulong[,] r = new ulong[k + 1, read + 1];
                    for (int l = 0; l <= k; l++)
                    {
                        r[l, 0] = rs[l];
                    }
                    for (int i = 0; i < read; i++)
                    {
                        ulong ti = d[buffer[i]];
                        r[0, i + 1] = (r[0, i] >> 1) | ti;
                        if ((k == 0) && ((r[0, i + 1] & 1) == 0))
                        {
                            matches++;
                        }
                    }
                    for (int l = 1; l <= k; l++)
                    {
                        for (int i = 0; i < read; i++)
                        {
                            ulong ti = d[buffer[i]];
                            r[l, i + 1] = ((r[l, i] >> 1) | ti) & (r[l - 1, i] >> 1);
                            if ((l == k) && ((r[l, i + 1] & 1) == 0))
                            {
                                matches++;
                            }
                        }
                    }
                    for (int l = 0; l <= k; l++)
                    {
                        rs[l] = r[l, read];
                    }
                }
            }
            return matches;
        }
    }
}
