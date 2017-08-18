using System;
using Xigadee;

namespace Test.Samples.Sample1
{
    class Program
    {
        static void Main(string[] args)
        {
            var pipeline = new MicroservicePipeline("Server");

            pipeline.Start();
            Console.WriteLine("Press a key to stop.");
            Console.ReadKey();
            pipeline.Stop();
            Console.WriteLine("Service stopped.");
            Console.ReadKey();

        }
    }
}
