using System;

namespace HashCode2017 {
    public class Program {
        public static void Main(string[] args) {
            new Program().Run();
        }

        private void Run() {
            int totalScore =
                new Instance("me_at_the_zoo").RunB() + // Second way: through video requests (B)
                new Instance("kittens").RunC() + // Third way: like B but with video size weighting (C)
                new Instance("trending_today").RunA() + // First way: through end points, through video requests (A)
                new Instance("videos_worth_spreading").RunC(); // (C)

            Console.WriteLine();
            Console.WriteLine("Total score: " + totalScore);
            Console.Read();
        }
    }
}
