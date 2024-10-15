namespace ImageParser
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Please provide a directory to scan.");
                return;
            }

            string directoryPath = args[0];

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("The provided directory does not exist.");
                return;
            }

            var parser = new ImageParser(directoryPath);
            parser.ParseDirectory();
        }
    }
}
