using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers;

namespace PunchedCards
{
    internal static class PunchedCards
    {
        private static void Main()
        {
            var trainingData = DataHelper.ReadTrainingData().ToList();
            var testData = DataHelper.ReadTestData().ToList();

            var punchedCardBitLengths = new[] {32, 64, 128, 256, 512, 1024, 2048, 4096};
            foreach (var punchedCardBitLength in punchedCardBitLengths)
            {
                Console.WriteLine("Punched card bit length: " + punchedCardBitLength);

                var puncher = new RandomPuncher(punchedCardBitLength);
                var punchedCardsPerLabel = GetPunchedCardsPerLabel(trainingData, puncher);

                Console.WriteLine();
                Console.WriteLine("Global top punched card:");
                WriteTrainingAndTestResults(GetGlobalTopPunchedCard(punchedCardsPerLabel), trainingData, testData, puncher);
                Console.WriteLine();

                Console.WriteLine("Top punched cards per label:");
                WriteTrainingAndTestResults(GetTopPunchedCardsPerLabel(punchedCardsPerLabel, 1), trainingData, testData, puncher);
                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static void WriteTrainingAndTestResults(IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> topPunchedCardsPerLabel, List<Tuple<BitArray, string>> trainingData, List<Tuple<BitArray, string>> testData,
            RandomPuncher puncher)
        {
            Console.WriteLine("Unique input combinations per punched card (descending): " +
                              GetPunchedCardsPerLabelString(topPunchedCardsPerLabel));

            var trainingCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(trainingData, topPunchedCardsPerLabel, puncher);
            Console.WriteLine("Training results: " +
                              trainingCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + trainingData.Count);

            var testCorrectRecognitionsPerLabel =
                RecognitionHelper.CountCorrectRecognitions(testData, topPunchedCardsPerLabel, puncher);
            Console.WriteLine("Test results: " +
                              testCorrectRecognitionsPerLabel
                                  .Sum(correctRecognitionsPerLabel => correctRecognitionsPerLabel.Value) +
                              " correct recognitions of " + testData.Count);
        }

        private static string GetPunchedCardsPerLabelString(
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsPerLabel)
        {
            var punchedCardsPerLabelUniqueLookupCounts = punchedCardsPerLabel
                .Select(punchedCardPerLabel =>
                    new Tuple<IReadOnlyCollection<int>, int>(
                        punchedCardPerLabel.Value
                            .Select(punchedCard => punchedCard.Value.Count)
                            .OrderByDescending(count => count)
                            .ToList(),
                        punchedCardPerLabel.Value.Sum(punchedCard => punchedCard.Value.Count)))
                .OrderByDescending(countsAndSum => countsAndSum.Item2)
                .ToList();
            return string.Join(", ",
                       punchedCardsPerLabelUniqueLookupCounts.Select(uniqueLookupCounts =>
                           $"{{{GetUniqueLookupsCountsString(uniqueLookupCounts)}}}")) + ": total sum " +
                   punchedCardsPerLabelUniqueLookupCounts.Sum(uniqueLookupCounts => uniqueLookupCounts.Item2);
        }

        private static string GetUniqueLookupsCountsString(Tuple<IReadOnlyCollection<int>, int> uniqueLookupCounts)
        {
            var valuesString = string.Join(", ", uniqueLookupCounts.Item1);
            return uniqueLookupCounts.Item1.Count <= 1
                ? valuesString
                : valuesString + ": sum " + uniqueLookupCounts.Item2;
        }

        private static IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> GetGlobalTopPunchedCard(
            IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsPerLabel)
        {
            var globalTopPunchedCard = punchedCardsPerLabel
                .OrderByDescending(punchedCardPerLabel =>
                    punchedCardPerLabel
                        .Value
                        .Sum(labelAndInputs => labelAndInputs.Value.Count))
                .First();
            return new Dictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>>
                {{globalTopPunchedCard.Key, globalTopPunchedCard.Value}};
        }

        private static IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>>
            GetTopPunchedCardsPerLabel(
                IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>> punchedCardsPerLabel,
                int topPunchedCardsPerLabelCount)
        {
            var topPunchedCardsPerLabel =
                new Dictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>>();

            for (byte i = 0; i < DataHelper.LabelsCount; i++)
            {
                var label = BinaryStringsHelper.GetLabelString(i, DataHelper.LabelsCount);

                var topPunchedCardsPerSpecificLabel = punchedCardsPerLabel
                    .OrderByDescending(punchedCardPerLabel => punchedCardPerLabel.Value[label].Count)
                    .Take(topPunchedCardsPerLabelCount);

                foreach (var topPunchedCardPerSpecificLabel in topPunchedCardsPerSpecificLabel)
                {
                    if (!topPunchedCardsPerLabel.TryGetValue(topPunchedCardPerSpecificLabel.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>();
                        topPunchedCardsPerLabel.Add(topPunchedCardPerSpecificLabel.Key, dictionary);
                    }

                    dictionary.Add(label, topPunchedCardPerSpecificLabel.Value[label]);
                }
            }

            return topPunchedCardsPerLabel;
        }

        private static IDictionary<string, IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>>
            GetPunchedCardsPerLabel(
                IEnumerable<Tuple<BitArray, string>> trainingData,
                IPuncher<string, BitArray, BitArray> puncher)
        {
            return trainingData
                .SelectMany(trainingDataItem =>
                    puncher.GetInputPunches(trainingDataItem.Item1).Select(inputPunch =>
                        new Tuple<IPunchedCard<string, BitArray>, string>(inputPunch, trainingDataItem.Item2)))
                .GroupBy(punchedCardAndLabel => punchedCardAndLabel.Item1.Key)
                .ToDictionary(
                    punchedCardByKeyGrouping => punchedCardByKeyGrouping.Key,
                    punchedCardByKeyGrouping =>
                        (IDictionary<string, IReadOnlyCollection<Tuple<BitArray, int>>>) punchedCardByKeyGrouping
                            .GroupBy(punchedCardAndLabel => punchedCardAndLabel.Item2)
                            .ToDictionary(
                                punchedCardByLabelGrouping => punchedCardByLabelGrouping.Key,
                                punchedCardByLabelGrouping =>
                                    (IReadOnlyCollection<Tuple<BitArray, int>>) punchedCardByLabelGrouping
                                        .Select(punchedCardAndLabel => punchedCardAndLabel.Item1.Input)
                                        .GroupBy(punchedCardInput => punchedCardInput, BitArrayEqualityComparer.Instance)
                                        .Select(punchedCardInputsGrouping =>
                                            new Tuple<BitArray, int>(punchedCardInputsGrouping.Key,
                                                punchedCardInputsGrouping.Count()))
                                        .OrderByDescending(uniqueInputAndCount => uniqueInputAndCount.Item2)
                                        .ToList()));
        }
    }
}