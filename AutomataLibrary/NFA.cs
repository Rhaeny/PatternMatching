using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AutomataLibrary
{
    [Serializable]
	public class NFA : AbstractFiniteAutomaton
	{
		protected SortedList<int, SortedList<char, SortedSet<int>>> MDelta = new SortedList<int, SortedList<char, SortedSet<int>>>();
		protected SortedList<int, SortedSet<int>> MEpsilonTrans = new SortedList<int, SortedSet<int>>();

		public NFA(SortedSet<char> alphabet, SortedSet<int> states, 
			IEnumerable<Tuple<int, string, int>> deltaItems, IEnumerable<Tuple<int, int>> epsilonItems,
			int initialState, SortedSet<int> finalStates)
			: base (alphabet, states, initialState, finalStates)
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
							states2 = new SortedSet<int>() { item.Item3 };
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
						destStates.Add(ch, new SortedSet<int>() { item.Item3 });
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

        public DFA TransformToDFA()
        {
            SortedList<int, SortedList<char, SortedSet<int>>> noEpsDelta = DeleteEpsilonTransitions();

            SortedSet<int> states = new SortedSet<int> { MInitialState };
            SortedSet<int> finalStates = new SortedSet<int>();
            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>();

            List<SortedSet<int>> stateSets = new List<SortedSet<int>> { new SortedSet<int> { MInitialState } };
            List<SortedSet<int>> notProcessedStateSets = new List<SortedSet<int>> { new SortedSet<int> { MInitialState } };

            if (MFinalStates.Contains(MInitialState))
            {
                finalStates.Add(stateSets.Count - 1);
            }

            while (notProcessedStateSets.Count > 0)
            {
                SortedSet<int> currentStateSet = notProcessedStateSets.First();
                int state1 = stateSets.FindIndex(state => state.SetEquals(currentStateSet));
                foreach (var a in MAlphabet)
                {
                    SortedSet<int> nextStates = new SortedSet<int>();
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
                    if (!notProcessedStateSets.Any(stateSet => stateSet.SetEquals(nextStates))
                        && !stateSets.Any(stateSet => stateSet.SetEquals(nextStates)))
                    {
                        notProcessedStateSets.Add(nextStates);
                        stateSets.Add(nextStates);
                        states.Add(stateSets.Count - 1);
                        if (stateSets.Last().Any(state => MFinalStates.Contains(state)))
                        {
                            finalStates.Add(stateSets.Count - 1);
                        }
                    }
                    deltaItems.Add(new Tuple<int, string, int>(state1, a.ToString(), stateSets.FindIndex(state => state.SetEquals(nextStates))));
                }
                notProcessedStateSets.Remove(currentStateSet);
            }
            return new DFA(MAlphabet, states, deltaItems, MInitialState, finalStates);
        }

        protected SortedList<int, SortedList<char, SortedSet<int>>> DeleteEpsilonTransitions()
	    {
            SortedList<int, SortedList<char, SortedSet<int>>> newDelta = new SortedList<int, SortedList<char, SortedSet<int>>>();
            foreach (var state in MStates)
	        {
                SortedList<char, SortedSet<int>> destStates = new SortedList<char, SortedSet<int>>();
                SortedSet<int> epsilonClosureSet = new SortedSet<int>();
                GetEpsilonClosure(epsilonClosureSet, state);
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

        protected bool TryGetStates2(int state1, char ch, out SortedSet<int> states2)
	    {
            SortedList<char, SortedSet<int>> destStates;
            if (MDelta.TryGetValue(state1, out destStates))
            {
                if (destStates.TryGetValue(ch, out states2))
                {
                    return true;
                }
            }
            states2 = null;
	        return false;
	    }

        protected void GetEpsilonClosure(SortedSet<int> epsilonClosureSet, int state)
	    {
            epsilonClosureSet.Add(state);
            foreach (var epsTrans in MEpsilonTrans.Where(epsTrans => epsTrans.Key == state))
	        {
                foreach (var value in epsTrans.Value)
	            {
	                GetEpsilonClosure(epsilonClosureSet, value);
	            }
	        }
	    }

        public override void Accepts(string input)
        {
            int x = 0;
            SortedList<int, SortedList<char, SortedSet<int>>> noEpsDelta = DeleteEpsilonTransitions();
            SortedSet<int> notProcessedStateSet = new SortedSet<int> {MInitialState};
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
                                    x++;
                                    isFound = true;
                                }
                                nextStateSet.Add(state2);
                            }
                        }
                    }
                }
                notProcessedStateSet = nextStateSet;
            }
            Console.WriteLine("Total: " + x);
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
            output.Append("}");
            return output.ToString();
	    }
	}
}
