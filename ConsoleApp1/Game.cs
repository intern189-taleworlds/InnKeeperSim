using System.Timers;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;//

namespace ConsoleApp1
{
    public class Game
    {
        //private System.Timers.Timer timer;

        private Dictionary<string, Dictionary<string, List<string>>> encyclopedia; 
        public bool GameStarted { get; private set; }
        private Dictionary<string, bool> dish;
        private Dictionary<string, FoodParams> cookBook;
        private Dictionary<MealCategory, List<string>> currentMenu;     //changed from list to dictionary
        private Dictionary<MealCategory, string> servedThisVisit;       //new dictionary to handle multiple food types served
        private GuestHandler guestHandler;
        private Guest? currentGuest;
        private static readonly Random rng = new Random();
        public float TotalScore { get; private set; }
        private int counter;

        public Game()
        {
            GameStarted = false;
            GameReset();
        }

        private void GameReset()
        {
            if (GameStarted)
            {
                guestHandler.Reset();

            }   
            guestHandler = new GuestHandler();
            guestHandler.gameOver += GameOver;
            dish = new Dictionary<string, bool>(FoodParams.attribCount);
            currentMenu = new Dictionary<MealCategory, List<string>>();     //changed from list to dictionary
            servedThisVisit = new Dictionary<MealCategory, string>();
            cookBook = BuildCookBook();
            encyclopedia = new Dictionary<string, Dictionary<string, List<string>>>();
            counter = 0;
            if (currentGuest != null) {
                currentGuest.Reset();
                currentGuest = null;
            }

        }

        public void ExitGame()
        {
            GameReset();
            Environment.Exit(0);
        }

        private static Dictionary<string, FoodParams> BuildCookBook()
        {
            //Hard-coded Cook Book, yeni yemekler aşağıdaki formatta, FoodParamsEnum'daki opsiyonların sırası takip edilerek, tanımlanabilir.
            return new Dictionary<string, FoodParams>
            {
              // {"name", new FoodParams(MealCategory.Category, [bool, bool, ..., bool],Rand.RandomFloats(FoodParams.attribCount))}
                // do not forget to define meal category
                //soups
                {"Chicken Soup", new FoodParams(MealCategory.Soup,
                    [true, false, false, false, false, false, true, false, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                {"Yogurt Soup", new FoodParams(MealCategory.Soup,
                    [false, false, true, true, false, false, false, true, true, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                {"Tomato Soup", new FoodParams(MealCategory.Soup,
                    [false, false, true, true, false, false, false, true, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                {"Fish Soup", new FoodParams(MealCategory.Soup,
                    [false, true, false, true, true, false, true, false, false, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                {"Tripe Soup", new FoodParams(MealCategory.Soup,
                    [true, false, false, false, true, false, true, false, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                //mains
                { "Kebab", new FoodParams(MealCategory.Main,
                    [true, false, true, true, false, false, true, false, false, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Chicken Salad", new FoodParams(MealCategory.Main,
                    [true, false, true, true, false, false, false, false, false, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Boiled Artichokes", new FoodParams(MealCategory.Main,
                    [false, false, true, true, false, false, false, true, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Grilled Meatballs", new FoodParams(MealCategory.Main,
                    [true, false, false, true, false, false, true, false, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Raw Fish Wrapped with Rice", new FoodParams(MealCategory.Main,
                    [false, true, true, false, false, false, false, true, false, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Meatless Raw Meatballs", new FoodParams(MealCategory.Main,
                    [false, false, true, true, false, false, true, false, false, true],
                    Rand.RandomFloats(FoodParams.attribCount))},
                //desserts
                { "Baklava", new FoodParams(MealCategory.Dessert,
                    [false, false, false, false, false, true, false, false, false, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Sütlaç", new FoodParams(MealCategory.Dessert,
                    [false, false, false, false, false, true, false, false, true, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Ice Cream", new FoodParams(MealCategory.Dessert,
                    [false, false, false, false, false, true, false, false, true, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Künefe", new FoodParams(MealCategory.Dessert,
                    [false, false, false, false, false, true, false, false, true, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
                { "Pastries", new FoodParams(MealCategory.Dessert,
                    [false, false, false, true, false, true, false, false, true, false],
                    Rand.RandomFloats(FoodParams.attribCount))},
            };
        }

        private void CreateMenu()//create new menu by referencing full cookbook above
        {
            //changes made in order to handle multiple categories
            currentMenu = new Dictionary<MealCategory, List<string>>();
            foreach (MealCategory category in Enum.GetValues(typeof(MealCategory)))
            {
                currentMenu[category] = cookBook
                    .Where(kv => kv.Value.Category == category)
                    .Select(kv => kv.Key)
                    .OrderBy(_ => rng.Next())
                    .Take(3)
                    .ToList();
            }
        }

        private float CalculateComfort()    //removed dishName parameter. data is taken from servedThisVisit dictionary
        {
            if (currentGuest == null)
            {
                Console.WriteLine("Can't calculate comfort when there is no guest to begin with smh...");
                throw new ArgumentNullException("currentGuest is null");
            }

            float comfort = 0;
            string[] names = Enum.GetNames(typeof(FoodParamsEnum));
            foreach (var served in servedThisVisit)     //calculate for each served dish
            {
                FoodParams dishParams = cookBook[served.Value];
                for (int i = 0; i < names.Length; i++)
                {
                    bool g = currentGuest.preferences[i];
                    dishParams.parameters.TryGetValue(names[i], out bool d);
                    if (g == d)
                    {
                        dishParams.weightList.TryGetValue(names[i], out float f);
                        comfort += f;
                    }
                }
            }
            return comfort;
        }

        private void SendGuest()    //removed dishname parameter. data is taken from servedThisVisit dictionary
        {

            if (currentGuest == null)
            {
                Console.WriteLine("No one is waiting behind the counter");
                return;
            }
            try
            {
                TotalScore += CalculateComfort();
                Console.WriteLine($"The {currentGuest.Name} has left. Total score: {TotalScore}");
                currentGuest = null;
                servedThisVisit.Clear(); //clean the dictionary as it is unique for this specific guest
                counter++;
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private bool CheckGameStart()
        {
            if (!GameStarted)
            {
                Console.WriteLine("You must first Start the Game!");
            }
            return GameStarted;
        }

        private void GameOver(Object? sender, EventArgs e)
        {
            counter = 20;
            Update();
        }

        private void Update()
        {
            if (counter >= 10)
            {
                Console.WriteLine("Game Over");
                Console.WriteLine($"Total Score: {TotalScore}");
                GameReset();
                GameStarted = false;
            }
        }

        ///<summary>
        /// Called once at the initialization
        ///</summary>
        public void Start()
        {
            if (GameStarted)
            {
                Console.WriteLine("Cafeteria is already open. Get back to work! Call the next person in queue.");
                return;
            }
            //Console.WriteLine("Welcome Chef! To view the tutorial press 't'.\nTo start the game press any key.");
            //if (Console.ReadKey().Key == ConsoleKey.T)
            
            Console.WriteLine("You are a chef. " +
            "Several Guests will come to your restaurant. " +
            "Your purpose is to serve them food they will like. " +
            "\nUse 'CallGuest()' to call a guest from the queue. " +
            "\nUse 'GetCookbook()' to see what each item on the menu is, and what they contain." +
            "\nUse 'GetMenu()' to see which items are available. Available items change regularly." +
            "\nUse 'ServeDish(string dishName)' to serve a guest their food." +
            "\nUse 'GetEncyclopedia()' to see the preferences of Guests. Encyclopedia updates as you progress. " +
            "\nIf a Guest waits too long in line to be called in, they may get frustrated. " +
            "If they get frustrated, you will hear a beep. That is your cue to hurry up!" +
            "\nGood Luck!");
            
            GameStarted = true;
            Console.WriteLine("The Cafeteria is open now. Make sure your guests are comfortable with what they are eating, and don't give any garlic to a vampire.");
            System.Threading.Thread.Sleep(5000);
            guestHandler.Start();
        }

        /// <summary>
        /// Call the next guest to your counter
        /// </summary>
        public void CallGuest()
        {
            if (!CheckGameStart()) return;
            if (currentGuest != null)
            {
                Console.WriteLine("The guest behind the counter is waiting and staring at you with a confused look.");
                return;
            }

            currentGuest = guestHandler.RemoveGuest();
            CreateMenu();
            Console.WriteLine($"A {currentGuest.Name} is standing behind the counter. They are waiting for you to hand them over a plate.");
            Update();
        }

        /// <summary>
        /// Open the book to see ingredients used in a meal
        /// </summary>
        public void GetCookbook()
        {
            if (!CheckGameStart()) return;
            foreach (MealCategory category in Enum.GetValues(typeof(MealCategory)))     //extra layer of loop to display categoric differences
            {
                Console.WriteLine($"--- {category}s ---");
                foreach (var kvp in cookBook.Where(kv => kv.Value.Category == category))
                {

                    Console.WriteLine($"Dish: {kvp.Key}");
                    Console.WriteLine("Ingredients:");
                    foreach (var val in kvp.Value.parameters)
                    {
                        if (val.Value)
                        {
                            Console.WriteLine($" {val.Key}");//kvp.Value?
                        }
                    }
                    Console.WriteLine("--------------------");
                }
            }
            Console.WriteLine();
            Update();
        }

        /// <summary>
        /// Menu changes every time you serve someone. Make sure you know what's going on in the kitchen.
        /// </summary>
        public void GetMenu()
        {
            if (!CheckGameStart()) return;
            if (currentGuest == null) 
            {
                Console.WriteLine("Call a guest in first!");
                return;
            }
            Console.WriteLine("Menu: ");
            foreach (MealCategory category in Enum.GetValues(typeof(MealCategory)))
            {
                string servedMark = servedThisVisit.ContainsKey(category) ? $" (served: {servedThisVisit[category]})" : "";
                Console.WriteLine($"{category}{servedMark}: {string.Join(", ", currentMenu[category])}");
            }
            Update();
        }

        /// <summary>
        /// Deliver a meal to the person behind the counter
        /// </summary>
        /// <param name="dishName"></param>
        public void ServeDish(string dishName)
        {
            if (!CheckGameStart()) return;
            if (currentGuest == null)
            {
                Console.WriteLine("No guest to be served!");
                return;
            }
            if (dishName == null)
            {
                Console.WriteLine("Choose a dish!");
                return;
            }

            if (!cookBook.TryGetValue(dishName, out FoodParams? dishParams))
            {
                Console.WriteLine($"We do not have {dishName} in our cookbook");
                return;
            }

            MealCategory category = dishParams.Category; //get the category of the dish
            if (!currentMenu[category].Contains(dishName))
            {
                Console.WriteLine($"There is no \"{dishName}\" available at the moment.");
                return;
            }

            if (servedThisVisit.ContainsKey(category))      //prevent serving same category twice
            {
                Console.WriteLine($"The guest does not want another {category}.");
                return;
            }

            servedThisVisit[category] = dishName;           //lock the served category
            Console.WriteLine($"Served {dishName} as the {category}.");

            if (servedThisVisit.Count == Enum.GetValues(typeof(MealCategory)).Length)
            {
                SendGuest(); //call sendGuest() only if all categories are served. this removes the need to add dishname parameter to sendGuest() and calculateComfort()
            }
            Update();
        }

        /*/// <summary>
        /// A full encyclopedia ready to help you whenever you forget what elves are allergic to
        /// </summary>
        public void GetEncyclopedia()
        {
            if (!CheckGameStart()) return;

            if (currentGuest == null)
            {
                Console.WriteLine("Call a guest first to update the encyclopedia!");
                return;
            }
            if (!encyclopedia.ContainsKey(currentGuest.Name))
            {
                string[] names = Enum.GetNames(typeof(FoodParamsEnum));
                List<string> likes = new List<string>();
                List<string> dislikes = new List<string>();
                for (int i = 0; i < Enum.GetValues(typeof(FoodParamsEnum)).Length; i++)
                {
                    if (currentGuest.preferences[i]) likes.Add(names[i]);
                    else dislikes.Add(names[i]);
                }
                Dictionary<string, List<string>> dict = new Dictionary<string, List<string>>();
                dict.Add("Likes", likes);
                dict.Add("Dislikes", dislikes);
                encyclopedia.Add(currentGuest.GetType().ToString(), dict);
            }

            foreach (string name in encyclopedia.Keys)
            {
                Console.WriteLine($"{name} likes: ");
                foreach (string l in encyclopedia[name]["Likes"]) { Console.WriteLine("- " + l); }
                Console.WriteLine($"{name} dislikes: ");
                foreach(string d in encyclopedia[name]["Dislikes"]) { Console.WriteLine("- " + d); }
            }

            Update();
        }*/

        public void GetEncyclopedia()
        {
            if (!CheckGameStart()) return;
            Console.WriteLine("""
            ---The Gourmet's Guide to the Tavern Management---

            Among 42 different races living in Ashen Lands; Elves, Dwarves and Orcs are the most populated ones.
            Naturally, not everyone can eat the same food. An Elvish plant can kill an Orc, and vice versa.

            While managing a tavern, restraunt or an inn, you are responsible for the health of your guests.
            So I wrote this guidebook to help new innkeepers about this unfortunately very common health issue.
            Below are the common knowledge about different peoples diets:

            Elves:
            Most Elves prefer a plant based diet and avoid meat and diary based meals.
            They're usually not fond of spicy food.
            Usually they lack a sweet tooth and salty pastries are their dessert preference.

            Dwarves:
            Opposing to the Elves, a basic Dwarven diet consists of all kinds of red and white meat,
            Harsh living conditions forced them to use animal products in every way possible, and naturally,
            milk, butter and other milk based products became a part of their usual diet.

            Orcs:
            Orcish cuisine is a mix of meat and ash-grown vegetables. Bad soil produced tasteless vegetables,
            Orcs solved this problem making stews with heavy spices. Their cuisine lacks seafood due to them
            living in landlocked provinces.
            """);
            Update();
        }

    }

}
