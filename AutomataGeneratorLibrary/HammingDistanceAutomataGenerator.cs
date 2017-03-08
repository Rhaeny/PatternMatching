using System;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public class HammingDistanceAutomataGenerator : AbstractAutomataGenerator
    {
        protected int Qsize;

        public HammingDistanceAutomataGenerator(string pattern, int k)
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                MAlphabet.Add(c);
            }

            Qsize = (int)Math.Round((k + 1) * (pattern.Length + 1 - (double)k / 2), MidpointRounding.AwayFromZero);

            int l = 0;
            int r = 0;

            for (int i = 0; i < Qsize; i++)
            {
                MStates.Add(i);
                if (r >= pattern.Length)
                {
                    MFinalStates.Add(i);
                    l = l + 1;
                    r = l;
                }
                else
                {
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                        foreach (var c in MAlphabet)
                        {
                            DeltaItems.Add(new Tuple<int, string, int>(i, c.ToString(), s));
                            if (i == 0)
                            {
                                DeltaItems.Add(new Tuple<int, string, int>(i, c.ToString(), i));
                            }
                        }
                    }
                    else
                    {
                        DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i + 1));
                    }
                    r = r + 1;
                }
            }
            MInitialState = 0;
            NFA = new NFA(MAlphabet, MStates, DeltaItems, EpsilonItems, MInitialState, MFinalStates);
        }
    }
}
