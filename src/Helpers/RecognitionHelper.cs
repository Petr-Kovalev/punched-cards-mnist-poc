using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<string, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<BitArray, string>> data,
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsCollection,
            IPuncher<string, BitArray, BitArray> puncher)
        {
            var correctRecognitionsPerLabel = new ConcurrentDictionary<string, int>();

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerLabelPerPunchedCard = CountCorrectRecognitionsPerLabelPerPunchedCard(punchedCardsCollection, dataItem.Item1, puncher);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(s => s.Value.Sum(v => v.Value))
                        .First()
                        .Key;
                    if (topLabel == dataItem.Item2)
                    {
                        correctRecognitionsPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            key => 1,
                            (key, value) => value + 1);
                    }
                });

            return correctRecognitionsPerLabel;
        }

        internal static IDictionary<string, IDictionary<string, int>> CountCorrectRecognitionsPerLabelPerPunchedCard(
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsCollection,
            BitArray input,
            IPuncher<string, BitArray, BitArray> puncher)
        {
            var correctRecognitionsPerLabelPerPunchedCard = new Dictionary<string, IDictionary<string, int>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                var inputOneIndices = BinaryStringsHelper.GetOneIndices(punchedInput).ToList();
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(correctRecognitionsPerLabelPerPunchedCard, punchedCardsCollectionItem.Key, label, inputOneIndices);
                }
            }

            return correctRecognitionsPerLabelPerPunchedCard;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<string, IDictionary<string, int>> correctRecognitionsPerLabelPerPunchedCard,
            string punchedCardKey, 
            KeyValuePair<string, IReadOnlyCollection<Tuple<BitArray, int>>> label, 
            ICollection<int> inputOneIndices)
        {
            var punchedCardCorrectRecognitionsPerLabel = label.Value.Sum(punchedInput =>
                BinaryStringsHelper.CalculateMatchingScore(inputOneIndices, punchedInput));

            if (!correctRecognitionsPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
            {
                dictionary = new Dictionary<string, int>();
                correctRecognitionsPerLabelPerPunchedCard[label.Key] = dictionary;
            }

            dictionary.Add(punchedCardKey, punchedCardCorrectRecognitionsPerLabel);
        }
    }
}