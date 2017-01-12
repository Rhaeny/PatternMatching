using System.Collections.Generic;

namespace AutomataLibrary
{
    public class AbstractFiniteAutomaton
    {
		protected SortedSet<char> MAlphabet;
		protected SortedSet<int> MStates;
		protected int MInitialState;
		protected SortedSet<int> MFinalStates;

		public AbstractFiniteAutomaton(SortedSet<char> alphabet, SortedSet<int> states, int initialState, SortedSet<int> finalStates)
		{
			MAlphabet = alphabet;
			MStates = states;
			MInitialState = initialState;
			MFinalStates = finalStates;
		}
	}
}
