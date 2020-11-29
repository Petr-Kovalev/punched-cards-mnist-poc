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
                    var matchingScoresPerLabelPerPunchedCard =
                        CountCorrectRecognitionsPerLabelPerPunchedCard(punchedCardsCollection, dataItem.Item1, puncher);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(s => s.Value.Sum(v => v.Value))
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

        internal static IDictionary<IBitVector, IDictionary<string, int>>
            CountCorrectRecognitionsPerLabelPerPunchedCard(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
                    punchedCardsCollection,
                IBitVector input,
                IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var correctRecognitionsPerLabelPerPunchedCard = new Dictionary<IBitVector, IDictionary<string, int>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(correctRecognitionsPerLabelPerPunchedCard, punchedCardsCollectionItem.Key,
                        label, punchedInput);
                }
            }

            return correctRecognitionsPerLabelPerPunchedCard;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<IBitVector, IDictionary<string, int>> correctRecognitionsPerLabelPerPunchedCard,
            string punchedCardKey,
            KeyValuePair<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>> label,
            IBitVector punchedInput)
        {
            var punchedCardCorrectRecognitionsPerLabel =
                label.Value.Sum(punchedLabelInput =>
                    punchedInput.AndCardinality(punchedLabelInput.Item1) * punchedLabelInput.Item2);

            if (!correctRecognitionsPerLabelPerPunchedCard.TryGetValue(label.Key, out var dictionary))
            {
                dictionary = new Dictionary<string, int>();
                correctRecognitionsPerLabelPerPunchedCard[label.Key] = dictionary;
            }

            dictionary.Add(punchedCardKey, punchedCardCorrectRecognitionsPerLabel);
        }
    }
}