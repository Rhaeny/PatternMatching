using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace AutomataLibrary
{
    [Serializable]
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

        public override void Accepts(string input)
        {
            int x = 0;
            int currentState = MInitialState;
            for (int i = 0; i < input.Length; i++)
            {
                SortedList<char, int> destStates;
                if (MDelta.TryGetValue(currentState, out destStates))
                {
                    int state2;
                    if (destStates.TryGetValue(input[i], out state2))
                    {
                        //Console.WriteLine("(" + currentState + "," + input[i] + ")->" + state2);
                        if (MFinalStates.Contains(state2))
                        {
                            //Console.WriteLine(" Match found at position " + i + ".");
                            x++;
                        }
                        currentState = state2;
                    }
                }
            }
            Console.WriteLine("Total: " + x);
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
                foreach (var destState in delta.Value)
                {
                    foreach (var outputTrans in outputDelta.Where(outputTrans => outputTrans.Item1 == delta.Key && outputTrans.Item3 == destState.Value))
                    {
                        outputTrans.Item2.Add(destState.Key);
                    }
                }
            }
            SortedSet<char> patternAlphabet = new SortedSet<char>();
            foreach (var transition in outputDelta.Where(transition => transition.Item2.Count == 1))
            {
                patternAlphabet.Add(transition.Item2.First());
                output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=" + transition.Item2.First() + "];");
            }
            foreach (var transition in outputDelta.Where(transition => transition.Item2.Count > 1))
            {
                SortedSet<char> missingChars = new SortedSet<char>();
                foreach (var c in patternAlphabet.Where(c => !transition.Item2.Contains(c)))
                {
                    missingChars.Add(c);
                }
                output.Append(transition.Item1 + " -> " + transition.Item3 + " [label=Comp_");
                foreach (var ch in missingChars)
                {
                    output.Append(ch);
                }
                output.Append("];");
            }
            output.Append("}");
            return output.ToString();
        }
    }
}
