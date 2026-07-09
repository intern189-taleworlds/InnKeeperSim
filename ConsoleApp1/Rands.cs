public class Rand
{
    public static readonly Random rng = new Random();
    /// <summary>
    /// Returns an array of randomly set bools.
    /// </summary>
    /// <param name="amount">Size of the bool array.</param>
    /// <returns></returns>
    public static bool[] RandomBools(int amount)
    {
        bool[] res = new bool[amount];
        for (int i = 0; i < amount; i++)
        {
            res[i] = rng.Next(2) != 0;
        }
        return res;
    }

    /// <summary>
    /// Returns an array of randomly set floats between 0.0 and 1.0.
    /// </summary>
    /// <param name="amount">Size of the float array.</param>
    /// <returns></returns>
    public static float[] RandomFloats(int amount)
    {
        float[] vals = new float[amount];
        float total = 0;
        for (int i = 0; i < amount; i++)
        {
            vals[i] = rng.Next(1, 11);
            total += vals[i];
        }

        float sum = 0;
        for (int j = 0; j < amount - 1; j++)
        {
            vals[j] = vals[j] / total;
            sum += vals[j];
        }

        vals[amount - 1] = 1.0f - sum;

        return vals;
    }
}