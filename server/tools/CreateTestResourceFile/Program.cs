using System;
using System.IO;
using System.Xml;

namespace CreateTestResourceFile
{
    class Program
    {
        const string TargetCulture = "zu-ZA";
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Please supply the resource folder name");
                return 1;
            }

            var inputFoldername = args[0];
            if (!Directory.Exists(inputFoldername))
            {
                Console.WriteLine($"The resource folder {inputFoldername} does not exist.");
                return 1;
            }

            Console.Clear();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("*************************************");
            Console.WriteLine("    Create Test Resource Files");
            Console.WriteLine("*************************************");
            Console.WriteLine();
            Console.ResetColor();

            Console.Write("Target Culture : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(TargetCulture);
            Console.ResetColor();

            Console.Write("Looking for resource files in : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine(inputFoldername);
            Console.ResetColor();

            Console.WriteLine();


            foreach (var inputFilename in Directory.GetFiles(inputFoldername, "Messages.resx", SearchOption.AllDirectories))
            {
                ProcessResourceFile(inputFilename, inputFoldername.Length);
            }

            return 0;
        }

        private static void ProcessResourceFile(string inputFilename, int prefixLength)
        {
            Console.Write("     Processing : ");
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(inputFilename.Substring(prefixLength));
            Console.ResetColor();

            try
            {
                var dom = new XmlDocument();
                dom.Load(inputFilename);

                foreach (XmlElement entry in dom.DocumentElement.SelectNodes("data/value"))
                {
                    const string prefix = "'€'";

                    var text = entry.InnerText;

                    if (!text.StartsWith(prefix))
                    {
                        text = prefix + text;

                        entry.InnerText = text;
                    }
                }

                var folder = Path.GetDirectoryName(inputFilename);
                var filename = Path.GetFileName(inputFilename);
                var plain = filename.Split('.')[0];
                var outFile = Path.Combine(folder, plain + "." + TargetCulture + ".resx");

                dom.Save(outFile);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("  -  Success");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("  -  " + ex.Message);
                Console.ResetColor();
            }
        }
    }
}
