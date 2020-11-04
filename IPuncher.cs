using System.Collections.Generic;

namespace PunchedCards
{
    internal interface IPuncher<TKey, in TInput, out TOutput>
    {
        IEnumerable<IPunchedCard<TKey, TOutput>> GetInputPunches(TInput input);
        IPunchedCard<TKey, TOutput> Punch(TKey key, TInput input);
    }
}