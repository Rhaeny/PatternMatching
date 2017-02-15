using System;
using System.Diagnostics;
using AutomataGeneratorLibrary;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            HammingDistanceAutomataGenerator hdag = new HammingDistanceAutomataGenerator("abc", 2);
            LevenshteinDistanceAutomataGenerator ldag = new LevenshteinDistanceAutomataGenerator("abc", 2);
            
            var getStartProcessQuery = new GetStartProcessQuery();
            var getProcessStartInfoQuery = new GetProcessStartInfoQuery();
            var registerLayoutPluginCommand = new RegisterLayoutPluginCommand(getProcessStartInfoQuery, getStartProcessQuery);

            var wrapper = new GraphGeneration(
                getStartProcessQuery,
                getProcessStartInfoQuery,
                registerLayoutPluginCommand)
            { GraphvizPath = @"..\..\..\packages\Graphviz.2.38.0.2\" };
            byte[] output = wrapper.GenerateGraph(hdag.GetAutomata().GetGraphString(), Enums.GraphReturnType.Png);
            System.IO.File.WriteAllBytes("Graph.png", output);
            Console.WriteLine(hdag.GetAutomata().GetGraphString());

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds/10:00}";
            Console.WriteLine("RunTime " + elapsedTime);
            Console.ReadLine();
        }
    }
}
