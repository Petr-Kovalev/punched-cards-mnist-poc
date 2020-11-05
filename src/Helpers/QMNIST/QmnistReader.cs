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

            int magicNumber = imagesReader.ReadBigInt32();
            int numberOfImages = imagesReader.ReadBigInt32();
            int width = imagesReader.ReadBigInt32();
            int height = imagesReader.ReadBigInt32();

            int magicLabel = labelsReader.ReadBigInt32();
            int numberOfLabels = labelsReader.ReadBigInt32();
            int labelsColumnCount = labelsReader.ReadBigInt32();

            for (int imageIndex = 0; imageIndex < numberOfImages; imageIndex++)
            {
                var bytes = imagesReader.ReadBytes(width * height);
                var data = new byte[height, width];
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        data[i, j] = bytes[i * height + j];
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