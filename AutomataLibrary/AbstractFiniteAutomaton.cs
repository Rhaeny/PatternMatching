using System.Collections.Generic;

namespace AutomataLibrary
{
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

        protected AbstractFiniteAutomaton()
        {
            throw new System.NotImplementedException();
        }

        public abstract string GetGraphString();
    }
}
