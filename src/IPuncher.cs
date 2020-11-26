using System.Collections.Generic;

namespace PunchedCards
{
    internal interface IPuncher<TKey, in TInput, out TOutput>
    {
        IPunchedCard<TKey, TOutput> Punch(TKey key, TInput input);
        IEnumerable<string> GetKeys(int count);
    }
}