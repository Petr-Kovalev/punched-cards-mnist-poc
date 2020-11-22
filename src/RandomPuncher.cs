using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace PunchedCards
{
    internal sealed class RandomPuncher: IPuncher<string, BitArray, string>
    {
        private const char NumberZeroCharacter = '0';
        private const char NumberOneCharacter = '1';
        private const char KeySeparator = '-';

        private static readonly Random Random = new Random(42);

        private readonly int _bitLength;

        private int _lastLength = int.MinValue;
        private int[][] _map;

        internal RandomPuncher(int bitLength)
        {
            _bitLength = bitLength;
        }

        public IEnumerable<IPunchedCard<string, string>> GetInputPunches(BitArray input)
        {
            if (_lastLength != input.Length)
            {
                _lastLength = input.Length;
                ReinitializeMap(input.Length);
            }

            for (var mapIndex = 0; mapIndex < _map.Length; mapIndex++)
            {
                yield return new PunchedCard<string, string>(GetKey(_map[mapIndex]), GetInputPunch(input, _map[mapIndex]));
            }
        }

        public IPunchedCard<string, string> Punch(string key, BitArray input)
        {
            return new PunchedCard<string, string>(key, GetInputPunch(input, GetIndices(key)));
        }

        private static IEnumerable<int> GetIndices(string key)
        {
            var startPosition = 0;
            int position;
            while ((position = key.IndexOf(KeySeparator, startPosition)) > 0)
            {
                yield return int.Parse(key.Substring(startPosition, position - startPosition));
                startPosition = position + 1;
            }

            yield return int.Parse(key.Substring(startPosition));
        }

        private static string GetKey(IEnumerable<int> indices)
        {
            var keyStringBuilder = new StringBuilder();
            foreach (var index in indices)
            {
                keyStringBuilder.Append(index);
                keyStringBuilder.Append(KeySeparator);
            }

            return keyStringBuilder.ToString(0, keyStringBuilder.Length - 1);
        }

        private static string GetInputPunch(BitArray input, IEnumerable<int> indices)
        {
            var inputPunchStringBuilder = new StringBuilder();
            foreach (var index in indices)
            {
                inputPunchStringBuilder.Append(input[index] ? NumberOneCharacter : NumberZeroCharacter);
            }

            return inputPunchStringBuilder.ToString();
        }

        private void ReinitializeMap(int length)
        {
            var usedIndexHashSet = new HashSet<int>();
            var rowsCount = length / _bitLength;
            _map = new int[rowsCount][];

            for (var i = 0; i < rowsCount; i++)
            {
                var row = new int[_bitLength];
                for (var j = 0; j < _bitLength; j++)
                {
                    int index;
                    do
                    {
                        index = Random.Next(length);
                    } while (!usedIndexHashSet.Add(index));

                    row[j] = index;
                }

                _map[i] = row;
            }
        }
    }
}