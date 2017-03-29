using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using BenchmarkDotNet.Running;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<DataAccessBase>();
            Console.WriteLine(summary);
            //var da = new DataAccessBase();
            //for (int i = 0; i < 5; ++i)
            //{
            //    Task t1 = TestProcedure("HardCoded", da.LoadStoredProcedureHardCoded);
            //    t1.Wait();
            //    Task t2 = TestProcedure("Reflective", da.LoadStoredProcedureReflective);
            //    t2.Wait();
            //    Task t3 = TestProcedure("Optimised Reflective", da.LoadStoredProcedureReflectiveWithOptimisation);
            //    t3.Wait();
            //}
            Console.Read();
        }

        private static Task TestProcedure(string name, Action proc)
        {
            return Task.Run(() =>
            {
                var sw = new Stopwatch();
                sw.Start();
                for (int i = 0; i < 10; ++i)
                {
                    proc();
                }
                sw.Stop();
                Console.WriteLine($"{name}: {sw.ElapsedMilliseconds} ms");
            });
        }
    }
}
