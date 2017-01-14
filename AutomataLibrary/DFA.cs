using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataLibrary
{
	public class DFA : AbstractFiniteAutomaton
	{
		protected SortedList<int, SortedList<char, int>> MDelta = new SortedList<int, SortedList<char, int>>();

		public DFA(SortedSet<char> alphabet, SortedSet<int> states, List<Tuple<int, string, int>> deltaItems, int initialState, SortedSet<int> finalStates)
			: base(alphabet, states, initialState, finalStates)
		{
			foreach(Tuple<int, string, int> item in deltaItems)
			{
				SortedList<char, int> destStates;
				if (MDelta.TryGetValue(item.Item1, out destStates))
				{
					foreach (char ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
					}
				}
				else
				{
					destStates = new SortedList<char, int>();
					foreach(char ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
					}
					MDelta.Add(item.Item1, destStates);
				}
			}
		}

        public override string GetGraphString()
        {
            StringBuilder output = new StringBuilder();
            output.Append("digraph{");

            foreach (var finalState in MFinalStates)
            {
                output.Append(finalState + "[shape=doublecircle];");
            }

            output.Append("Start [shape=plaintext];Start -> " + MInitialState + ";");

            foreach (var state in MStates)
            {
                SortedList<char, int> transitions;
                if (MDelta.TryGetValue(state, out transitions))
                {
                    foreach (var transition in transitions)
                    {
                        output.Append(state + " -> " + transition.Value + " [label=" + transition.Key + "];");
                    }
                }
            }

            output.Append("}");
            return output.ToString();
        }
    }
}
