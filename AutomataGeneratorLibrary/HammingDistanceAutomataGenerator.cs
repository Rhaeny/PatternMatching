using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public class HammingDistanceAutomataGenerator
    {
        public NFA NFA;
        protected int M;

        protected SortedSet<char> MAlphabet;
        protected SortedSet<int> MStates;
        protected int MInitialState;
        protected SortedSet<int> MFinalStates;
        protected List<Tuple<int, string, int>> DeltaItems;
        protected List<Tuple<int, int>> EpsilonItems = new List<Tuple<int, int>>();

        public HammingDistanceAutomataGenerator(string pattern, int k)
        {
            M = (int)Math.Round((k + 1)*(pattern.Length + 1 - (double)k/2), MidpointRounding.AwayFromZero);

            MAlphabet = new SortedSet<char>(pattern.Distinct());

            MStates = new SortedSet<int>();
            for (int i = 0; i < M; i++)
            {
                MStates.Add(i);
            }

            MInitialState = MStates.ElementAt(0);

            DeltaItems = new List<Tuple<int, string, int>>();
            EpsilonItems = new List<Tuple<int, int>>();
            MFinalStates = new SortedSet<int>();
            int l = 0;
            int r = 0;
            for (int i = 0; i < pattern.Length; i++)
            {
                if (r > pattern.Length)
                {
                    MFinalStates.Add(i);
                    l = l + 1;
                    r = l + 1;
                }
                else
                {
                    DeltaItems.Add(new Tuple<int, string, int>(i, pattern[r].ToString(), i+1));
                    if (l < k)
                    {
                        int s = i + pattern.Length + 1 - l;
                        EpsilonItems.Add(new Tuple<int, int>(i, s));
                    }
                    r = r + 1;
                }
            }
            EpsilonItems.Add(new Tuple<int, int>(0, 0));
            NFA = new NFA(MAlphabet, MStates, DeltaItems, EpsilonItems, MInitialState, MFinalStates);
        }
    }
}
