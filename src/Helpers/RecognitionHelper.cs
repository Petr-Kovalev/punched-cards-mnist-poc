using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<IBitVector, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<IBitVector, IBitVector>> data,
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
                punchedCardsCollection,
            IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var correctRecognitionsPerLabel =
                new ConcurrentDictionary<IBitVector, int>();

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerLabelPerPunchedCard = CalculateMatchingScoresPerLabelPerPunchedCard(punchedCardsCollection, dataItem.Item1, puncher);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(p => p.Value.Sum(keyScore => keyScore.Value))
                        .First()
                        .Key;
                    if (topLabel.Equals(dataItem.Item2))
                    {
                        correctRecognitionsPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            key => 1,
                            (key, value) => value + 1);
                    }
                });

            return correctRecognitionsPerLabel;
        }

        internal static IDictionary<IBitVector, IDictionary<string, double>>
            CalculateMatchingScoresPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
                    punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var matchingScoresPerLabelPerPunchedCard = new Dictionary<IBitVector, IDictionary<string, double>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(matchingScoresPerLabelPerPunchedCard, punchedCardsCollectionItem.Key,
                        label, punchedInput);
                }
            }

            return matchingScoresPerLabelPerPunchedCard;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<IBitVector, IDictionary<string, double>> matchingScoresPerLabelPerPunchedCard,
            string punchedCardKey,
            KeyValuePair<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>> label,
            IBitVector punchedInput)
        {
            var matchingScorePerLabel = CalculateMatchingScore(punchedInput, label.Value);

            if (!matchingScoresPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
            {
                dictionary = new Dictionary<string, double>();
                matchingScoresPerLabelPerPunchedCard[label.Key] = dictionary;
            }

            dictionary.Add(punchedCardKey, matchingScorePerLabel);
        }

        private static double CalculateMatchingScore(IBitVector punchedInput,
            IEnumerable<Tuple<IBitVector, int>> labelPunchedInputs)
        {
            var matchingScoreSum = 0d;
            var count = 0;

            foreach (var labelPunchedInput in labelPunchedInputs)
            {
                matchingScoreSum += CalculateMatchingScore(punchedInput, labelPunchedInput.Item1) *
                                 labelPunchedInput.Item2;
                count += labelPunchedInput.Item2;
            }

            return matchingScoreSum / count;
        }

        internal static int CalculateBitVectorsScore(IReadOnlyCollection<Tuple<IBitVector, int>> bitVectors)
        {
            return bitVectors.Count;
        }

        private static double CalculateMatchingScore(IBitVector firstBitVector, IBitVector secondBitVector)
        {
            if (firstBitVector.Count != secondBitVector.Count)
            {
                throw new Exception("Counts does not match!");
            }

            return firstBitVector.AndCardinality(secondBitVector) - firstBitVector.XorCardinality(secondBitVector) / 10d;
        }
    }
}