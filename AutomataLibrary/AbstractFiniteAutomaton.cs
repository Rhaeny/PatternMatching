using System;
using System.Collections.Generic;

namespace AutomataLibrary
{
    [Serializable]
    public abstract class AbstractFiniteAutomaton
    {
		protected SortedSet<char> MAlphabet;
		protected SortedSet<int> MStates;
		protected int MInitialState;
		protected SortedSet<int> MFinalStates;

        protected AbstractFiniteAutomaton(SortedSet<char> alphabet, SortedSet<int> states, int initialState, SortedSet<int> finalStates)
		{
			MAlphabet = alphabet;
			MStates = states;
			MInitialState = initialState;
			MFinalStates = finalStates;
		}

        public abstract void Accepts(string input);

        public abstract string GetGraphvizString();
    }
}
