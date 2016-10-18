using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brainfucker
{
    public class CompressedBrainfuckInterpreter : BrainfuckRunner
    {
        public const int N_CELLS = 30000;

        private Stack<int> _stack = new Stack<int>();

        private const byte INSTRUCTION_INCREMENT_DATA_POINTER = 0;
        private const byte INSTRUCTION_DECREMENT_DATA_POINTER = 1;
        private const byte INSTRUCTION_INCREMENT_DATA = 2;
        private const byte INSTRUCTION_DECREMENT_DATA = 3;
        private const byte INSTRUCTION_OUTPUT_BYTE = 4;
        private const byte INSTRUCTION_INPUT_BYTE = 5;
        private const byte INSTRUCTION_JUMP_FORWARD = 6;
        private const byte INSTRUCTION_JUMP_BACKWARD = 7;

        public override int Run(string brainfuck)
        {
            string shortBrainfuck = string.Join("", brainfuck.Where(c => c == '+' || c == '-' || c == '<' || c == '>' || c == '[' || c == ']' || c == '.' || c == ','));

            ExecuteBrainfuck(CompressBrainfuck(shortBrainfuck));
            return 0;
        }

        private int[] CompressBrainfuck(string brainfuck)
        {
            int programLength = brainfuck.Length;
            int[] instructions = new int[programLength * 2];
            int programCounter = 0;

            char previousCharacter = brainfuck[0];
            int instructionIndex = 0, operandIndex = 1;
            while (programCounter < programLength)
            {

                char currentCharacter = brainfuck[programCounter];
                if (programCounter > 0)
                {
                    if (currentCharacter == previousCharacter && (currentCharacter == '+' || currentCharacter == '-' || currentCharacter == '<' || currentCharacter == '>'))
                    {
                        instructions[operandIndex]++;
                        programCounter++;
                        continue;
                    }
                    else
                    {
                        instructionIndex += 2;
                        operandIndex += 2;
                    }
                }

                switch (currentCharacter)
                {
                    case '>':
                        instructions[instructionIndex] = INSTRUCTION_INCREMENT_DATA_POINTER;
                        instructions[operandIndex] = 1;
                        break;
                    case '<':
                        instructions[instructionIndex] = INSTRUCTION_DECREMENT_DATA_POINTER;
                        instructions[operandIndex] = 1;
                        break;
                    case '+':
                        instructions[instructionIndex] = INSTRUCTION_INCREMENT_DATA;
                        instructions[operandIndex] = 1;
                        break;
                    case '-':
                        instructions[instructionIndex] = INSTRUCTION_DECREMENT_DATA;
                        instructions[operandIndex] = 1;
                        break;
                    case '.':
                        instructions[instructionIndex] = INSTRUCTION_OUTPUT_BYTE;
                        break;
                    case ',':
                        instructions[instructionIndex] = INSTRUCTION_INPUT_BYTE;
                        break;
                    case '[':
                        instructions[instructionIndex] = INSTRUCTION_JUMP_FORWARD;
                        _stack.Push(instructionIndex);
                        break;
                    case ']':
                        instructions[instructionIndex] = INSTRUCTION_JUMP_BACKWARD;
                        int matchingBracket = _stack.Pop();
                        instructions[instructionIndex + 1] = matchingBracket;
                        instructions[matchingBracket + 1] = instructionIndex;
                        break;
                }

                programCounter++;
                previousCharacter = currentCharacter;
            }

            Array.Resize(ref instructions, operandIndex + 1);
            return instructions;
        }

        private void ExecuteBrainfuck(int[] instructions)
        {
            unchecked
            {
                int programCounter = 0;
                int dataPointer = 0;
                byte[] cells = new byte[N_CELLS];

                while (programCounter < instructions.Length)
                {
                    int twoProgramCounterPlusOne = programCounter + 1;

                    switch (instructions[programCounter])
                    {
                        case INSTRUCTION_INCREMENT_DATA_POINTER:
                            dataPointer += instructions[twoProgramCounterPlusOne];
                            break;
                        case INSTRUCTION_DECREMENT_DATA_POINTER:
                            dataPointer -= instructions[twoProgramCounterPlusOne];
                            break;
                        case INSTRUCTION_INCREMENT_DATA:
                            cells[dataPointer] += (byte)instructions[twoProgramCounterPlusOne];
                            break;
                        case INSTRUCTION_DECREMENT_DATA:
                            cells[dataPointer] -= (byte)instructions[twoProgramCounterPlusOne];
                            break;
                        case INSTRUCTION_OUTPUT_BYTE:
                            OutputBuffer.WriteByte(cells[dataPointer]);
                            break;
                        case INSTRUCTION_INPUT_BYTE:
                            cells[dataPointer] = (byte)Console.Read();
                            break;
                        case INSTRUCTION_JUMP_FORWARD:
                            if (cells[dataPointer] == 0)
                            {
                                programCounter = instructions[twoProgramCounterPlusOne];
                            }
                            break;
                        case INSTRUCTION_JUMP_BACKWARD:
                            if (cells[dataPointer] != 0)
                            {
                                programCounter = instructions[twoProgramCounterPlusOne];
                            }
                            break;
                    }

                    programCounter += 2;
                }
            }
        }
    }
}
