using System.Timers;

namespace ConsoleApp1
{
    internal class GuestHandler
    {
        private const double defaultGuestFrequency = 6000000;//miliseconds
        private const double defaultGuestWaitTime = 9000000;//miliseconds
        private const double guestFrequencyModifier = 1000;//miliseconds
        private System.Timers.Timer timer;
        private Queue<Guest> guestQueue;
        private static readonly List<Func<double, Guest>> guestPool = new List<Func<double, Guest>>()
        {
            (interval)=>new Elf(interval),
            (interval)=>new Dwarf(interval),
            (interval)=>new Orc(interval)
            //expandable
        };

        public event EventHandler gameOver;

        public GuestHandler()
        {
            timer = new System.Timers.Timer(defaultGuestFrequency);
            timer.AutoReset = true;
            timer.Elapsed += AddGuest;
            timer.Enabled = false;
            guestQueue = new Queue<Guest>();
        }

        public void Reset()
        {
            foreach (var guest in guestQueue)
            {
                guest.Reset();
                guest.guestFedUp -= GameOver;
            }
            guestQueue.Clear();
            timer.Stop();
            timer.Dispose();
        }

        public void Start()
        {
            timer.Enabled = true;
            int i = Rand.rng.Next(guestPool.Count);
            Guest guest = guestPool[i](defaultGuestWaitTime);
            guest.guestFedUp += GameOver;
            guestQueue.Enqueue(guest);
            Console.WriteLine("A new Guest arrived and is waiting in line.");
        }

        private void AddGuest(Object? source, ElapsedEventArgs e)
        {
            int index = Rand.rng.Next(guestPool.Count);
            Guest guest = guestPool[index](defaultGuestWaitTime);
            guest.guestFedUp += GameOver;
            guestQueue.Enqueue(guest);
            Console.WriteLine("A new Guest arrived and is waiting in line.");
            if (timer.Interval >= 2000) { timer.Interval -= guestFrequencyModifier; }
        }

        private void GameOver(Object? sender, EventArgs e)
        {
            gameOver?.Invoke(this, EventArgs.Empty);
        }

        public Guest RemoveGuest()
        {
            return guestQueue.Dequeue();
        }

    }
}