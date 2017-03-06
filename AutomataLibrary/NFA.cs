using System;
using System.Collections.Generic;
using System.Linq;
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

        public DFA TransformToDFA()
	    {
	        SortedSet<int> initialStateSet = new SortedSet<int>();
	        GetEpsilonClosure(initialStateSet, MInitialState);

            List<Tuple<SortedSet<int>, string, SortedSet<int>>> deltaItems = new List<Tuple<SortedSet<int>, string, SortedSet<int>>>();
            List<SortedSet<int>> finalStates = new List<SortedSet<int>>();
            List<SortedSet<int>> states = new List<SortedSet<int>>();
            List<SortedSet<int>> s = new List<SortedSet<int>> { initialStateSet };

            ProcessState(s, initialStateSet, deltaItems, finalStates);

            foreach (var set in s)
            {
                PrintSortedSet(set);
            }

            return new DFA(MAlphabet, new SortedSet<int>(), new List<Tuple<int, string, int>>(), 0, new SortedSet<int>());
	    }

	    protected void ProcessState(List<SortedSet<int>> s, SortedSet<int> currentStateSet,
            List<Tuple<SortedSet<int>, string, SortedSet<int>>> deltaItems, List<SortedSet<int>> finalStates)
	    {
	        foreach (var a in MAlphabet)
	        {
	            foreach (var state1 in currentStateSet)
	            {
	                SortedSet<int> states1EpsClosure = new SortedSet<int>();
	                GetEpsilonClosure(states1EpsClosure, state1);

	                SortedSet<int> states2Sum = new SortedSet<int>();
	                foreach (var state1EpsCLosure in states1EpsClosure)
	                {
	                    SortedSet<int> states2;
	                    if (GetStates2(state1EpsCLosure, a, out states2))
	                    {
                            states2Sum.UnionWith(states2);
	                    }
	                }

                    if (!s.Any(set => set.SetEquals(currentStateSet)))
                    {
                        s.Add(currentStateSet);
                        PrintSortedSet(states2Sum);
                        ProcessState(s, states2Sum, deltaItems, finalStates);
                    }

                    /*SortedSet<int> states2;
	                if (GetStates2(state1, a, out states2))
	                {
                        foreach (var state2 in states2)
	                    {
                            Console.WriteLine(state1 + " " + a + " " + state2);
	                        if (!s.Any(set => set.SetEquals(currentStateSet)))
	                        {
                                s.Add(currentStateSet);

                                SortedSet<int> state2Set = new SortedSet<int>();
                                GetEpsilonClosure(state2Set, state2);
                                ProcessState(s, state2Set, deltaItems, finalStates);
                            }
                        }
                        Console.WriteLine(a);
                        PrintSortedSet(states2);
                        if (!s.Any(set => set.SetEquals(currentStateSet)))
                        {
                            s.Add(currentStateSet);
                            ProcessState(s, GetEpsilonClosureOfSet(states2), deltaItems, finalStates);
                        }

                    }*/
                }
	        }
	    }

	    protected bool GetStates2(int state1, char a, out SortedSet<int> states2)
	    {
            SortedList<char, SortedSet<int>> destStates;
            if (MDelta.TryGetValue(state1, out destStates))
            {
                if (destStates.TryGetValue(a, out states2))
                {
                    return true;
                }
            }
	        states2 = null;
	        return false;
	    }

	    protected SortedSet<int> GetEpsilonClosureOfSet(SortedSet<int> set)
	    {
	        SortedSet<int> epsilonClosureOfSet = new SortedSet<int>();
	        foreach (var state in set)
	        {
	            SortedSet<int> epsilonClosure = new SortedSet<int>();
                GetEpsilonClosure(epsilonClosure, state);
                epsilonClosureOfSet.UnionWith(epsilonClosure);
	        }
            PrintSortedSet(epsilonClosureOfSet);
            return epsilonClosureOfSet;
	    }

	    protected void GetEpsilonClosure(SortedSet<int> epsilonClosure, int state)
	    {
            epsilonClosure.Add(state);
            foreach (var epsTrans in MEpsilonTrans.Where(epsTrans => epsTrans.Key == state))
	        {
                foreach (var value in epsTrans.Value)
	            {
	                GetEpsilonClosure(epsilonClosure, value);
	            }
	        }
	    }

	    public void PrintSortedSet(SortedSet<int> sortedSet)
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
                    foreach (var value in transition.Value)
                    {
                        foreach (var outputTrans in outputDelta.Where(outputTrans => outputTrans.Item1 == delta.Key && outputTrans.Item3 == value))
                        {
                            outputTrans.Item2.Add(transition.Key);
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
            output.Append("}");
            return output.ToString();
	    }
	}
}
