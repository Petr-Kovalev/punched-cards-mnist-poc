using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PunchedCards.Helpers
{
    internal static class LookupHelper
    {
        internal static IEnumerable<KeyValuePair<string, int>> CountCorrectRecognitions(
            IEnumerable<Tuple<string, string>> data,
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<string, int>>>> punchedCardsCollection,
            IPuncher<string, string, string> puncher)
        {
            var matchesPerLabel = new ConcurrentDictionary<string, int>();

            data
                .AsParallel()
                .ForAll(dataItem =>
                {
                    var matchingScoresPerLabelPerPunchedCard = CalculateMatchingScoresPerLabelPerPunchedCard(punchedCardsCollection, dataItem.Item1, puncher);
                    var topLabel = matchingScoresPerLabelPerPunchedCard
                        .OrderByDescending(s => s.Value.Sum(v => v.Value))
                        .First()
                        .Key;
                    if (topLabel == dataItem.Item2)
                    {
                        matchesPerLabel.AddOrUpdate(
                            dataItem.Item2,
                            key => 1,
                            (key, value) => value + 1);
                    }
                });

            return matchesPerLabel;
        }

        internal static IDictionary<string, IDictionary<string, int>> CalculateMatchingScoresPerLabelPerPunchedCard(
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<string, int>>>> punchedCardsCollection, 
            string input,
            IPuncher<string, string, string> puncher)
        {
            var matchingScores = new Dictionary<string, IDictionary<string, int>>();

            foreach (var punchedCardsCollectionItem in punchedCardsCollection)
            {
                var punchedInput = puncher.Punch(punchedCardsCollectionItem.Key, input).Input;
                var inputOneIndices = BinaryStringsHelper.GetOneIndices(punchedInput).ToList();
                foreach (var label in punchedCardsCollectionItem.Value)
                {
                    ProcessTheSpecificLabel(matchingScores, punchedCardsCollectionItem.Key, label, inputOneIndices);
                }
            }

            return matchingScores;
        }

        private static void ProcessTheSpecificLabel(
            IDictionary<string, IDictionary<string, int>> matchingScores,
            string punchedCardKey, 
            KeyValuePair<string, IReadOnlyCollection<Tuple<string, int>>> label, 
            ICollection<int> inputOneIndices)
        {
            var punchedCardMatchingScorePerLabel = label.Value.Sum(punchInput =>
                BinaryStringsHelper.CalculateMatchingScore(inputOneIndices, punchInput));

            if (!matchingScores.TryGetValue(label.Key, out var dictionary))
            {
                dictionary = new Dictionary<string, int>();
                matchingScores[label.Key] = dictionary;
            }

            dictionary.Add(punchedCardKey, punchedCardMatchingScorePerLabel);
        }
    }
}