using System;
using System.Collections.Generic;

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
	}
}
