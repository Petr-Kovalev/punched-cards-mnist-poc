using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PunchedCards.Helpers.QMNIST;

namespace PunchedCards.Helpers
{
    internal static class DataHelper
    {
        internal const int LabelsCount = 10;

        internal static IEnumerable<Tuple<BitArray, BitArray>> ReadTrainingData()
        {
            return ReaData(QmnistReader.ReadTrainingData);
        }

        internal static IEnumerable<Tuple<BitArray, BitArray>> ReadTestData()
        {
            return ReaData(QmnistReader.ReadTestData);
        }

        private static IEnumerable<Tuple<BitArray, BitArray>> ReaData(Func<IEnumerable<Image>> readImagesFunction)
        {
            return readImagesFunction()
                .Select(image => new Tuple<BitArray, BitArray>(
                    GetValueBitArray(image.Data),
                    GetLabelBitArray(image.Label)));
        }

        internal static BitArray GetLabelBitArray(byte label)
        {
            return new BitArray(ByteToBooleanEnumerable(label).Skip(4).ToArray());
        }

        private static BitArray GetValueBitArray(byte[,] imageData)
        {
            const byte height = 28;
            const byte width = 28;
            const int pixelRepresentationSizeInBits = 8;

            var valueBitArray = new BitArray(height * width * pixelRepresentationSizeInBits);

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
                            valueBitArray[startIndex + bitIndex] = true;
                        }

                        bitIndex++;
                    }
                }
            }

            return valueBitArray;
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