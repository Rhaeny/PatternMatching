using System;
using System.Collections.Generic;

namespace AutomataLibrary
{
    /// <summary>
    /// Abstract class of finite automaton.
    /// </summary>
    [Serializable]
    public abstract class AbstractFiniteAutomaton
    {
        /// <summary>
        /// Set of all symbols contained in automaton.
        /// </summary>
		protected SortedSet<char> MAlphabet;

        /// <summary>
        /// Set of states of automaton.
        /// </summary>
		protected SortedSet<int> MStates;

        /// <summary>
        /// Initial state of automaton.
        /// </summary>
		protected int MInitialState;

        /// <summary>
        /// Set of final states of automaton.
        /// </summary>
		protected SortedSet<int> MFinalStates;

        /// <summary>
        /// Initializes a new instance of <see cref="AbstractFiniteAutomaton"/>.
        /// </summary>
        /// <param name="alphabet">Set of all symbols contained in automaton.</param>
        /// <param name="states">Set of states of automaton.</param>
        /// <param name="initialState">Initial state of automaton.</param>
        /// <param name="finalStates">Set of final states of automaton.</param>
        protected AbstractFiniteAutomaton(SortedSet<char> alphabet, SortedSet<int> states, int initialState, SortedSet<int> finalStates)
        {
            MAlphabet = alphabet;
            MStates = states;
            MInitialState = initialState;
            MFinalStates = finalStates;
        }

        /// <summary>
        /// Accepts input text and finds number of matches.
        /// </summary>
        /// <param name="input">Input text for automaton.</param>
        /// <returns>Number of matches.</returns>
        public abstract int AcceptInput(string input);

        /// <summary>
        /// Accepts file and finds number of matches.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        /// <returns>Number of matches.</returns>
        public abstract int AcceptFile(string filePath);

        /// <summary>
        /// Gets dot string of automaton that can be passed to Graphviz graph generator.
        /// </summary>
        /// <returns>Dot string of automaton.</returns>
        public abstract string GetGraphvizString();
    }
}
