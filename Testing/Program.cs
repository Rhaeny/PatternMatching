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
            AbstractFiniteAutomaton fa;

            SortedSet<char> mAlphabet;
            SortedSet<int> mStates;
            int mInitialState;
            SortedSet<int> mFinalStates;
            List<Tuple<int, string, int>> deltaItems;

            Console.WriteLine("1 - NFA\n2 - DFA");
            var x = Console.ReadLine();
            switch (x)
            {
                case "1":
                {
                    mAlphabet = new SortedSet<char> { 'a', 'b' };
                    mStates = new SortedSet<int> { 0, 1, 2, 3, 4 };
                    mInitialState = 0;
                    mFinalStates = new SortedSet<int> { 2, 4 };
                    deltaItems = new List<Tuple<int, string, int>>
                    {
                        new Tuple<int, string, int>(0, "a", 1),
                        new Tuple<int, string, int>(1, "b", 2),
                        new Tuple<int, string, int>(3, "b", 4)
                    };
                    var epsilonItems = new List<Tuple<int, int>>
                    {
                        new Tuple<int, int>(0, 0),
                        new Tuple<int, int>(0, 3),
                        new Tuple<int, int>(1, 4)
                    };
                    fa = new NFA(mAlphabet, mStates, deltaItems, epsilonItems, mInitialState, mFinalStates);
                    break;
                }
                    
                default:
                {
                    mAlphabet = new SortedSet<char> { 'a', 'b' };
                    mStates = new SortedSet<int> { 1, 2, 3, 4, 5 };
                    mInitialState = 1;
                    mFinalStates = new SortedSet<int> { 1, 4 };
                    deltaItems = new List<Tuple<int, string, int>>
                    {
                        new Tuple<int, string, int>(1, "b", 1),
                        new Tuple<int, string, int>(1, "a", 2),
                        new Tuple<int, string, int>(2, "a", 4),
                        new Tuple<int, string, int>(2, "b", 5),
                        new Tuple<int, string, int>(3, "a", 1),
                        new Tuple<int, string, int>(3, "b", 4),
                        new Tuple<int, string, int>(4, "a", 1),
                        new Tuple<int, string, int>(4, "b", 3),
                        new Tuple<int, string, int>(5, "a", 4),
                        new Tuple<int, string, int>(5, "b", 5)
                    };
                    fa = new DFA(mAlphabet, mStates, deltaItems, mInitialState, mFinalStates);
                    break;
                }
            }

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand) {GraphvizPath = @"C:\Program Files (x86)\Graphviz2.38\bin\"};
            byte[] output = wrapper.GenerateGraph(fa.GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("Graph.png", output);
        }
    }
}
