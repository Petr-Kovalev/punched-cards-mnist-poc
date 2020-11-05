namespace PunchedCards
{
    internal interface IPunchedCard<out TKey, out TInput>
    {
        TKey Key { get; }

        TInput Input { get; }
    }
}