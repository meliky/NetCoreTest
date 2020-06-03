using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading.Tasks;
using Minio;

namespace OctodiffTest
{
    class S3Connector
    {
        public static async Task UploadStream(Stream inputStream, string bucketName, string objectName)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = @"C:\Temp\mc.exe";
                process.StartInfo.Arguments = $"pipe local/{bucketName}/{objectName}";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardInput = true;

                process.Start();

                var stream = process.StandardInput.BaseStream;
                using (var gzip = new GZipStream(stream, CompressionMode.Compress))
                {
                    await inputStream.CopyToAsync(gzip).ConfigureAwait(false);
                }
            }
        }

    }
}
