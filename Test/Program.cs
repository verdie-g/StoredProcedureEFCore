using System;
using System.Collections;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var da = new DataAccessBase();
            var rows = da.ListRowsFromTable1();
            Console.Read();
        }
    }
}
