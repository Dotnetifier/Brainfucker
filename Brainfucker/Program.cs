using System;
using System.Diagnostics;
using System.IO;

namespace Brainfucker
{
    public class Program
    {
        public static int Main(string[] args)
        {
            if (args.Length > 0 && !(Array.IndexOf(args, "-?") >= 0) && !(Array.IndexOf(args, "-help") >= 0))
            {
                if (File.Exists(args[0]))
                {
                    int result = RunBrainfuck(args);

                    return result;
                }
                else
                {
                    Console.WriteLine("The specified file could not be found.");
                    return 2;
                }
            }
            else
            {
                PrintManual();
                return 0;
            }
        }

        private static int RunBrainfuck(string[] args)
        {
            int obIndex = Array.IndexOf(args, "-ob");
            if (args.Length > obIndex + 1)
            {
                int obTime;
                if (int.TryParse(args[obIndex + 1], out obTime))
                {
                    OutputBuffer.Open(obTime);
                }
            }

            Stopwatch time = new Stopwatch();
            try
            {
                using (StreamReader brainfuckReader = new StreamReader(args[0]))
                {
                    string brainfuck = brainfuckReader.ReadToEnd();
                    time.Start();
                    return CreateBrainfuckRunner(args).Run(brainfuck);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                time.Stop();

                OutputBuffer.Close();
                if (Array.IndexOf(args, "-time") >= 0 || Array.IndexOf(args, "-t") >= 0)
                {

                    Console.WriteLine();
                    Console.WriteLine($"Brainfuck program executed in {time.ElapsedMilliseconds} milliseconds ({time.ElapsedTicks} ticks)");
                }
            }
            return 1;
        }

        private static BrainfuckRunner CreateBrainfuckRunner(string[] args)
        {
            if (Array.IndexOf(args, "-matched") >= 0 || Array.IndexOf(args, "-m") >= 0)
            {
                return new MatchedBrainfuckInterpreter();
            }
            else if (Array.IndexOf(args, "-compress") >= 0 || Array.IndexOf(args, "-compressed") >= 0 || Array.IndexOf(args, "-c") >= 0)
            {
                return new CompressedBrainfuckInterpreter();
            }
            else if (Array.IndexOf(args, "-immediate") >= 0 || Array.IndexOf(args, "-i") >= 0)
            {
                return new ImmediateBrainfuckInterpreter();
            }
            else
            {
                return new ImmediateBrainfuckInterpreter();
            }
        }

        private static void PrintManual()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("The first argument must be a file path, either absolute or relative. Once the file is read in, it will be run as a Brainfuck file.");
            Console.WriteLine();
            Console.WriteLine("You can specify the following options:");
            Console.WriteLine("-i\tUses the immediate interpreter. The Brainfuck program is interpreted immediately. An alternative option is -immediate");
            Console.WriteLine("-m\tMatches brackets in the Brainfuck file and runs it. This may result in a significant performance increase. An alternative option is -matched");
            Console.WriteLine("-c\tCompresses the Brainfuck instructions and runs it. This option also uses the matching optimization. Alternative options are -compress and -compressed");
            Console.WriteLine("-t\tPrints the execution time after the Brainfuck program finishes. An alternative option is -time");
            Console.WriteLine("-ob X\tBuffers the program output for X milliseconds. May increase performance");
            Console.WriteLine("-?\tPrints this help text. An alternative option is -help");
        }
    }
}
