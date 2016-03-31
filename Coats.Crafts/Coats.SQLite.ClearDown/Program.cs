using System;

namespace Coats.SQLite.ClearDown
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SQLite Clear Down Utility");
            Console.WriteLine("=========================");
            Console.WriteLine();
            Console.WriteLine("WARNING!!! : THIS UTILITY WILL DELETE ALL DATA FROM THE tcm TABLE AND CANNOT BE RECOVERED");
            Console.WriteLine();
            Console.Write("Are you sure you wish to proceed (y/n) ? : ");
            
            var keyPressed = Console.ReadKey(false).KeyChar;
            if (keyPressed == 'y' || keyPressed == 'Y')
            {
                SQLiteDatabase sqlite = new SQLiteDatabase();
                sqlite.ClearDownTable();
            }

            Console.WriteLine();
            Console.WriteLine("Table cleared down");
        }
    }
}
