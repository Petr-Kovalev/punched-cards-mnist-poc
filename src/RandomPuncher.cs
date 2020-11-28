using System;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards
{
    internal sealed class RandomPuncher : IPuncher<string, IBitVector, IBitVector>
    {
        private static readonly Random Random = new Random(42);

        private readonly int _bitLength;

        private int _lastCount = int.MinValue;
        private int[][] _map;

        internal RandomPuncher(int bitLength)
        {
            _bitLength = bitLength;
        }

        public IPunchedCard<string, IBitVector> Punch(string key, IBitVector input)
        {
            return new PunchedCard<string, IBitVector>(key, input.Punch(_map[int.Parse(key)]));
        }

        public IEnumerable<string> GetKeys(int count)
        {
            if (_lastCount != count)
            {
                _lastCount = count;
                ReinitializeMap(count);
            }

            return Enumerable.Range(0, _map.Length).Select(index => index.ToString());
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