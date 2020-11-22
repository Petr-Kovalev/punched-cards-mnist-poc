using System;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.Helpers
{
    internal static class BinaryStringsHelper
    {
        private const char NumberZeroCharacter = '0';
        private const char NumberOneCharacter = '1';

        internal static int CalculateMatchingScore(ICollection<int> inputOneIndices, Tuple<string, int> punchedInput)
        {
            return inputOneIndices.Count(inputOneIndex => punchedInput.Item1[inputOneIndex] == NumberOneCharacter) *
                   punchedInput.Item2;
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

        private static string ByteToBinaryString(byte value, int bitSize)
        {
            return Convert.ToString(value, 2).PadLeft(bitSize, NumberZeroCharacter);
        }
    }
}