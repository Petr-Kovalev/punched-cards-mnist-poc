using System;
using System.Collections.Generic;
using System.IO;

namespace PunchedCards.Helpers.QMNIST
{
    // https://github.com/facebookresearch/qmnist
    internal static class QmnistReader
    {
        private const string TrainImagesFileName = "qmnist/qmnist-train-images-idx3-ubyte";
        private const string TrainLabelsFileName = "qmnist/qmnist-train-labels-idx2-int";
        private const string TestImagesFileName = "qmnist/qmnist-test-images-idx3-ubyte";
        private const string TestLabelsFileName = "qmnist/qmnist-test-labels-idx2-int";

        internal static IEnumerable<Image> ReadTrainingData()
        {
            return Read(TrainImagesFileName, TrainLabelsFileName);
        }

        internal static IEnumerable<Image> ReadTestData()
        {
            return Read(TestImagesFileName, TestLabelsFileName);
        }

        private static IEnumerable<Image> Read(string imagesPath, string labelsPath)
        {
            using var labelsFileStream = File.OpenRead(labelsPath);
            using var labelsReader = new BinaryReader(labelsFileStream);
            using var imagesFileStream = File.OpenRead(imagesPath);
            using var imagesReader = new BinaryReader(imagesFileStream);

            var magicNumber = imagesReader.ReadBigInt32();
            var numberOfImages = imagesReader.ReadBigInt32();
            var width = imagesReader.ReadBigInt32();
            var height = imagesReader.ReadBigInt32();

            var magicLabel = labelsReader.ReadBigInt32();
            var numberOfLabels = labelsReader.ReadBigInt32();
            var labelsColumnCount = labelsReader.ReadBigInt32();

            for (var imageIndex = 0; imageIndex < numberOfImages; imageIndex++)
            {
                var bytes = imagesReader.ReadBytes(height * width);
                var data = new byte[height, width];
                for (var rowIndex = 0; rowIndex < height; rowIndex++)
                {
                    for (var columnIndex = 0; columnIndex < width; columnIndex++)
                    {
                        data[rowIndex, columnIndex] = bytes[rowIndex * width + columnIndex];
                    }
                }

                var label = (byte) labelsReader.ReadBigInt32();

                //Read extra bytes for QMNIST
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();
                labelsReader.ReadBigInt32();

                yield return new Image
                {
                    Data = data,
                    Label = label
                };
            }
        }

        private static int ReadBigInt32(this BinaryReader br)
        {
            var bytes = br.ReadBytes(sizeof(int));
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            return BitConverter.ToInt32(bytes, 0);
        }
    }
}