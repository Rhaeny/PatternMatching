using System;
using System.Collections.Generic;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    /// <summary>
    /// Levenshtein distance automata generator.
    /// </summary>
    public class LevenshteinDistanceAutomataGenerator
    {
        /// <summary>
        /// Generates complement version of <see cref="NFA"/> based on <see cref="pattern"/> and <see cref="k"/> parameters.
        /// </summary>
        /// <param name="pattern">The pattern of automaton.</param>
        /// <param name="k">Maximum number of errors.</param>
        /// <returns>Generated NFA.</returns>
        public NFA GenerateComplementVersionNFA(string pattern, int k)
        {
            SortedSet<char> mAlphabet = new SortedSet<char>();
            SortedSet<int> mStates = new SortedSet<int>();
            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>();
            List<Tuple<int, int>> epsilonItems = new List<Tuple<int, int>>();
            SortedSet<int> mFinalStates = new SortedSet<int>();
            if (k > pattern.Length)
            {
                k = pattern.Length;
            }
            for (char c = (char)000; c <= (char)255; c++)
            {
                mAlphabet.Add(c);
            }
            int qCount = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);
            int l = k;
            int r = pattern.Length;
            for (int i = qCount - 1; i >= 0; i--)
            {
                mStates.Add(i);
                if (r == pattern.Length)
                {
                    mFinalStates.Add(i);
                }
                else
                {
                    deltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        foreach (var a in mAlphabet)
                        {
                            if (a != pattern[r])
                            {
                                deltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s));
                            }
                        }
                        if (r > l)
                        {
                            foreach (var a in mAlphabet)
                            {
                                if (a != pattern[r])
                                {
                                    deltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s - 1));
                                }
                            }
                        }
                        epsilonItems.Add(new Tuple<int, int>(i, s));
                    }
                }
                r = r - 1;
                if (r < l)
                {
                    r = pattern.Length;
                    l = l - 1;
                }
            }
            foreach (var a in mAlphabet)
            {
                deltaItems.Add(new Tuple<int, string, int>(0, a.ToString(), 0));
            }
            return new NFA(mAlphabet, mStates, deltaItems, epsilonItems, 0, mFinalStates);
        }

        /// <summary>
        /// Generates sigma version of <see cref="NFA"/> based on <see cref="pattern"/> and <see cref="k"/> parameters.
        /// </summary>
        /// <param name="pattern">The pattern of automaton.</param>
        /// <param name="k">Maximum number of errors.</param>
        /// <returns>Generated NFA.</returns>
        public NFA GenerateSigmaVersionNFA(string pattern, int k)
        {
            SortedSet<char> mAlphabet = new SortedSet<char>();
            SortedSet<int> mStates = new SortedSet<int>();
            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>();
            List<Tuple<int, int>> epsilonItems = new List<Tuple<int, int>>();
            SortedSet<int> mFinalStates = new SortedSet<int>();
            if (k > pattern.Length)
            {
                k = pattern.Length;
            }
            for (char c = (char)000; c <= (char)255; c++)
            {
                mAlphabet.Add(c);
            }
            int qCount = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);
            int l = k;
            int r = pattern.Length;
            for (int i = qCount - 1; i >= 0; i--)
            {
                mStates.Add(i);
                if (r == pattern.Length)
                {
                    mFinalStates.Add(i);
                }
                else
                {
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        deltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                        deltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), s));
                        foreach (var a in mAlphabet)
                        {
                            if (a != pattern[r])
                            {
                                deltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s));
                            }
                        }
                        if (r > l)
                        {
                            foreach (var a in mAlphabet)
                            {
                                deltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s - 1));
                            }
                        }
                        epsilonItems.Add(new Tuple<int, int>(i, s));
                    }
                    else
                    {
                        deltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                    }
                }
                r = r - 1;
                if (r < l)
                {
                    r = pattern.Length;
                    l = l - 1;
                }
            }
            foreach (var a in mAlphabet)
            {
                deltaItems.Add(new Tuple<int, string, int>(0, a.ToString(), 0));
            }
            return new NFA(mAlphabet, mStates, deltaItems, epsilonItems, 0, mFinalStates);
        }
    }
}
