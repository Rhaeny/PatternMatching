using System;
using System.Collections.Generic;
using System.Text;

namespace AutomataLibrary
{
	public class DFA : AbstractFiniteAutomaton
	{
		protected SortedList<int, SortedList<char, int>> MDelta = new SortedList<int, SortedList<char, int>>();

		public DFA(SortedSet<char> alphabet, SortedSet<int> states, IEnumerable<Tuple<int, string, int>> deltaItems, int initialState, SortedSet<int> finalStates)
			: base(alphabet, states, initialState, finalStates)
		{
			foreach(var item in deltaItems)
			{
				SortedList<char, int> destStates;
				if (MDelta.TryGetValue(item.Item1, out destStates))
				{
					foreach (var ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
					}
				}
				else
				{
					destStates = new SortedList<char, int>();
					foreach(var ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
					}
					MDelta.Add(item.Item1, destStates);
				}
			}
		}

	    public bool Accepts(string input)
	    {
            for (int i = 0; i < input.Length; i++)
            {
                List<Tuple<int, char, int>> steps = new List<Tuple<int, char, int>>();
                int currentStep = MInitialState;
                for (int j = i; j < input.Length; j++)
                {
                    SortedList<char, int> destItems;
                    if (MDelta.TryGetValue(currentStep, out destItems))
                    {
                        int item2;
                        if (destItems.TryGetValue(input[j], out item2))
                        {
                            steps.Add(new Tuple<int, char, int>(currentStep, input[j], item2));
                            if (MFinalStates.Contains(item2))
                            {
                                Console.WriteLine("Nalezena shoda na pozici " + i + ".\nKroky:");
                                foreach (var step in steps)
                                {
                                    Console.WriteLine("\t" + step.Item1 + "-" + step.Item2 + "-" + step.Item3);
                                }
                                break;
                            }
                            currentStep = item2;
                        }
                    }
                }
	        }
	        return true;
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
                output.Append(state + ";");
            }
            foreach (var delta in MDelta)
            {
                foreach (var transition in delta.Value)
                {
                    output.Append(delta.Key + " -> " + transition.Value + " [label=" + transition.Key + "];");
                }
            }
            output.Append("}");
            return output.ToString();
        }
    }
}
