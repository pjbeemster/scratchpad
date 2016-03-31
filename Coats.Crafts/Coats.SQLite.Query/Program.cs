using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Coats.SQLite.Query
{
    class Program
    {
        static int Main(string[] args)
        {
            ShowWelcome();

            if (args.Length != 2 || args.Contains("-?") || args.Contains("?"))
            {
                ShowUsage();
                return 0;
            }

            SQLiteDatabase sqlite = new SQLiteDatabase();

            switch (args[0].ToLower())
            {
                case "-e":
                    return CheckExists(args[1]);
                    break;
                case "-i":
                    return Insert(args[1]);
                    break;
                case "-d":
                    return Delete(args[1]);
                    break;
                default:
                    Console.WriteLine("Unrecognised action. Must be -e | -i | -d");
                    Console.WriteLine();
                    ShowUsage();
                    break;
            }

            Console.WriteLine("Press any key to exit");
            var keyPressed = Console.ReadKey(false).KeyChar;
            return 0;

        }

        private static void ShowWelcome()
        {
            Console.WriteLine("SQLite Query Utility");
            Console.WriteLine("====================");
            Console.WriteLine();
        }

        private static void ShowUsage()
        {
            Console.WriteLine("This is a simple Exists / Insert / Delete utility");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine();
            Console.WriteLine("-e [tcmId] - e.g. SQLiteQuery -e tcm_70-18237-16_tcm_70-18240-32");
            Console.WriteLine("-i [tcmId] - e.g. SQLiteQuery -i tcm_70-18237-16_tcm_70-18240-32");
            Console.WriteLine("-d [tcmId] - e.g. SQLiteQuery -d tcm_70-18237-16_tcm_70-18240-32");
            Console.WriteLine();
        }

        private static int CheckExists(string tcmId)
        {
            SQLiteDatabase sqlite = new SQLiteDatabase();
            bool exists = sqlite.TcmIdExists(tcmId);
            Console.WriteLine(string.Format("TcmId [{0}] {1} present in {2}", tcmId, exists ? "IS" : "is NOT", sqlite.Database));
            return exists ? 0 : 1;
        }

        private static int Insert(string tcmId)
        {
            SQLiteDatabase sqlite = new SQLiteDatabase();
            bool success = sqlite.InsertTcmId(tcmId);
            Console.WriteLine(string.Format("Insert TcmId [{0}] into {1} : {2}", tcmId, sqlite.Database, success ? "SUCESS" : "FAILED"));
            return success ? 0 : 1;
        }

        private static int Delete(string tcmId)
        {
            Console.WriteLine();
            Console.Write("Are you sure you wish to delete TcmId [" + tcmId + "] (y/n)");
            var keyPressed = Console.ReadKey(false).KeyChar;
            if (keyPressed == 'y' || keyPressed == 'Y')
            {
                SQLiteDatabase sqlite = new SQLiteDatabase();
                bool success = sqlite.DeleteTcmId(tcmId);
                Console.WriteLine(string.Format("Delete TcmId [{0}] from {1} : {2}", tcmId, sqlite.Database, success ? "SUCESS" : "FAILED"));
                return success ? 0 : 1;
            }
            Console.WriteLine("Deletion cancelled");
            return -1;
        }
    }
}
