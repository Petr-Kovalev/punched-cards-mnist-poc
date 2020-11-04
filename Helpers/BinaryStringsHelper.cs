using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunchedCards.Helpers
{
    internal static class BinaryStringsHelper
    {
        private const char NumberZeroCharacter = '0';
        private const char NumberOneCharacter = '1';

        internal static int CalculateMatchingScore(ICollection<int> inputOneIndices, Tuple<string, int> punchInput)
        {
            return inputOneIndices
                       .Count(inputOneIndex => punchInput.Item1[inputOneIndex] == NumberOneCharacter)
                   * punchInput.Item2;
        }

        internal static IEnumerable<int> GetOneIndices(string input)
        {
            for (var i = 0; i < input.Length; i++)
            {
                if (input[i] == NumberOneCharacter)
                {
                    yield return i;
                }
            }
        }

        internal static string GetLabelString(byte label, byte labelCount)
        {
            return ByteToBinaryString(label, 4);

            // SDR optional implementation
            //var characterArray = new char[labelCount];
            //for (var i = 0; i < labelCount; i++)
            //{
            //    characterArray[i] = i == label ? NumberOneCharacter : NumberZeroCharacter;
            //}
            //return new string(characterArray);
        }

        internal static string GetValueString(byte[,] imageData)
        {
            const byte width = 28;
            const byte height = 28;
            const int bitSize = 8;

            var valueStringBuilder = new StringBuilder();
            for (byte i = 0; i < width; i++)
            {
                for (byte j = 0; j < height; j++)
                {
                    valueStringBuilder.Append(ByteToBinaryString(imageData[i, j], bitSize));
                }
            }

            return valueStringBuilder.ToString();
        }

        private static string ByteToBinaryString(byte value, int bitSize)
        {
            return Convert.ToString(value, 2).PadLeft(bitSize, NumberZeroCharacter);
        }
    }
}