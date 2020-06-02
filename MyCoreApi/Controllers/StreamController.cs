using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyCoreApi.Upload;

namespace MyCoreApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class StreamController : ControllerBase
    {
        [HttpGet]
        public Stream Get()
        {
            using (var file = new FileStream(@"C:\Temp\deneme.txt", FileMode.Open, FileAccess.Read))
            using (var dest = new FileStream(@"C:\Temp\deneme.txt.gz", FileMode.Create, FileAccess.Write))
            using (var zip = new GZipStream(dest, CompressionLevel.Optimal))
            {
                file.CopyTo(zip);
            }
            return new GZipStream(new FileStream(@"C:\Temp\deneme.txt.gz", FileMode.Open, FileAccess.Read), CompressionMode.Decompress);
        }

        [HttpPost]
        public async Task<string> Post([FromQuery]int length)
        {
            var pipe = new Pipe();
            using(var pipeReader = pipe.Reader.AsStream())
            {
                var task = Uploader.UploadStream(pipeReader);
                using (var pipeWriter = pipe.Writer.AsStream())
                using (var gzip = new GZipStream(pipeWriter, CompressionMode.Compress))
                {
                    await Request.Body.CopyToAsync(gzip);
                }
                await task;
            }
            return "OK";
        }


    }
}
