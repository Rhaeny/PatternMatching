namespace DynamicProgramming
{
    public class HammingDistance
    {
        public int AcceptInput(string pattern, int k, string input)
        {
            int matches = 0;

            double[,] d = new double[pattern.Length + 1, input.Length + 1];

            for (int j = 1; j <= pattern.Length; j++)
            {
                d[j, 0] = k + 1;
            }
            for (int i = 0; i <= input.Length; i++)
            {
                d[0, i] = 0;
            }
            for (int i = 1; i <= input.Length; i++)
            {
                for (int j = 1; j <= pattern.Length; j++)
                {
                    if (input[i-1] == pattern[j-1])
                    {
                        d[j, i] = d[j - 1, i - 1];
                    }
                    else
                    {
                        d[j, i] = d[j - 1, i - 1] + 1;
                    }
                }
            }
            for (int i = 1; i <= input.Length; i++)
            {
                if (d[pattern.Length, i] <= k)
                {
                    matches++;
                }
            }
            return matches;
        }
    }
}
