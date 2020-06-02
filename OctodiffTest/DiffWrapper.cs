using Octodiff.Core;
using Octodiff.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OctodiffTest
{
    public static class DiffWrapper
    {
        public static void CreateSignature(string signaturePath, string oldFilePath)
        {
            var signatureBuilder = new SignatureBuilder();
            using (var basisStream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var signatureStream = new FileStream(signaturePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                signatureBuilder.Build(basisStream, new SignatureWriter(signatureStream));
            }

        }

        public static void CreateDelta(string deltaFilePath, string oldFileSignaturePath, string newFilePath)
        {
            var deltaBuilder = new DeltaBuilder();
            using (var newFileStream = new FileStream(newFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var signatureFileStream = new FileStream(oldFileSignaturePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                deltaBuilder.BuildDelta(newFileStream, new SignatureReader(signatureFileStream, new ProgReport()), new AggregateCopyOperationsDecorator(new BinaryDeltaWriter(deltaStream)));
            }
        }

        public static void CreateNewFileFromDelta(string newFilePath, string oldFilePath, string deltaFilePath)
        {
            var deltaApplier = new DeltaApplier { SkipHashCheck = false };
            using (var basisStream = new FileStream(oldFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var deltaStream = new FileStream(deltaFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (var newFileStream = new FileStream(newFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
            {
                deltaApplier.Apply(basisStream, new BinaryDeltaReader(deltaStream, new ProgReport()), newFileStream);
            }

        }
    }
    class ProgReport : IProgressReporter
    {
        public void ReportProgress(string operation, long currentPosition, long total)
        {
        }
    }
}
