using System;
using System.Collections.Generic;
using System.Linq;
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

	    public void Accepts(string input)
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
                                Console.WriteLine("Match found at position " + j + ".\nSteps:");
                                foreach (var step in steps)
                                {
                                    Console.WriteLine("\t" + step.Item1 + "-" + step.Item2 + "-" + step.Item3);
                                }
                                steps.Clear();
                                break;
                            }
                            currentStep = item2;
                        }
                    }
                }
	        }
	    }

        public void PrintSortedSet(SortedSet<char> sortedSet)
        {
            Console.WriteLine();
            Console.Write("{ ");
            foreach (var closureState in sortedSet)
            {
                Console.Write(closureState + " ");
            }
            Console.Write("}");
            Console.WriteLine();
        }

        public override string GetGraphString()
        {
            StringBuilder output = new StringBuilder();
            List<Tuple<int, SortedSet<char>, int>> outputDelta = (from state1 in MStates from state2 in MStates select new Tuple<int, SortedSet<char>, int>(state1, new SortedSet<char>(), state2)).ToList();
            output.Append("digraph{");
            foreach (var finalState in MFinalStates)
            {
                output.Append(finalState + "[shape=doublecircle];");
            }
            output.Append("Start [shape=plaintext];Start -> " + MInitialState + ";");
            foreach (var delta in MDelta)
            {
                foreach (var transition in delta.Value)
                {
                    foreach (var outputTrans in outputDelta.Where(outputTrans => outputTrans.Item1 == delta.Key && outputTrans.Item3 == transition.Value))
                    {
                        outputTrans.Item2.Add(transition.Key);
                    }
                }
            }
            SortedSet<char> patternSymbols = new SortedSet<char>();
            foreach (var transition in outputDelta.Where(transition => transition.Item2.Count == 1))
            {
                patternSymbols.Add(transition.Item2.First());
                output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=" + transition.Item2.First() + "];");
            }
            foreach (var transition in outputDelta.Where(transition => transition.Item2.Count > 1))
            {
                SortedSet<char> missingSymbols = new SortedSet<char>();
                foreach (var c in patternSymbols.Where(c => !transition.Item2.Contains(c)))
                {
                    missingSymbols.Add(c);
                }
                output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=Comp_");
                foreach (var c in missingSymbols)
                {
                    output.Append(c);
                }
                output.Append("];");
            }
            output.Append("}");
            return output.ToString();
        }
    }
}
