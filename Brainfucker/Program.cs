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
                Stopwatch time = new Stopwatch();
                time.Start();
                int result = RunBrainfuck(args);
                time.Stop();

                if (Array.IndexOf(args, "-time") >= 0 || Array.IndexOf(args, "-t") >= 0)
                {

                    Console.WriteLine();
                    Console.WriteLine($"Brainfuck program executed in {time.ElapsedMilliseconds} milliseconds ({time.ElapsedTicks} ticks)");
                }

                return result;
            }
            else
            {
                PrintManual();
                return 0;
            }
        }

        private static int RunBrainfuck(string[] args)
        {
            if (File.Exists(args[0]))
            {

                BrainfuckRunner brainfuckRunner;
                if (Array.IndexOf(args, "-compile") >= 0 || Array.IndexOf(args, "-compiled") >= 0 || Array.IndexOf(args, "-c") >= 0)
                {

                    brainfuckRunner = new CompiledBrainfuckInterpreter();
                }
                else if (Array.IndexOf(args, "-immediate") >= 0 || Array.IndexOf(args, "-i") >= 0)
                {
                    brainfuckRunner = new ImmediateBrainfuckInterpreter();
                }
                else
                {
                    brainfuckRunner = new ImmediateBrainfuckInterpreter();
                }

                try
                {
                    return brainfuckRunner.RunFile(args[0]);

                }
                catch (IndexOutOfRangeException)
                {
                    Console.WriteLine("Segmentation fault.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                return 1;
            }
            else
            {
                Console.WriteLine("The specified file could not be found.");
                return 2;
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
            Console.WriteLine("-c\tCompiles the Brainfuck file and runs it. This may result in a significant performance increase. Alternative options are -compile and -compiled");
            Console.WriteLine("-t\tPrints the execution time after the Brainfuck program finishes. An alternative option is -time");
            Console.WriteLine("-?\tPrints this help text. An alternative option is -help");
        }
    }
}
