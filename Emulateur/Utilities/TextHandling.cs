namespace Butterfly
{
    class TextHandling
    {
        public static int Parse(string input)
        {
            int sum = 0, i = 0, length = input.Length, k;

            while (i < length)
            {
                k = input[i++];
                if (k < 48 || k > 59)
                    return 0;
                sum = 10 * sum + k - 48;
            }

            return sum;
        }

        public static string GetString(double k)
        {
            return k.ToString();
        }

        public static int BooleanToInt(bool k)
        {
            return k ? 1 : 0;
        }
    }
}