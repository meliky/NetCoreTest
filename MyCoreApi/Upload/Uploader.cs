using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyCoreApi.Upload
{
    public class Uploader
    {
        private static readonly string bucketName = "csharpbucket";
        private static readonly string accessKey = "minioadmin";
        private static readonly string secretKey = "minioadmin";
        public static async Task UploadStream(Stream stream)
        {

            var config = new AmazonS3Config
            {
                RegionEndpoint = RegionEndpoint.USEast1, // MUST set this before setting ServiceURL and it should match the `MINIO_REGION` environment variable.
                ServiceURL = "http://192.168.17.132:9000", // replace http://localhost:9000 with URL of your MinIO server
                ForcePathStyle = true // MUST be true to work correctly with MinIO server
            };

            var amazonS3Client = new AmazonS3Client(accessKey, secretKey, config);
            var putRequest = new PutObjectRequest
            {
                InputStream = stream,
                AutoResetStreamPosition = false,
                AutoCloseStream = false,
                
                BucketName = bucketName,
                Key = "newfile.gz"
                
            };
            var result = await amazonS3Client.PutObjectAsync(putRequest);
            if (result.HttpStatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new Exception($"received status code:{result.HttpStatusCode}");
            }
        }
    }
}
