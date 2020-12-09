using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.BitVectors;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

        internal static IEnumerable<Tuple<IBitVector, IBitVector>> ReadTrainingData(IBitVectorFactory bitVectorFactory)
        {
            return ReaData(QmnistReader.ReadTrainingData, bitVectorFactory);
        }

        internal static IEnumerable<Tuple<IBitVector, IBitVector>> ReadTestData(IBitVectorFactory bitVectorFactory)
        {
            return ReaData(QmnistReader.ReadTestData, bitVectorFactory);
        }

        private static IEnumerable<Tuple<IBitVector, IBitVector>> ReaData(Func<IEnumerable<Image>> readImagesFunction, IBitVectorFactory bitVectorFactory)
        {
            return readImagesFunction()
                .Select(image => new Tuple<IBitVector, IBitVector>(
                    GetValueBitVector(image.Data, bitVectorFactory),
                    GetLabelBitVector(image.Label, bitVectorFactory)));
        }

        internal static IBitVector GetLabelBitVector(byte label, IBitVectorFactory bitVectorFactory)
        {
            return bitVectorFactory.Create(
                GetActiveBitIndices(label).Where(i => i >= 4).Select(i => i - 4),
                4);
        }

        private static IBitVector GetValueBitVector(byte[,] imageData, IBitVectorFactory bitVectorFactory)
        {
            const byte height = 28;
            const byte width = 28;
            const int pixelRepresentationSizeInBits = 8;

            return bitVectorFactory.Create(
                GetActiveBitIndices(imageData, height, width, pixelRepresentationSizeInBits),
                height * width * pixelRepresentationSizeInBits);
        }

        private static IEnumerable<int> GetActiveBitIndices(
            byte[,] imageData,
            int height,
            int width,
            int pixelRepresentationSizeInBits)
        {
            for (byte rowIndex = 0; rowIndex < height; rowIndex++)
            {
                for (byte columnIndex = 0; columnIndex < width; columnIndex++)
                {
                    var startIndex = (rowIndex * width + columnIndex) * pixelRepresentationSizeInBits;
                    foreach (var activeBitIndex in GetActiveBitIndices(imageData[rowIndex, columnIndex]))
                    {
                        yield return startIndex + activeBitIndex;
                    }
                }
            }
        }

        private static IEnumerable<int> GetActiveBitIndices(byte b)
        {
            if ((b & 128) != 0)
            {
                yield return 0;
            }

            if ((b & 64) != 0)
            {
                yield return 1;
            }

            if ((b & 32) != 0)
            {
                yield return 2;
            }

            if ((b & 16) != 0)
            {
                yield return 3;
            }

            if ((b & 8) != 0)
            {
                yield return 4;
            }

            if ((b & 4) != 0)
            {
                yield return 5;
            }

            if ((b & 2) != 0)
            {
                yield return 6;
            }

            if ((b & 1) != 0)
            {
                yield return 7;
            }
        }
    }
}