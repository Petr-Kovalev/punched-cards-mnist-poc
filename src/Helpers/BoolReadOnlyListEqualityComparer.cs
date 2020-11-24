using System.Collections.Generic;

namespace PunchedCards.Helpers
{
    internal sealed class BoolReadOnlyListEqualityComparer : IEqualityComparer<IReadOnlyList<bool>>
    {
        internal static IEqualityComparer<IReadOnlyList<bool>> Instance = new BoolReadOnlyListEqualityComparer();

        private BoolReadOnlyListEqualityComparer()
        {
        }

        public bool Equals(IReadOnlyList<bool> x, IReadOnlyList<bool> y)
        {
            if (x == null ||
                y == null ||
                x.Count != y.Count)
            {
                return false;
            }

            for (var i = 0; i < x.Count; i++)
            {
                if (x[i] != y[i])
                {
                    return false;
                }
            }

            return true;
        }

        public int GetHashCode(IReadOnlyList<bool> obj)
        {
            var hash = 17;

            unchecked
            {
                foreach (var bit in obj)
                {
                    hash = hash * 23 + bit.GetHashCode();
                }
            }

            return hash;
        }
    }
}