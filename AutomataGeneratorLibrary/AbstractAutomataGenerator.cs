using System;
using System.Collections.Generic;
using System.Linq;
using AutomataLibrary;

namespace AutomataGeneratorLibrary
{
    public abstract class AbstractAutomataGenerator
    {
        protected NFA NFA;

        protected SortedSet<char> MAlphabet;
        protected SortedSet<int> MStates;
        protected int MInitialState;
        protected SortedSet<int> MFinalStates;
        protected List<Tuple<int, string, int>> DeltaItems;
        protected List<Tuple<int, int>> EpsilonItems;

        protected AbstractAutomataGenerator(string pattern)
        {
            MAlphabet = new SortedSet<char>(pattern.Distinct());
            MStates = new SortedSet<int>();
            DeltaItems = new List<Tuple<int, string, int>>();
            EpsilonItems = new List<Tuple<int, int>>();
            MFinalStates = new SortedSet<int>();
        }

        public NFA GetAutomata()
        {
            return NFA;
        }
    }
}
