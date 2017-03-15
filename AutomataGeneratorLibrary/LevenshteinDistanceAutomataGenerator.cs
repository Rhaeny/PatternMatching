using System;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public class LevenshteinDistanceAutomataGenerator : AbstractAutomataGenerator
    {
        protected int Qsize;

        public LevenshteinDistanceAutomataGenerator(string pattern, int k)
        {
            if (k > pattern.Length)
            {
                k = pattern.Length;
            }

            for (char c = (char)000; c <= (char)255; c++)
            {
                MAlphabet.Add(c);
            }

            Qsize = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);

            int l = k;
            int r = pattern.Length;

            for (int i = Qsize - 1; i >= 0; i--)
            {
                MStates.Add(i);
                if (r == pattern.Length)
                {
                    MFinalStates.Add(i);
                }
                else
                {
                    if (i == 0)
                    {
                        foreach (var a in MAlphabet)
                        {
                            DeltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), i));
                        }
                    }
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                        foreach (var a in MAlphabet)
                        {
                            DeltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s));
                        }
                        if (r > l)
                        {
                            foreach (var a in MAlphabet)
                            {
                                DeltaItems.Add(new Tuple<int, string, int>(i, a.ToString(), s - 1));
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
