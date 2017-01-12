using System;
using System.Collections.Generic;
using AutomataLibrary;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            SortedSet<char> mAlphabet = new SortedSet<char> {'a', 'b'};
            SortedSet<int> mStates = new SortedSet<int> { 0, 1, 2, 3, 4 };
            int mInitialState = 0;
            SortedSet<int> mFinalStates = new SortedSet<int> {2, 4};
            List<Tuple<int, string, int>> deltaItems = new List<Tuple<int, string, int>>
            {
                new Tuple<int, string, int>(0, "a", 1),
                new Tuple<int, string, int>(1, "b", 2),
                new Tuple<int, string, int>(3, "b", 4)
            };
            List<Tuple<int, int>> epsilonItems = new List<Tuple<int, int>>
            {
                new Tuple<int, int>(0, 0),
                new Tuple<int, int>(0, 3),
                new Tuple<int, int>(1, 4)
            };
            NFA nfa = new NFA(mAlphabet, mStates, deltaItems, epsilonItems, mInitialState, mFinalStates);

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand) {GraphvizPath = @"C:\Program Files (x86)\Graphviz2.38\bin\"};
            byte[] output = wrapper.GenerateGraph(nfa.GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("Graph.png", output);
        }
    }
}
