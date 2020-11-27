using System;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

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
                    GetValueBitArray(image.Data),
                    GetLabelBitArray(image.Label)));
        }

        internal static IBitVector GetLabelBitArray(byte label)
        {
            return new BitVector(ByteToBooleanEnumerable(label).Skip(4));
        }

        private static IBitVector GetValueBitArray(byte[,] imageData)
        {
            const byte height = 28;
            const byte width = 28;
            const int pixelRepresentationSizeInBits = 8;

            return new BitVector(
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

                    byte bitIndex = 0;
                    foreach (var bit in ByteToBooleanEnumerable(imageData[rowIndex, columnIndex]))
                    {
                        if (bit)
                        {
                            yield return startIndex + bitIndex;
                        }

                        bitIndex++;
                    }
                }
            }
        }

        private static IEnumerable<bool> ByteToBooleanEnumerable(byte b)
        {
            yield return (b & 128) != 0;
            yield return (b & 64) != 0;
            yield return (b & 32) != 0;
            yield return (b & 16) != 0;
            yield return (b & 8) != 0;
            yield return (b & 4) != 0;
            yield return (b & 2) != 0;
            yield return (b & 1) != 0;
        }
    }
}