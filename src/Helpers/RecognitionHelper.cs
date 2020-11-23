using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<BitArray, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<BitArray, BitArray>> data,
            IDictionary<string, IDictionary<BitArray, IReadOnlyCollection<Tuple<BitArray, int>>>>
                punchedCardsCollection,
            IPuncher<string, BitArray, BitArray> puncher)
        {
            var correctRecognitionsPerLabel =
                new ConcurrentDictionary<BitArray, int>(BitArrayEqualityComparer.Instance);

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerLabelPerPunchedCard =
                        CountCorrectRecognitionsPerLabelPerPunchedCard(punchedCardsCollection, dataItem.Item1, puncher);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(s => s.Value.Sum(v => v.Value))
                        .First()
                        .Key;
                    if (BitArrayEqualityComparer.Instance.Equals(topLabel, dataItem.Item2))
                    {
                        correctRecognitionsPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            key => 1,
                            (key, value) => value + 1);
                    }
                });

            return correctRecognitionsPerLabel;
        }

        internal static IDictionary<BitArray, IDictionary<string, int>> CountCorrectRecognitionsPerLabelPerPunchedCard(
            IDictionary<string, IDictionary<BitArray, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsCollection,
            BitArray input,
            IPuncher<string, BitArray, BitArray> puncher)
        {
            var correctRecognitionsPerLabelPerPunchedCard = new Dictionary<BitArray, IDictionary<string, int>>(BitArrayEqualityComparer.Instance);

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                var inputOneIndices = GetOneIndices(punchedInput).ToList();
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(correctRecognitionsPerLabelPerPunchedCard, punchedCardsCollectionItem.Key, label, inputOneIndices);
                }
            }

            return correctRecognitionsPerLabelPerPunchedCard;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<BitArray, IDictionary<string, int>> correctRecognitionsPerLabelPerPunchedCard,
            string punchedCardKey,
            KeyValuePair<BitArray, IReadOnlyCollection<Tuple<BitArray, int>>> label,
            ICollection<int> inputOneIndices)
        {
            var punchedCardCorrectRecognitionsPerLabel =
                label.Value.Sum(punchedInput => CalculateMatchingScore(inputOneIndices, punchedInput));

            if (!correctRecognitionsPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
            {
                dictionary = new Dictionary<string, int>();
                correctRecognitionsPerLabelPerPunchedCard[label.Key] = dictionary;
            }

            dictionary.Add(punchedCardKey, punchedCardCorrectRecognitionsPerLabel);
        }

        internal static int CalculateMatchingScore(ICollection<int> inputOneIndices, Tuple<BitArray, int> punchedInput)
        {
            return inputOneIndices.Count(inputOneIndex => punchedInput.Item1[inputOneIndex]) * punchedInput.Item2;
        }

        internal static IEnumerable<int> GetOneIndices(BitArray input)
        {
            for (var i = 0; i < input.Count; i++)
            {
                if (input[i])
                {
                    yield return i;
                }
            }
        }
    }
}