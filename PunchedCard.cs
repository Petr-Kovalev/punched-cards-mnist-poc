using System;

namespace PunchedCards
{
    internal class PunchedCard<TKey, TInput> : IPunchedCard<TKey, TInput>
    {
        internal PunchedCard(TKey key, TInput input)
        {
            Key = key;
            Input = input;
        }

        public TKey Key { get; }

        public TInput Input { get; }

        public override int GetHashCode()
        {
            return HashCode.Combine(Key, Input);
        }

        public override bool Equals(object obj)
        {
            return obj is PunchedCard<TKey, TInput> inputPunch &&
                   Key.Equals(inputPunch.Key) &&
                   Input.Equals(inputPunch.Input);
        }
    }
}