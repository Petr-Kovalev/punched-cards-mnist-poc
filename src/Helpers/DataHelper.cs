using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

        internal static IEnumerable<Tuple<string, string>> ReadTrainingData()
        {
            return ReaData(QmnistReader.ReadTrainingData);
        }

        internal static IEnumerable<Tuple<string, string>> ReadTestData()
        {
            return ReaData(QmnistReader.ReadTestData);
        }

        private static IEnumerable<Tuple<string, string>> ReaData(Func<IEnumerable<Image>> readImagesFunction)
        {
            return readImagesFunction()
                .Select(image => new Tuple<string, string>(
                    BinaryStringsHelper.GetValueString(image.Data),
                    BinaryStringsHelper.GetLabelString(image.Label, LabelsCount)));
        }
    }
}