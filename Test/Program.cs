using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;


namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var da = new DataAccessBase();
            var sw = new Stopwatch();
            sw.Start();
            //da.LoadStoredProcedureHardCoded();
            //Console.WriteLine("Hardcoded: " + sw.ElapsedMilliseconds + " ms");
            //sw.Restart();
            da.LoadStoredProcedureReflective();
            Console.WriteLine("Reflective: " + sw.ElapsedMilliseconds + " ms");
            //da.LoadStoredProcedureReflectiveWithOptimisation();
            //Console.WriteLine("Optimised Reflective: " + sw.ElapsedMilliseconds + " ms");
            Console.Read();
        }
    }
}
