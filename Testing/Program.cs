using System;
using System.Diagnostics;
using AutomataGeneratorLibrary;
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
            AbstractAutomataGenerator aag;

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand)
            { GraphvizPath = @"..\..\..\packages\Graphviz.2.38.0.2\" };

            Console.WriteLine("Pattern:");
            string pattern = Console.ReadLine();
            Console.WriteLine("k:");
            int k = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("1 - Hamming Distance\n2 - Levenshtein Distance");
            var x = Console.ReadLine();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            switch (x)
            {
                case "1":
                        aag = new HammingDistanceAutomataGenerator(pattern, k);
                        break;
                default:
                        aag = new LevenshteinDistanceAutomataGenerator(pattern, k);
                        break;
            }

            NFA nfa = aag.GetAutomata();

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("NFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();

            /*byte[] output = wrapper.GenerateGraph(nfa.GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("GraphNFA.png", output);*/
            
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("PNG image of NFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();

            DFA dfa = nfa.TransformToDFA();

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("DFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();
            
            /*output = wrapper.GenerateGraph(dfa.GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("GraphDFA.png", output);*/

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("PNG image of DFA generated. Runtime: " + elapsedTime);

            Console.WriteLine("Enter input for DFA:");
            dfa.Accepts(Console.ReadLine());
        }
    }
}
