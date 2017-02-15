using System;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public class LevenshteinDistanceAutomataGenerator : AbstractAutomataGenerator
    {
        protected int M;

        public LevenshteinDistanceAutomataGenerator(string pattern, int k) : base(pattern)
        {
            M = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);

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
    }
}
