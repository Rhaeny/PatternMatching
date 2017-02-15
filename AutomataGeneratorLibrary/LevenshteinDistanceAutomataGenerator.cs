using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public class LevenshteinDistanceAutomataGenerator
    {
        protected NFA NFA;
        protected int M;

        protected SortedSet<char> MAlphabet;
        protected SortedSet<int> MStates;
        protected int MInitialState;
        protected SortedSet<int> MFinalStates;
        protected List<Tuple<int, string, int>> DeltaItems;
        protected List<Tuple<int, int>> EpsilonItems;

        public LevenshteinDistanceAutomataGenerator(string pattern, int k)
        {
            M = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);

            MAlphabet = new SortedSet<char>(pattern.Distinct());
            MStates = new SortedSet<int>();
            DeltaItems = new List<Tuple<int, string, int>>();
            EpsilonItems = new List<Tuple<int, int>>();
            MFinalStates = new SortedSet<int>();

            int l = k;
            int r = pattern.Length;

            for (int i = M - 1; i >= 0; i--)
            {
                MStates.Add(i);
                if (r == pattern.Length)
                {
                    MFinalStates.Add(i);
                }
                else
                {
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                        foreach (var c in MAlphabet)
                        {
                            if (c != pattern[r])
                            {
                                DeltaItems.Add(new Tuple<int, string, int>(i, c.ToString(), s));
                                if (i == 0)
                                {
                                    DeltaItems.Add(new Tuple<int, string, int>(i, c.ToString(), i));
                                }
                            }
                        }
                        if (r > l)
                        {
                            foreach (var c in MAlphabet)
                            {
                                if (c != pattern[r])
                                {
                                    DeltaItems.Add(new Tuple<int, string, int>(i, c.ToString(), s - 1));
                                }
                            }
                        }
                        EpsilonItems.Add(new Tuple<int, int>(i, s));
                    }
                    else
                    {
                        DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                    }
                }
                r = r - 1;
                if (r < l)
                {
                    r = pattern.Length;
                    l = l - 1;
                }
            }
            MInitialState = 0;
            NFA = new NFA(MAlphabet, MStates, DeltaItems, EpsilonItems, MInitialState, MFinalStates);
        }

        public NFA GetAutomata()
        {
            return NFA;
        }
    }
}
