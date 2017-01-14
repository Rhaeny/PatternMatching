using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataLibrary
{
	public class NFA : AbstractFiniteAutomaton
	{
		protected SortedList<int, SortedList<char, SortedSet<int>>> MDelta = new SortedList<int, SortedList<char, SortedSet<int>>>();
		protected SortedList<int, SortedSet<int>> MEpsilonTrans = new SortedList<int, SortedSet<int>>();

		public NFA(SortedSet<char> alphabet, SortedSet<int> states, 
			List<Tuple<int, string, int>> deltaItems, List<Tuple<int, int>> epsilonItems,
			int initialState, SortedSet<int> finalStates)
			: base (alphabet, states, initialState, finalStates)
		{
			foreach (Tuple<int, string, int> item in deltaItems)
			{
				SortedList<char, SortedSet<int>> destStates;
				if (MDelta.TryGetValue(item.Item1, out destStates))
				{
					foreach (char ch in item.Item2)
					{
						SortedSet<int> s;
						if (destStates.TryGetValue(ch, out s))
						{
							s.Add(item.Item3);
						}
						else
						{
							s = new SortedSet<int>() { item.Item3 };
							destStates.Add(ch, s);
						}

					}
				}
				else
				{
					destStates = new SortedList<char, SortedSet<int>>();
					foreach (char ch in item.Item2)
					{
						destStates.Add(ch, new SortedSet<int>() { item.Item3 });
					}
					MDelta.Add(item.Item1, destStates);
				}
			}
			foreach(Tuple<int, int> item in epsilonItems)
			{
				SortedSet<int> s;
				if (MEpsilonTrans.TryGetValue(item.Item1, out s))
				{
					s.Add(item.Item2);
				}
				else
				{
					s = new SortedSet<int>() { item.Item2};
					MEpsilonTrans.Add(item.Item1, s);
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
                SortedList <char, SortedSet <int>> transitions;
	            if (MDelta.TryGetValue(state, out transitions))
	            {
	                foreach (var transition in transitions)
	                {
	                    foreach (var item2 in transition.Value)
	                    {
                            output.Append(state + " -> " + item2 + " [label=" + transition.Key + "];");
                        }
	                }
	            }

                SortedSet<int> epsTrans;
	            if (MEpsilonTrans.TryGetValue(state, out epsTrans))
	            {
	                foreach (var item2 in epsTrans)
	                {
                        output.Append(state + " -> " + item2 + " [label=E];");
	                }
	            }
	        }

            output.Append("}");
            return output.ToString();
	    }
	}
}
