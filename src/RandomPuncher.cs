using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PunchedCards
{
    internal sealed class RandomPuncher : IPuncher<string, BitArray, BitArray>
    {
        private const char KeySeparator = '-';

        private static readonly Random Random = new Random(42);

        private readonly int _bitLength;

        private int _lastCount = int.MinValue;
        private int[][] _map;

        internal RandomPuncher(int bitLength)
        {
            _bitLength = bitLength;
        }

        public IEnumerable<IPunchedCard<string, BitArray>> GetInputPunches(BitArray input)
        {
            if (_lastCount != input.Count)
            {
                _lastCount = input.Count;
                ReinitializeMap(input.Count);
            }

            for (var mapIndex = 0; mapIndex < _map.Length; mapIndex++)
            {
                yield return new PunchedCard<string, BitArray>(GetKey(_map[mapIndex]), GetInputPunch(input, _map[mapIndex]));
            }
        }

        public IPunchedCard<string, BitArray> Punch(string key, BitArray input)
        {
            return new PunchedCard<string, BitArray>(key, GetInputPunch(input, GetIndices(key).ToList()));
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

        private static BitArray GetInputPunch(BitArray input, IReadOnlyCollection<int> indices)
        {
            var booleanArray = new bool[indices.Count];

            var currentIndex = 0;
            foreach (var index in indices)
            {
                if (input[index])
                {
                    booleanArray[currentIndex] = true;
                }

                currentIndex++;
            }

            return new BitArray(booleanArray);
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