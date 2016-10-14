using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainfucker
{
    public abstract class BrainfuckRunner
    {
        public int RunFile(string path)
        {
            using (StreamReader brainfuckReader = new StreamReader(path))
            {
                return Run(brainfuckReader.ReadToEnd());
            }
        }

        public int Run(Stream brainfuck)
        {
            using (StreamReader brainfuckReader = new StreamReader(brainfuck))
            {
                return Run(brainfuckReader.ReadToEnd());
            }
        }

        public abstract int Run(string brainfuck);
    }
}
