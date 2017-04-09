using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BitParallelismLibrary
{
    public class HammingDistanceBit
    {
        public int AcceptInput(string pattern, int k, string input)
        {
            int matches = 0;
            SortedSet<char> mAlphabet = new SortedSet<char>();
            for (char c = 'a'; c <= 'd'; c++)
            {
                mAlphabet.Add(c);
            }

            List<BitArray> r = new List<BitArray>();
            List<BitArray> d = new List<BitArray>();
            bool[] r0 = new bool[pattern.Length];
            for (int j = 0; j <= pattern.Length; j++)
            {
                r0[j] = true;
            }
            r.Add(new BitArray(r0));
            for (int i = 0; i <= input.Length; i++)
            {
                BitArray arr = new BitArray(r[i + 1]);
                for (int j = 0; j < pattern.Length - 1; j++)
                {
                    arr[j + 1] = arr[j];
                }
                arr[0] = false;

                bool[] arr2 = new bool[pattern.Length];
                for (int j = 0; j < pattern.Length; j++)
                {
                    if (pattern[j] == mAlphabet.ElementAt(x))
                    {
                        arr2[j] = false;
                    }
                    else
                    {
                        arr2[j] = true;
                    }
                }
                d.Add(new BitArray(arr2));
            }
            
            return matches;
        }

        /*
        for (int j = 0; j < pattern.Length; j++)
        {
            for (int x = 0; x < mAlphabet.Count; x++)
            {
                Console.Write(d[j, x] + "\t");
            }
            Console.WriteLine();
        }
        foreach (var arr in d)
            {
                foreach (var x in arr)
                {
                    Console.Write(x + " ");
                }
                Console.WriteLine();
            }
        */
    }
}
