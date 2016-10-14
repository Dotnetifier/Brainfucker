using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainfucker
{
    public class CompiledBrainfuckInterpreter : BrainfuckRunner
    {
        public const int N_CELLS = 30000;
        public const int STACK_SIZE = 1024;

        private Stack<int> _stack = new Stack<int>();

        private const byte INSTRUCTION_INCREMENT_NONE = 0;
        private const byte INSTRUCTION_INCREMENT_DATA_POINTER = 1;
        private const byte INSTRUCTION_DECREMENT_DATA_POINTER = 2;
        private const byte INSTRUCTION_INCREMENT_DATA = 3;
        private const byte INSTRUCTION_DECREMENT_DATA = 4;
        private const byte INSTRUCTION_OUTPUT_BYTE = 5;
        private const byte INSTRUCTION_INPUT_BYTE = 6;
        private const byte INSTRUCTION_JUMP_FORWARD = 7;
        private const byte INSTRUCTION_JUMP_BACKWARD = 8;

        public override int Run(string brainfuck)
        {
            ExecuteBrainfuck(CompileBrainfuck(brainfuck));
            return 0;
        }

        private int[] CompileBrainfuck(string brainfuck)
        {
            int programLength = brainfuck.Length;
            int[] instructions = new int[programLength * 2];
            int programCounter = 0;

            while (programCounter < programLength)
            {
                switch (brainfuck[programCounter])
                {
                    case '>':
                        instructions[2 * programCounter] = INSTRUCTION_INCREMENT_DATA_POINTER;
                        break;
                    case '<':
                        instructions[2 * programCounter] = INSTRUCTION_DECREMENT_DATA_POINTER;
                        break;
                    case '+':
                        instructions[2 * programCounter] = INSTRUCTION_INCREMENT_DATA;
                        break;
                    case '-':
                        instructions[2 * programCounter] = INSTRUCTION_DECREMENT_DATA;
                        break;
                    case '.':
                        instructions[2 * programCounter] = INSTRUCTION_OUTPUT_BYTE;
                        break;
                    case ',':
                        instructions[2 * programCounter] = INSTRUCTION_INPUT_BYTE;
                        break;
                    case '[':
                        instructions[2 * programCounter] = INSTRUCTION_JUMP_FORWARD;
                        _stack.Push(programCounter);
                        break;
                    case ']':
                        instructions[2 * programCounter] = INSTRUCTION_JUMP_BACKWARD;
                        int matchingBracket = _stack.Pop();
                        instructions[2 * programCounter + 1] = matchingBracket;
                        instructions[2 * matchingBracket + 1] = programCounter;
                        break;
                }

                programCounter++;
            }

            return instructions;
        }

        private void ExecuteBrainfuck(int[] instructions)
        {
            unchecked
            {
                int programLength = instructions.Length / 2;
                int programCounter = 0;
                int dataPointer = 0;
                byte[] cells = new byte[N_CELLS];

                while (programCounter < programLength)
                {
                    switch (instructions[2 * programCounter])
                    {
                        case INSTRUCTION_INCREMENT_DATA_POINTER:
                            dataPointer++;
                            break;
                        case INSTRUCTION_DECREMENT_DATA_POINTER:
                            dataPointer--;
                            break;
                        case INSTRUCTION_INCREMENT_DATA:
                            cells[dataPointer]++;
                            break;
                        case INSTRUCTION_DECREMENT_DATA:
                            cells[dataPointer]--;
                            break;
                        case INSTRUCTION_OUTPUT_BYTE:
                            Console.Write((char)cells[dataPointer]);
                            break;
                        case INSTRUCTION_INPUT_BYTE:
                            cells[dataPointer] = (byte)Console.Read();
                            break;
                        case INSTRUCTION_JUMP_FORWARD:
                            if (cells[dataPointer] == 0)
                            {

                                programCounter = instructions[2 * programCounter + 1];

                            }
                            break;
                        case INSTRUCTION_JUMP_BACKWARD:
                            if (cells[dataPointer] != 0)
                            {

                                programCounter = instructions[2 * programCounter + 1];

                            }
                            break;
                    }

                    programCounter++;
                }
            }
        }
    }
}
