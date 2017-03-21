using System;
using System.Collections.Generic;
using System.IO;
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
		    int transCount = 0;
			foreach(var item in deltaItems)
			{
				SortedList<char, int> destStates;
				if (MDelta.TryGetValue(item.Item1, out destStates))
				{
					foreach (var ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
                        transCount++;
                    }
				}
				else
				{
					destStates = new SortedList<char, int>();
					foreach(var ch in item.Item2)
					{
						destStates.Add(ch, item.Item3);
					    transCount++;
					}
					MDelta.Add(item.Item1, destStates);
				}
			}
            Console.WriteLine("Number of DFA states: " + MStates.Count);
            Console.WriteLine("Number of DFA transitions: " + transCount);
        }

        public override void AcceptInput(string input)
        {
            int x = 0;
            int currentState = MInitialState;
            foreach (var ch in input)
            {
                SortedList<char, int> destStates;
                if (MDelta.TryGetValue(currentState, out destStates))
                {
                    int state2;
                    if (destStates.TryGetValue(ch, out state2))
                    {
                        if (MFinalStates.Contains(state2))
                        {
                            x++;
                        }
                        currentState = state2;
                    }
                }
            }
            Console.WriteLine("Total: " + x);
        }

        public override void AcceptFile(string fileName)
        {
            int x = 0;
            int currentState = MInitialState;
            using (StreamReader r = new StreamReader(fileName))
            {
                char[] buffer = new char[1024];
                int read;
                while ((read = r.ReadBlock(buffer, 0, buffer.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        SortedList<char, int> destStates;
                        if (MDelta.TryGetValue(currentState, out destStates))
                        {
                            int state2;
                            if (destStates.TryGetValue(buffer[i], out state2))
                            {
                                if (MFinalStates.Contains(state2))
                                {
                                    x++;
                                }
                                currentState = state2;
                            }
                        }
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
            return output.Append("}").ToString();
        }
    }
}
