using System;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Threading.Tasks;

namespace OctodiffTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            PipeTest().Wait();
            Console.ReadLine();
        }

        public static async Task PipeTest()
        {
            var pipe = new Pipe();
            using (var infile = new FileStream(@"C:\Temp\infile", FileMode.Open))
            using (var pipeReader = pipe.Reader.AsStream())
            {
                var task = WriteToFile(pipeReader);
                using (var pipeWriter = pipe.Writer.AsStream())
                using (var gzip = new GZipStream(pipeWriter, CompressionMode.Compress))
                {
                    await infile.CopyToAsync(gzip);

                }
                await task;
            }
        }

        public static async Task WriteToFile(Stream file)
        {
            using (var outfile = new FileStream(@"C:\Temp\outfile", FileMode.Create))
            {
                await file.CopyToAsync(outfile);
            }
        }
    }
}
