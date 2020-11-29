using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

        private static readonly IBitVectorFactory BitVectorFactory = new BitVectorFactoryRoaringBitmap();

        internal static IEnumerable<Tuple<IBitVector, IBitVector>> ReadTrainingData()
        {
            return ReaData(QmnistReader.ReadTrainingData);
        }

        internal static IEnumerable<Tuple<IBitVector, IBitVector>> ReadTestData()
        {
            return ReaData(QmnistReader.ReadTestData);
        }

        private static IEnumerable<Tuple<IBitVector, IBitVector>> ReaData(Func<IEnumerable<Image>> readImagesFunction)
        {
            return readImagesFunction()
                .Select(image => new Tuple<IBitVector, IBitVector>(
                    GetValueBitVector(image.Data),
                    GetLabelBitVector(image.Label)));
        }

        internal static IBitVector GetLabelBitVector(byte label)
        {
            return BitVectorFactory.Create(
                GetBitIndices(label).Where(i => i >= 4).Select(i => i - 4),
                4);
        }

        private static IBitVector GetValueBitVector(byte[,] imageData)
        {
            const byte height = 28;
            const byte width = 28;
            const int pixelRepresentationSizeInBits = 8;

            return BitVectorFactory.Create(
                GetOneIndices(imageData, height, width, pixelRepresentationSizeInBits),
                height * width * pixelRepresentationSizeInBits);
        }

        private static IEnumerable<int> GetOneIndices(
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
                    foreach (var bitIndex in GetBitIndices(imageData[rowIndex, columnIndex]))
                    {
                        yield return startIndex + bitIndex;
                    }
                }
            }
        }

        private static IEnumerable<int> GetBitIndices(byte b)
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