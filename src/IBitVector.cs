namespace PunchedCards
{
    internal interface IBitVector
    {
        int Count{ get; }

        int AndCardinality(IBitVector bitVector);

        bool this [int index] { get; }
    }
}