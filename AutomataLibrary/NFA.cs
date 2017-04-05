using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MyCollections;

namespace AutomataLibrary
{
    /// <summary>
    /// Nondeterministic finite automaton class.
    /// </summary>
    [Serializable]
	public class NFA : AbstractFiniteAutomaton
	{
        /// <summary>
        /// Sorted list of delta transitions.
        /// </summary>
		protected SortedList<int, SortedList<char, SortedSet<int>>> MDelta = new SortedList<int, SortedList<char, SortedSet<int>>>();

        /// <summary>
        /// Sorted list of epsilon transition.
        /// </summary>
		protected SortedList<int, SortedSet<int>> MEpsilonTrans = new SortedList<int, SortedSet<int>>();

        /// <summary>
        /// Initializes a new instance of <see cref="NFA"/>.
        /// </summary>
        /// <param name="alphabet">Set of all symbols contained in automaton.</param>
        /// <param name="states">Set of states of automaton.</param>
        /// <param name="deltaItems">Collection of delta transitions of automaton.</param>
        /// <param name="epsilonItems">Collection of epsilon transitions of automaton.</param>
        /// <param name="initialState">Initial state of automaton.</param>
        /// <param name="finalStates">Set of final states of automaton.</param>
		public NFA(SortedSet<char> alphabet, SortedSet<int> states, IEnumerable<Tuple<int, string, int>> deltaItems, IEnumerable<Tuple<int, int>> epsilonItems,
			int initialState, SortedSet<int> finalStates) : base (alphabet, states, initialState, finalStates)
		{
            int transCount = 0;
            foreach (var item in deltaItems)
			{
				SortedList<char, SortedSet<int>> destStates;
				if (MDelta.TryGetValue(item.Item1, out destStates))
				{
					foreach (var ch in item.Item2)
					{
                        SortedSet<int> states2;
						if (destStates.TryGetValue(ch, out states2))
						{
							states2.Add(item.Item3);
                            transCount++;
                        }
						else
						{
							states2 = new SortedSet<int> { item.Item3 };
							destStates.Add(ch, states2);
                            transCount++;
                        }
					}
				}
				else
				{
					destStates = new SortedList<char, SortedSet<int>>();
					foreach (var ch in item.Item2)
					{
						destStates.Add(ch, new SortedSet<int> { item.Item3 });
                        transCount++;
                    }
					MDelta.Add(item.Item1, destStates);
				}
			}
			foreach(var item in epsilonItems)
			{
				SortedSet<int> states2;
				if (MEpsilonTrans.TryGetValue(item.Item1, out states2))
				{
					states2.Add(item.Item2);
				}
				else
				{
					states2 = new SortedSet<int>() { item.Item2};
					MEpsilonTrans.Add(item.Item1, states2);
				}
			}
            Console.WriteLine("Number of NFA states: " + MStates.Count);
            Console.WriteLine("Number of NFA transitions: " + transCount);
        }

        /// <summary>
        /// Tranforms this <see cref="NFA"/> to equivalent <see cref="DFA"/> by using the standard subset construction.
        /// </summary>
        /// <returns><see cref="DFA"/> equivalent to this <see cref="NFA"/>.</returns>
        public DFA TransformToDFA()
        {
            SortedList<int, SortedList<char, SortedSet<int>>> noEpsDelta = RemoveEpsilonTransitions();

            SortedSet<int> finalStates = new SortedSet<int>();
            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>();

            HashSet<EquatableSortedSet<int>> notProcessedStateSets = new HashSet<EquatableSortedSet<int>>
            {
                new EquatableSortedSet<int> { MInitialState }
            };
            Dictionary<EquatableSortedSet<int>, int> stateSets = new Dictionary<EquatableSortedSet<int>, int>
            {
                { new EquatableSortedSet<int> { MInitialState }, 0 }
            };

            while (notProcessedStateSets.Count > 0)
            {
                EquatableSortedSet<int> currentStateSet = notProcessedStateSets.First();
                int state1;
                stateSets.TryGetValue(currentStateSet, out state1);
                foreach (var a in MAlphabet)
                {
                    EquatableSortedSet<int> nextStates = new EquatableSortedSet<int>();
                    foreach (var currentState in currentStateSet)
                    {
                        SortedList<char, SortedSet<int>> destStates;
                        if (noEpsDelta.TryGetValue(currentState, out destStates))
                        {
                            SortedSet<int> states2;
                            if (destStates.TryGetValue(a, out states2))
                            {
                                nextStates.UnionWith(states2);
                            }
                        }
                    }
                    int state2;
                    if (!stateSets.TryGetValue(nextStates, out state2))
                    {
                        if (!notProcessedStateSets.Contains(nextStates))
                        {
                            notProcessedStateSets.Add(nextStates);
                            stateSets.Add(nextStates, stateSets.Count);
                            state2 = stateSets.Last().Value;
                        }
                    }
                    deltaItems.Add(new Tuple<int, string, int>(state1, a.ToString(), state2));
                }
                if (currentStateSet.Any(state => MFinalStates.Contains(state)))
                {
                    finalStates.Add(state1);
                }
                notProcessedStateSets.Remove(currentStateSet);
            }
            return new DFA(MAlphabet, new SortedSet<int>(stateSets.Values), deltaItems, MInitialState, finalStates);
        }

        /// <summary>
        /// Adds additional transitions by removing epsilon transitions from <see cref="MEpsilonTrans"/>.
        /// </summary>
        /// <returns>Sorted list of altered delta transitions.</returns>
        protected SortedList<int, SortedList<char, SortedSet<int>>> RemoveEpsilonTransitions()
	    {
            SortedList<int, SortedList<char, SortedSet<int>>> newDelta = new SortedList<int, SortedList<char, SortedSet<int>>>();
            foreach (var state in MStates)
	        {
                SortedList<char, SortedSet<int>> destStates = new SortedList<char, SortedSet<int>>();
                SortedSet<int> epsilonClosureSet = new SortedSet<int>();
                GetEpsilonClosure(state, epsilonClosureSet);
	            foreach (var a in MAlphabet)
	            {
                    SortedSet<int> allStates2 = new SortedSet<int>();
                    foreach (var epsilonClosureState in epsilonClosureSet)
                    {
                        SortedSet<int> states2;
                        if (TryGetStates2(epsilonClosureState, a, out states2))
                        {
                            allStates2.UnionWith(states2);
                        }
                    }
                    destStates.Add(a, allStates2);
                }
                newDelta.Add(state, destStates);
            }
	        return newDelta;
	    }

        /// <summary>
        /// Gets all states, that can be moved over by symbol <see cref="a"/> from <see cref="currentState"/>.
        /// </summary>
        /// <param name="currentState">The current state whose states to get.</param>
        /// <param name="a">The symbol to move over.</param>
        /// <param name="states2">When this method returns, the set of states that can be accessed via symbol <see cref="a"/> from <see cref="currentState"/>,
        /// if the current state is found; otherwise, the default value for the type of <see cref="states2"/> parameter. This parameter is passed uninitialized.</param>
        /// <returns>True, if at least one state is found; False, otherwise.</returns>
        protected bool TryGetStates2(int currentState, char a, out SortedSet<int> states2)
	    {
            SortedList<char, SortedSet<int>> destStates;
            if (MDelta.TryGetValue(currentState, out destStates))
            {
                if (destStates.TryGetValue(a, out states2))
                {
                    return true;
                }
            }
            states2 = null;
	        return false;
	    }

        /// <summary>
        /// Recursively finds epsilon closure of <see cref="currentState"/>.
        /// </summary>
        /// <param name="currentState"> Current state whose states to get.</param>
        /// <param name="epsilonClosureSet">Set of states, that can be moved over from original state by epsilon transitions. Serves also as return value.</param>
        protected void GetEpsilonClosure(int currentState, SortedSet<int> epsilonClosureSet)
	    {
            epsilonClosureSet.Add(currentState);
            foreach (var epsTrans in MEpsilonTrans.Where(epsTrans => epsTrans.Key == currentState))
	        {
                foreach (var state2 in epsTrans.Value)
	            {
	                GetEpsilonClosure(state2, epsilonClosureSet);
	            }
	        }
	    }

        /// <summary>
        /// Accepts input text and finds count of matches by using basic method of simulation of run of <see cref="NFA"/>.
        /// </summary>
        /// <param name="input">Input text for automaton.</param>
        /// <returns>Count of matches.</returns>
        public override int AcceptInput(string input)
        {
            int matches = 0;
            SortedList<int, SortedList<char, SortedSet<int>>> noEpsDelta = RemoveEpsilonTransitions();
            SortedSet<int> notProcessedStateSet = new SortedSet<int> { MInitialState};
            for (int i = 0; i < input.Length && notProcessedStateSet.Count > 0; i++)
            {
                bool isFound = false;
                SortedSet<int> nextStateSet = new SortedSet<int>();
                foreach (var notProcessedState in notProcessedStateSet)
                {
                    SortedList<char, SortedSet<int>> destStates;
                    if (noEpsDelta.TryGetValue(notProcessedState, out destStates))
                    {
                        SortedSet<int> states2;
                        if (destStates.TryGetValue(input[i], out states2))
                        {
                            foreach (var state2 in states2)
                            {
                                if (MFinalStates.Contains(state2) && !isFound)
                                {
                                    matches++;
                                    isFound = true;
                                }
                                nextStateSet.Add(state2);
                            }
                        }
                    }
                }
                notProcessedStateSet = nextStateSet;
            }
            return matches;
        }

        /// <summary>
        /// Accepts file and finds count of matches by using basic method of simulation of run of <see cref="NFA"/>.
        /// </summary>
        /// <param name="filePath">Path of the file.</param>
        /// <returns>Count of matches.</returns>
        public override int AcceptFile(string filePath)
        {
            int matches = 0;
            using (StreamReader r = new StreamReader(filePath))
            {
                SortedList<int, SortedList<char, SortedSet<int>>> noEpsDelta = RemoveEpsilonTransitions();
                SortedSet<int> notProcessedStateSet = new SortedSet<int> { MInitialState };
                char[] buffer = new char[1024];
                int read;
                while ((read = r.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        bool isFound = false;
                        SortedSet<int> nextStateSet = new SortedSet<int>();
                        foreach (var notProcessedState in notProcessedStateSet)
                        {
                            SortedList<char, SortedSet<int>> destStates;
                            if (noEpsDelta.TryGetValue(notProcessedState, out destStates))
                            {
                                SortedSet<int> states2;
                                if (destStates.TryGetValue(buffer[i], out states2))
                                {
                                    foreach (var state2 in states2)
                                    {
                                        if (MFinalStates.Contains(state2) && !isFound)
                                        {
                                            matches++;
                                            isFound = true;
                                        }
                                        nextStateSet.Add(state2);
                                    }
                                }
                            }
                        }
                        if (nextStateSet.Count > 0)
                        {
                            notProcessedStateSet = nextStateSet;
                        }
                    }
                }
            }
            return matches;
        }

        /// <summary>
        /// Prints sorted set to console for easy debugging.
        /// </summary>
        /// <param name="sortedSet">Sorted set to print.</param>
        protected void PrintSortedSet(SortedSet<int> sortedSet)
	    {
            Console.Write("\n{ ");
            foreach (var closureState in sortedSet)
            {
                Console.Write(closureState + " ");
            }
            Console.Write("}\n");
        }

        /// <summary>
        /// Gets dot string of <see cref="NFA"/> that can be passed to Graphviz graph generator.
        /// </summary>
        /// <returns>Dot string of <see cref="NFA"/>.</returns>
	    public override string GetGraphvizString()
	    {
	        StringBuilder output = new StringBuilder();
	        List<Tuple<int, SortedSet<char>, int>> outputDelta = (
                from state1 in MStates
                from state2 in MStates
                select new Tuple<int, SortedSet<char>, int>(state1, new SortedSet<char>(), state2)).ToList();
	        output.Append("digraph{Start [shape=plaintext];Start -> " + MInitialState + ";");
	        foreach (var finalState in MFinalStates)
	        {
	            output.Append(finalState + "[shape=doublecircle];");
	        }
            foreach (var delta in MDelta)
            {
                foreach (var destStates in delta.Value)
                {
                    foreach (var state2 in destStates.Value)
                    {
                        foreach (var outputTrans in outputDelta.Where(outputTrans => outputTrans.Item1 == delta.Key && outputTrans.Item3 == state2))
                        {
                            outputTrans.Item2.Add(destStates.Key);
                        }
                    }
                }
            }
	        foreach (var transition in outputDelta)
	        {
	            if (transition.Item2.SetEquals(MAlphabet))
	            {
                    output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=Sig];");
                }
	            else
	            {
                    foreach (var symbol in transition.Item2)
                    {
                        output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=" + symbol + "];");
                    }
                }
	        }
            foreach (var epsTrans in MEpsilonTrans)
            {
                foreach (var value in epsTrans.Value)
                {
                    output.Append(epsTrans.Key + " -> " + value + " [label=Eps];");
                }
            }
            return output.Append("}").ToString();
	    }
	}
}
