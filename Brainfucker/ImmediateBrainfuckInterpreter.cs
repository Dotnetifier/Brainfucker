using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainfucker
{
    public class ImmediateBrainfuckInterpreter : BrainfuckRunner
    {
        public const int N_CELLS = 30000;

        public override int Run(string brainfuck)
        {
            ExecuteBrainfuck(brainfuck);
            return 0;
        }

        private static void ExecuteBrainfuck(string brainfuck)
        {
            byte[] cells = new byte[N_CELLS];
            int programCounter = 0;
            int cellPointer = 0;

            unchecked
            {
                int programLength = brainfuck.Length;

                while (programCounter < programLength)
                {
                    switch (brainfuck[programCounter])
                    {
                        case '>':
                            cellPointer++;
                            break;
                        case '<':
                            cellPointer--;
                            break;
                        case '+':
                            cells[cellPointer]++;
                            break;
                        case '-':
                            cells[cellPointer]--;
                            break;
                        case '.':
                            Console.Write((char)cells[cellPointer]);
                            break;
                        case ',':
                            cells[cellPointer] = (byte)Console.Read();
                            break;
                        case '[':

                            if (cells[cellPointer] == 0)
                            {

                                int bracketPosition = programCounter;
                                int nBrackets = 1;
                                while (nBrackets > 0)
                                {
                                    bracketPosition++;

                                    if (brainfuck[bracketPosition] == '[') nBrackets++;
                                    if (brainfuck[bracketPosition] == ']') nBrackets--;
                                }
                                programCounter = bracketPosition;

                            }
                            break;
                        case ']':
                            if (cells[cellPointer] != 0)
                            {

                                int bracketPosition = programCounter;
                                int nBrackets = 1;
                                while (nBrackets > 0)
                                {
                                    bracketPosition--;

                                    if (brainfuck[bracketPosition] == ']') nBrackets++;
                                    if (brainfuck[bracketPosition] == '[') nBrackets--;
                                }
                                programCounter = bracketPosition;

                            }
                            break;
                    }

                    programCounter++;
                }
            }
        }
    }
}
