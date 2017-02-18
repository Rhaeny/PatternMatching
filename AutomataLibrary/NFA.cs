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

	    /*public DFA TransformToDFA()
	    {
            SortedSet<int> initialStateSet = new SortedSet<int>();
            GetEpsilonClosure(initialStateSet, MInitialState);

            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>();

            List<SortedSet<int>> finalStates = new List<SortedSet<int>>();

            List<SortedSet<int>> states = new List<SortedSet<int>>();

            List<SortedSet<int>> s = new List<SortedSet<int>> {initialStateSet};
            
	        foreach (var sStates in s.ToList())
	        {
                foreach (var a in MAlphabet)
                {
                    foreach (var state in sStates)
	                {
	                    SortedList<char, SortedSet<int>> destItems1;
	                    if (MDelta.TryGetValue(state, out destItems1))
	                    {
	                        SortedSet<int> destItems2;
	                        if (destItems1.TryGetValue(a, out destItems2))
	                        {
	                            foreach (var destItem in destItems2)
	                            {
	                                SortedSet<int> closureItems = new SortedSet<int>();
	                                GetEpsilonClosure(closureItems, destItem);
	                                if (!closureItems.All(item => states.Any(stateItem => stateItem.Contains(item)))
                                        && !s.Contains(closureItems))
	                                {
	                                    s.Add(closureItems);
	                                    foreach (var x in closureItems)
	                                    {
	                                        Console.WriteLine("   " + x);
	                                    }
	                                }
                                    Console.WriteLine(state + " " + a);
	                            }
	                        }
	                    }
	                }
	            }
	            if (sStates.Any(state => MFinalStates.Contains(state)))
	            {
	                finalStates.Add(sStates);
	            }
	        }
	        foreach (var state in s)
	        {
	            foreach (var x in state)
	            {
	                Console.WriteLine(state);
	            }
                Console.WriteLine("-");
	        }
	        DFA dfa = new DFA(MAlphabet, new SortedSet<int>(), deltaItems, 0, new SortedSet<int>());
	        return dfa;
	    }*/

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

	    public override string GetGraphString()
	    {
	        StringBuilder output = new StringBuilder();
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
                        output.Append(delta.Key + " -> " + value + " [label=" + transition.Key + "];");
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
