using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards
{
    internal sealed class RandomPuncher : IPuncher<string, IBitVector, IBitVector>
    {
        private static readonly Random Random = new Random(42);

        private readonly int _bitCount;

        private int _lastCount = int.MinValue;
        private int[][] _map;

        internal RandomPuncher(int bitCount)
        {
            _bitCount = bitCount;
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

        private void ReinitializeMap(int count)
        {
            var usedIndexHashSet = new HashSet<int>();
            var rowsCount = count / _bitCount;
            _map = new int[rowsCount][];

            for (var i = 0; i < rowsCount; i++)
            {
                var row = new int[_bitCount];
                for (var j = 0; j < _bitCount; j++)
                {
                    int index;
                    do
                    {
                        index = Random.Next(count);
                    } while (!usedIndexHashSet.Add(index));

                    row[j] = index;
                }

                _map[i] = row;
            }
        }
    }
}