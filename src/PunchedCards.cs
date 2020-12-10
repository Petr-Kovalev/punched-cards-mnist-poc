using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using PunchedCards.BitVectors;
using PunchedCards.Helpers;

namespace PunchedCards
{
    internal static class PunchedCards
    {
        private static readonly IBitVectorFactory BitVectorFactory = new BitVectorFactoryRoaringBitmap();

        private static void Main()
        {
            var trainingData = DataHelper.ReadTrainingData(BitVectorFactory).ToList();
            var testData = DataHelper.ReadTestData(BitVectorFactory).ToList();

            var punchedCardBitLengths = new[] {32, 64, 128, 256, 512, 1024, 2048, 4096};

            const bool enableTimeLogging = false;

            Stopwatch stopwatch;
            if (enableTimeLogging)
            {
                stopwatch = Stopwatch.StartNew();
            }

            foreach (var punchedCardBitLength in punchedCardBitLengths)
            {
                Console.WriteLine("Punched card bit length: " + punchedCardBitLength);

                IPuncher<string, IBitVector, IBitVector> puncher = new RandomPuncher(punchedCardBitLength, BitVectorFactory);
                var punchedCardsPerKeyPerLabel = GetPunchedCardsPerKeyPerLabel(trainingData, puncher);

                if (enableTimeLogging)
                {
                    Console.WriteLine($"Cards preparation time: {(int)stopwatch.Elapsed.TotalSeconds} seconds");
                    stopwatch.Restart();
                }

                Console.WriteLine();

                Console.WriteLine("Global top punched card:");
                WriteTrainingAndTestResults(GetGlobalTopPunchedCard(punchedCardsPerKeyPerLabel), trainingData, testData, puncher);
                if (enableTimeLogging)
                {
                    Console.WriteLine($"Calculation time: {(int)stopwatch.Elapsed.TotalSeconds} seconds");
                    stopwatch.Restart();
                }

                Console.WriteLine();

                Console.WriteLine("Top punched cards per label:");
                WriteTrainingAndTestResults(GetTopPunchedCardsPerLabel(punchedCardsPerKeyPerLabel, 1), trainingData, testData, puncher);
                if (enableTimeLogging)
                {
                    Console.WriteLine($"Calculation time: {(int)stopwatch.Elapsed.TotalSeconds} seconds");
                    stopwatch.Restart();
                }

                Console.WriteLine();
            }

            Console.WriteLine("Press \"Enter\" to exit the program...");
            Console.ReadLine();
        }

        private static void WriteTrainingAndTestResults(
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>> topPunchedCardsPerLabel,
            List<Tuple<IBitVector, IBitVector>> trainingData,
            List<Tuple<IBitVector, IBitVector>> testData,
            IPuncher<string, IBitVector, IBitVector> puncher)
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
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>> punchedCardsPerLabel)
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

        private static IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>> GetGlobalTopPunchedCard(
            IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>> punchedCardsPerKeyPerLabel)
        {
            var globalTopPunchedCard = punchedCardsPerKeyPerLabel
                .OrderByDescending(punchedCardPerKeyPerLabel =>
                    punchedCardPerKeyPerLabel.Value
                        .AsParallel()
                        .Sum(labelAndInputs => RecognitionHelper.GetBitVectorsRank(labelAndInputs.Value)))
                .First();
            return new Dictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
                {{globalTopPunchedCard.Key, globalTopPunchedCard.Value}};
        }

        private static IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
            GetTopPunchedCardsPerLabel(
                IDictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>> punchedCardsPerKeyPerLabel,
                int topPunchedCardsPerKeyPerLabelCount)
        {
            var topPunchedCardsPerKeyPerLabel =
                new Dictionary<string, IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>();

            for (byte i = 0; i < DataHelper.LabelsCount; i++)
            {
                var label = DataHelper.GetLabelBitVector(i, BitVectorFactory);

                var topPunchedCardsPerSpecificLabel = punchedCardsPerKeyPerLabel
                    .OrderByDescending(punchedCardPerLabel =>
                        RecognitionHelper.GetBitVectorsRank(punchedCardPerLabel.Value[label]))
                    .Take(topPunchedCardsPerKeyPerLabelCount);

                foreach (var topPunchedCardPerSpecificLabel in topPunchedCardsPerSpecificLabel)
                {
                    if (!topPunchedCardsPerKeyPerLabel.TryGetValue(topPunchedCardPerSpecificLabel.Key, out var dictionary))
                    {
                        dictionary = new Dictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>();
                        topPunchedCardsPerKeyPerLabel.Add(topPunchedCardPerSpecificLabel.Key, dictionary);
                    }

                    dictionary.Add(label, topPunchedCardPerSpecificLabel.Value[label]);
                }
            }

            return topPunchedCardsPerKeyPerLabel;
        }

        private static IDictionary<string,
                IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>
            GetPunchedCardsPerKeyPerLabel(
                IList<Tuple<IBitVector, IBitVector>> trainingData,
                IPuncher<string, IBitVector, IBitVector> puncher)
        {
            var count = trainingData[0].Item1.Count;

            var punchedCardsPerKeyPerLabel = new Dictionary<
                string,
                IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>>>();
            puncher
                .GetKeys(count)
                .AsParallel()
                .ForAll(key =>
                {
                    punchedCardsPerKeyPerLabel.Add(key,
                        GetPunchedCardsPerLabel(trainingData.Select(trainingDataItem =>
                            new Tuple<IPunchedCard<string, IBitVector>, IBitVector>(
                                puncher.Punch(key, trainingDataItem.Item1), trainingDataItem.Item2))));
                });
            return punchedCardsPerKeyPerLabel;
        }

        private static IDictionary<IBitVector, IReadOnlyCollection<Tuple<IBitVector, int>>> GetPunchedCardsPerLabel(
            IEnumerable<Tuple<IPunchedCard<string, IBitVector>, IBitVector>> punchedCardInputsByKeyGrouping)
        {
            return punchedCardInputsByKeyGrouping
                .GroupBy(punchedCardAndLabel => punchedCardAndLabel.Item2)
                .ToDictionary(
                    punchedCardByLabelGrouping => punchedCardByLabelGrouping.Key,
                    punchedCardByLabelGrouping =>
                        GetUniquePunchedCardInputs(punchedCardByLabelGrouping
                            .Select(punchedCardAndLabel => punchedCardAndLabel.Item1.Input)));
        }

        private static IReadOnlyCollection<Tuple<IBitVector, int>> GetUniquePunchedCardInputs(IEnumerable<IBitVector> punchedCardInputs)
        {
            return punchedCardInputs
                .GroupBy(punchedCardInput => punchedCardInput)
                .Select(punchedCardInputsGrouping =>
                    new Tuple<IBitVector, int>(
                        punchedCardInputsGrouping.Key,
                        punchedCardInputsGrouping.Count()))
                .OrderByDescending(uniqueInputAndCount => uniqueInputAndCount.Item2)
                .ToList();
        }
    }
}