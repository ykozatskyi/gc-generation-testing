using System;
using System.Threading;

//GC (Garbage collector) using 3 generations to manage actual object in managed heap:
//Generation 1 - objects in heap which were created recently and still were not handled by GC
//Generation 2 - objects that survived in 1 GC.Collect()
//Generation 3 - objects that survived in more than 1 GC.Collect()
//Generation N - depend on system

namespace GCGenerationsTester
{
    class ClassFoo
    {
        byte[] foo = new byte[1024]; //1 KB

        public ClassFoo() => Console.WriteLine($"ClassFoo constructor. Hash: {this.GetHashCode()}. Thread: {Thread.CurrentThread.ManagedThreadId}");

        /// <summary>
        /// Finalizer
        /// </summary>
        ~ClassFoo() => Console.WriteLine($"ClassFoo destructor. Hash: {this.GetHashCode()}. Thread: {Thread.CurrentThread.ManagedThreadId}");
    }

    class ClassBar
    {
        byte[] foo = new byte[1024 * 50]; //100 KB
    }

    class Program
    {
        /// <summary>
        /// Fill the heap of ClassBar objects
        /// </summary>
        static void BarsFactory()
        {
            ClassBar[] array = new ClassBar[1000];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = new ClassBar();
                Thread.Sleep(20);
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Managed heap generations tester\n");
            Console.WriteLine(new string('-', 30));

            //object to keep in heap
            ClassFoo classFooObj = new ClassFoo();

            Console.WriteLine($"This system supports maximum {GC.MaxGeneration + 1} generations");
            Console.WriteLine(new string('-', 30));

            new Thread(BarsFactory).Start();
            for (int i = 0; i < 10; i++)
            {
                //every second check classFooObj in the heap
                Console.Write($"{nameof(classFooObj)} generation: {GC.GetGeneration(classFooObj)} | ");
                Console.WriteLine($"Heap size: {GC.GetTotalMemory(false) / 1024} KB");
                Thread.Sleep(1000);
            }

            Console.WriteLine(new string('-', 30));

            for (int i = 0; i < (GC.MaxGeneration + 1); i++)
                Console.WriteLine($"Generation {i} was checked {GC.CollectionCount(i)} times");

            Console.WriteLine(new string('-', 30));
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }
    }
}
