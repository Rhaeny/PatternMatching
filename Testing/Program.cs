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

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine("Pattern:");
            string pattern = Console.ReadLine();
            Console.WriteLine("k:");
            int k = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("1 - Hamming Distance\n2 - Levenshtein Distance");
            var x = Console.ReadLine();
            switch (x)
            {
                case "1":
                        aag = new HammingDistanceAutomataGenerator(pattern, k);
                        break;
                default:
                        aag = new LevenshteinDistanceAutomataGenerator(pattern, k);
                        break;
            }
            DFA dfa = aag.GetAutomata().TransformToDFA();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("NFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();

            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand)
            { GraphvizPath = @"..\..\..\packages\Graphviz.2.38.0.2\" };
            byte[] output = wrapper.GenerateGraph(dfa.GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("Graph.png", output);
            
            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("PNG image generated. Runtime: " + elapsedTime);

            Console.WriteLine("Zadejte vstup pro DFA:");
            dfa.Accepts(Console.ReadLine());
        }
    }
}
