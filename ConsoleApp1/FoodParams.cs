using System.ComponentModel;

namespace ConsoleApp1
{
    public enum FoodParamsEnum
    {
        Meats,
        Fishes,
        Vegetables,
        Salty,
        Stew,
        Sweet,
        Hot,
        Butter,
        Milk,
        Lettuce
    }

    public enum MealCategory    //categories
    {
        Soup,
        Main,
        Dessert
    }

    internal class FoodParams   //added categories to foodparams in ln.26, 31 and 46
    {
        public MealCategory Category { get; private set; }
        public Dictionary<string, bool> parameters { get; private set; }
        public Dictionary<string, float> weightList { get; private set; }
        public static readonly int attribCount = Enum.GetNames(typeof(FoodParamsEnum)).Length;

        public FoodParams(MealCategory category, bool[] prms, float[] weights)
        {
            if (prms == null || weights == null)
            {
                throw new ArgumentNullException("Arguments can not be null");
            }

            if (prms.Length != attribCount || weights.Length != attribCount)
            {
                throw new ArgumentException("Array sizes must match attribute count");
            }

            float sum = 0;
            foreach (var f in weights) { sum += f; }
            if (sum != 1.0f) { throw new ArgumentException("Sum of weights must be 1"); }

            this.Category = category;
            this.weightList = new Dictionary<string, float>();
            this.parameters = new Dictionary<string, bool>(attribCount);
            string[] names = Enum.GetNames(typeof(FoodParamsEnum));
            for (int i = 0; i < attribCount; i++)
            {
                this.weightList.Add(names[i], weights[i]);
                this.parameters.Add(names[i], prms[i]);
            }
        }
    }
}