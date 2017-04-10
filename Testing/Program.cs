using System;
using System.Diagnostics;
using System.IO;
using AutomataGeneratorLibrary;
using AutomataLibrary;
using BitParallelismLibrary;
using GraphVizWrapper;
using GraphVizWrapper.Commands;
using GraphVizWrapper.Queries;

namespace Testing
{
    class Program
    {
        private static void Main(string[] args)
        {
            TestAccept();
        }

        public static void TestAcceptBit()
        {
            Console.WriteLine("Pattern:");
            string pattern = Console.ReadLine();
            Console.WriteLine("k:");
            int k = Convert.ToInt32(Console.ReadLine());

            HammingDistanceNFASimulator hd = new HammingDistanceNFASimulator();
            LevenshteinDistanceNFASimulator ld = new LevenshteinDistanceNFASimulator();

            DisplayTimerProperties();

            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            const long numIterations = 21;

            string fileName = @"D:\cantrbry\alice29.txt";
            string fileText = File.ReadAllText(fileName);

            string[] operationNames =
            {
                "Operation: hg.AcceptInput(" + pattern + ", " + k + ", " + fileName + ")",
                "Operation: ld.AcceptInputComplementVersion(" + pattern + ", " + k + ", " + fileName + ") for pattern "
            };

            for (int operation = 0; operation <= 1; operation++)
            {
                long numTicks = 0;
                long maxTicks = 0;
                long minTicks = long.MaxValue;
                int indexFastest = -1;
                int indexSlowest = -1;

                Stopwatch time10KOperations = Stopwatch.StartNew();

                for (int i = 0; i <= numIterations; i++)
                {
                    long ticksThisTime;
                    Stopwatch timePerParse;

                    switch (operation)
                    {
                        case 0:
                            timePerParse = Stopwatch.StartNew();

                            Console.WriteLine(hd.AcceptInput(pattern, k, fileText));

                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                        default:
                            timePerParse = Stopwatch.StartNew();

                            Console.WriteLine(ld.AcceptInput("adbbca", 3, "adcabcaabadbbca"));

                            timePerParse.Stop();
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                    }
                    if (i == 0)
                    {
                        time10KOperations.Reset();
                        time10KOperations.Start();
                    }
                    else
                    {
                        if (maxTicks < ticksThisTime)
                        {
                            indexSlowest = i;
                            maxTicks = ticksThisTime;
                        }
                        if (minTicks > ticksThisTime)
                        {
                            indexFastest = i;
                            minTicks = ticksThisTime;
                        }
                        numTicks += ticksThisTime;
                    }
                    //Console.WriteLine(ticksThisTime * nanosecPerTick + "ns");
                }
                time10KOperations.Stop();
                var milliSec = time10KOperations.ElapsedMilliseconds;

                Console.WriteLine();
                Console.WriteLine("{0} Summary:", operationNames[operation]);
                Console.WriteLine("  Slowest time:  #{0}/{1} = {2} nanoseconds", indexSlowest, numIterations, maxTicks * nanosecPerTick);
                Console.WriteLine("  Fastest time:  #{0}/{1} = {2} nanoseconds", indexFastest, numIterations, minTicks * nanosecPerTick);
                Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds", numTicks / numIterations, (numTicks * nanosecPerTick) / numIterations);
                Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds", numIterations, milliSec);
            }
        }

        public static void TestAccept()
        {
            NFA nfa;

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
                    HammingDistanceAutomataGenerator hammingGen = new HammingDistanceAutomataGenerator();
                    nfa = hammingGen.GenerateSigmaVersionNFA(pattern, k);
                    break;
                default:
                    LevenshteinDistanceAutomataGenerator levenshteinGen = new LevenshteinDistanceAutomataGenerator();
                    nfa = levenshteinGen.GenerateSigmaVersionNFA(pattern, k);
                    break;
            }

            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("NFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();

            /*byte[] output = wrapper.GenerateGraph(nfa.GetGraphvizString(), Enums.GraphReturnType.Png);
            File.WriteAllBytes("GraphNFA.png", output);

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("PNG image of NFA generated. Runtime: " + elapsedTime);
            stopWatch.Restart();*/

            DFA dfa = nfa.TransformToDFA();

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("DFA generated. Runtime: " + elapsedTime);

            /*output = wrapper.GenerateGraph(dfa.GetGraphvizString(), Enums.GraphReturnType.Png);
            File.WriteAllBytes("GraphDFA.png", output);

            ts = stopWatch.Elapsed;
            elapsedTime = $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
            Console.WriteLine("PNG image of DFA generated. Runtime: " + elapsedTime);*/

            string fileName = @"D:\cantrbry\alice29.txt";

            //string fileText = File.ReadAllText(fileName);
            //Console.WriteLine("Number of characters: " + fileText.Length);

            DisplayTimerProperties();

            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            const long numIterations = 2;

            string[] operationNames =
            {
                "Operation: nfa.Accepts(" + fileName + ") for pattern " + pattern,
                "Operation: dfa.Accepts(" + fileName + ") for pattern " + pattern
            };

            for (int operation = 0; operation <= 1; operation++)
            {
                long numTicks = 0;
                long maxTicks = 0;
                long minTicks = long.MaxValue;
                int indexFastest = -1;
                int indexSlowest = -1;

                Stopwatch time10KOperations = Stopwatch.StartNew();

                for (int i = 0; i <= numIterations; i++)
                {
                    long ticksThisTime;
                    Stopwatch timePerParse;

                    switch (operation)
                    {
                        case 0:
                            timePerParse = Stopwatch.StartNew();

                            Console.WriteLine("Total: " + nfa.AcceptFile(fileName));

                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                        default:
                            timePerParse = Stopwatch.StartNew();

                            Console.WriteLine("Total: " + dfa.AcceptFile(fileName));

                            timePerParse.Stop();
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                    }
                    if (i == 0)
                    {
                        time10KOperations.Reset();
                        time10KOperations.Start();
                    }
                    else
                    {
                        if (maxTicks < ticksThisTime)
                        {
                            indexSlowest = i;
                            maxTicks = ticksThisTime;
                        }
                        if (minTicks > ticksThisTime)
                        {
                            indexFastest = i;
                            minTicks = ticksThisTime;
                        }
                        numTicks += ticksThisTime;
                    }
                    Console.WriteLine(ticksThisTime * nanosecPerTick + "ns");
                }
                time10KOperations.Stop();
                var milliSec = time10KOperations.ElapsedMilliseconds;

                Console.WriteLine();
                Console.WriteLine("{0} Summary:", operationNames[operation]);
                Console.WriteLine("  Slowest time:  #{0}/{1} = {2} nanoseconds", indexSlowest, numIterations, maxTicks * nanosecPerTick);
                Console.WriteLine("  Fastest time:  #{0}/{1} = {2} nanoseconds", indexFastest, numIterations, minTicks * nanosecPerTick);
                Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds", numTicks / numIterations, (numTicks * nanosecPerTick) / numIterations);
                Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds", numIterations, milliSec);
            }
        }

        public static void TestTransform()
        {
            HammingDistanceAutomataGenerator hammingGen = new HammingDistanceAutomataGenerator();
            LevenshteinDistanceAutomataGenerator levenshteinGen = new LevenshteinDistanceAutomataGenerator();

            NFA nfa;

            Console.WriteLine("Pattern:");
            string pattern = Console.ReadLine();
            Console.WriteLine("k:");
            int k = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("1 - Hamming Distance\n2 - Levenshtein Distance");
            var x = Console.ReadLine();

            DisplayTimerProperties();

            long nanosecPerTick = (1000L * 1000L * 1000L) / Stopwatch.Frequency;
            const long numIterations = 21;
            string[] operationNames =
            {
                "Operation: generating nfa for pattern " + pattern + " with mistake " + k,
                "Operation: nfa.TransformToDFA() for pattern " + pattern + " with mistake " + k
            };

            switch (x)
            {
                case "1":
                    nfa = hammingGen.GenerateComplementVersionNFA(pattern, k);
                    break;
                default:
                    nfa = levenshteinGen.GenerateComplementVersionNFA(pattern, k);
                    break;
            }
            
            for (int operation = 0; operation <= 1; operation++)
            {
                long numTicks = 0;
                long maxTicks = 0;
                long minTicks = long.MaxValue;
                int indexFastest = -1;
                int indexSlowest = -1;

                Stopwatch time10KOperations = Stopwatch.StartNew();

                for (int i = 0; i <= numIterations; i++)
                {
                    long ticksThisTime;
                    Stopwatch timePerParse;

                    switch (operation)
                    {
                        case 0:
                            timePerParse = Stopwatch.StartNew();
                            switch (x)
                            {
                                case "1":
                                    nfa = hammingGen.GenerateComplementVersionNFA(pattern, k);
                                    break;
                                default:
                                    nfa = levenshteinGen.GenerateComplementVersionNFA(pattern, k);
                                    break;
                            }
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                        default:
                            timePerParse = Stopwatch.StartNew();

                            nfa.TransformToDFA();

                            timePerParse.Stop();
                            ticksThisTime = timePerParse.ElapsedTicks;
                            break;
                    }
                    if (i == 0)
                    {
                        time10KOperations.Reset();
                        time10KOperations.Start();
                    }
                    else
                    {
                        if (maxTicks < ticksThisTime)
                        {
                            indexSlowest = i;
                            maxTicks = ticksThisTime;
                        }
                        if (minTicks > ticksThisTime)
                        {
                            indexFastest = i;
                            minTicks = ticksThisTime;
                        }
                        numTicks += ticksThisTime;
                    }
                }
                time10KOperations.Stop();
                var milliSec = time10KOperations.ElapsedMilliseconds;

                Console.WriteLine();
                Console.WriteLine("{0} Summary:", operationNames[operation]);
                Console.WriteLine("  Slowest time:  #{0}/{1} = {2} nanoseconds", indexSlowest, numIterations, maxTicks * nanosecPerTick);
                Console.WriteLine("  Fastest time:  #{0}/{1} = {2} nanoseconds", indexFastest, numIterations, minTicks * nanosecPerTick);
                Console.WriteLine("  Average time:  {0} ticks = {1} nanoseconds", numTicks / numIterations, (numTicks * nanosecPerTick) / numIterations);
                Console.WriteLine("  Total time looping through {0} operations: {1} milliseconds", numIterations, milliSec);
            }
        }

        public static void DisplayTimerProperties()
        {
            Console.WriteLine(Stopwatch.IsHighResolution
                ? "Operations timed using the system's high-resolution performance counter."
                : "Operations timed using the DateTime class.");
            long frequency = Stopwatch.Frequency;
            Console.WriteLine("  Timer frequency in ticks per second = {0}", frequency);
            long nanosecPerTick = (1000L * 1000L * 1000L) / frequency;
            Console.WriteLine("  Timer is accurate within {0} nanoseconds", nanosecPerTick);
        }
    }
}
