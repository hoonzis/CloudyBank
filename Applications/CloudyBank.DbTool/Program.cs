using System;
using CloudyBank.DbTool.Technical;
using System.Diagnostics;

namespace CloudyBank.DbTool
{
    static class Program
    {

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                Console.Write("Need valid database name !");
                Console.Write("i.e well-defined in app.config");
                return;
            }
            
            String dbName = args[0];

            String message = DbGenerator.GenerateDatabase(dbName);
            Console.WriteLine(message);

            bool created = false;
            try
            {
                DbGenerator.GenerateSchema(dbName);
                created = true;
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

            //if the db was succesfully created, generate the data
            if (created)
            {
                //ContextInstance.Create();
                Console.WriteLine(String.Format("Database '{0}' successfully created!", dbName));
                Stopwatch sw = Stopwatch.StartNew();
                sw.Start();

                DbGenerator.GenerateData();
                sw.Stop();
                Console.WriteLine(String.Format("Data generated in: {0}", sw.ElapsedMilliseconds));
                sw.Restart();
                DbGenerator.ComputeData(sw);
                
            }
            Console.Write("Press any key to close");
            Console.ReadKey();
        }
    }
}
