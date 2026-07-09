using System.Timers;

namespace ConsoleApp1
{
    public enum GuestRaces
    {
        Elf,
        Dwarf,
        Orc
    }

    abstract class Guest
    {
        private bool isFedUp;
        float valueDefault = 5f;
        private readonly System.Timers.Timer timer;
        public event EventHandler guestFedUp;
        public bool[] preferences { get; private protected set; }
        public string Name { get; protected set; } = "Guest";
        public virtual float comfortValue(bool ingredient, float value)
        {
            if (ingredient)
                return value;
            return valueDefault;
        }

        public Guest(double interval)
        {
            isFedUp = false;
            preferences = new bool[FoodParams.attribCount];
            timer = new System.Timers.Timer(interval);
            timer.Elapsed += OnCustomerFedUp;
            timer.Elapsed += OnWaitTooLong;
            timer.AutoReset = true;
            timer.Enabled = true;
        }

        public void Reset()
        {
            timer.Stop();
            timer.Dispose();
        }

        public virtual void OnCustomerFedUp(Object? source, ElapsedEventArgs e) //TODO
        {
            if (isFedUp)
            {
                Console.WriteLine($"{Name} is fed up and decided to leave!");
                guestFedUp?.Invoke(this, EventArgs.Empty);
            }
        }
        public virtual void OnWaitTooLong(Object? source, ElapsedEventArgs e) //TODO
        {
            if (isFedUp)
            {
                return;
            }
            Console.Beep();
            isFedUp = true;
        }
    }

    class Elf : Guest
    {
        public Elf(double interval) : base(interval)
        {
            Name = "Elf";
            //preferences = Rand.RandomBools(FoodParams.attribCount);
            preferences[2] = true;
            preferences[3] = true;
            preferences[4] = true;
            preferences[9] = true;
        }

        /*public override void OnCustomerFedUp(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnWaitTooLong(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }*/
    }
    class Dwarf : Guest
    {
        public Dwarf(double interval) : base(interval)
        {
            Name = "Dwarf";
            //preferences = Rand.RandomBools(FoodParams.attribCount);
            preferences[0] = true;
            preferences[1] = true;
            preferences[5] = true;
            preferences[7] = true;
            preferences[8] = true;
        }

        /*public override void OnCustomerFedUp(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnWaitTooLong(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }*/
    }
    class Orc : Guest
    {
        public Orc(double interval) : base(interval)
        {
            Name = "Orc";
            //preferences = Rand.RandomBools(FoodParams.attribCount);
            preferences[0] = true;
            preferences[2] = true;
            preferences[3] = true;
            preferences[4] = true;
            preferences[6] = true;
        }

        /*public override void OnCustomerFedUp(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public override void OnWaitTooLong(Object? source, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }*/
    }
}