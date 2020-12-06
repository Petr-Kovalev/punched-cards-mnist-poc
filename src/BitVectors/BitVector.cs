using System.Collections.Generic;

namespace PunchedCards.BitVectors
{
    internal abstract class BitVector: IBitVector
    {
        public abstract int Count { get; }

        public abstract int AndCardinality(IBitVector bitVector);

        public abstract IBitVector Punch(IEnumerable<int> indices);

        public abstract bool IsBitActive(int index);

        protected IEnumerable<int> PunchInternal(IEnumerable<int> indices)
        {
            var currentIndex = 0;

            foreach (var index in indices)
            {
                if (IsBitActive(index))
                {
                    yield return currentIndex;
                }

                currentIndex++;
            }
        }
    }
}