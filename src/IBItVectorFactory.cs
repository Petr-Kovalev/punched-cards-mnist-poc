using System.Collections.Generic;

namespace PunchedCards
{
    internal interface IBitVectorFactory
    {
        IBitVector Create(IEnumerable<int> indices, int count);

        IBitVector Create(IEnumerable<bool> booleanEnumerable);
    }
}