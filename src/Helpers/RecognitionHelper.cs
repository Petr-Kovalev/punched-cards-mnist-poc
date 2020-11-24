using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.Helpers
{
    internal static class RecognitionHelper
    {
        internal static IEnumerable<KeyValuePair<IReadOnlyList<bool>, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<IReadOnlyList<bool>, IReadOnlyList<bool>>> data,
            IDictionary<string, IDictionary<IReadOnlyList<bool>, IReadOnlyCollection<Tuple<IReadOnlyList<bool>, int>>>>
                punchedCardsCollection,
            IPuncher<string, IReadOnlyList<bool>, IReadOnlyList<bool>> puncher)
        {
            var correctRecognitionsPerLabel =
                new ConcurrentDictionary<IReadOnlyList<bool>, int>(BoolReadOnlyListEqualityComparer.Instance);

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
                    if (BoolReadOnlyListEqualityComparer.Instance.Equals(topLabel, dataItem.Item2))
                    {
                        correctRecognitionsPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            key => 1,
                            (key, value) => value + 1);
                    }
                });

            return correctRecognitionsPerLabel;
        }

        internal static IDictionary<IReadOnlyList<bool>, IDictionary<string, int>> CountCorrectRecognitionsPerLabelPerPunchedCard(
            IDictionary<string, IDictionary<IReadOnlyList<bool>, IReadOnlyCollection<Tuple<IReadOnlyList<bool>, int>>>> punchedCardsCollection,
            IReadOnlyList<bool> input,
            IPuncher<string, IReadOnlyList<bool>, IReadOnlyList<bool>> puncher)
        {
            var correctRecognitionsPerLabelPerPunchedCard = new Dictionary<IReadOnlyList<bool>, IDictionary<string, int>>(BoolReadOnlyListEqualityComparer.Instance);

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
            IDictionary<IReadOnlyList<bool>, IDictionary<string, int>> correctRecognitionsPerLabelPerPunchedCard,
            string punchedCardKey,
            KeyValuePair<IReadOnlyList<bool>, IReadOnlyCollection<Tuple<IReadOnlyList<bool>, int>>> label,
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

        internal static int CalculateMatchingScore(ICollection<int> inputOneIndices, Tuple<IReadOnlyList<bool>, int> punchedInput)
        {
            return inputOneIndices.Count(inputOneIndex => punchedInput.Item1[inputOneIndex]) * punchedInput.Item2;
        }

        internal static IEnumerable<int> GetOneIndices(IReadOnlyList<bool> input)
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