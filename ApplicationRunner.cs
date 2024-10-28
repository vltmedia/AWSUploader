using Amazon;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSUploader
{
    public class ApplicationRunner
    {
        public S3FilesUploader s3FilesUploader = null;
       
        public ApplicationRunner(string[] args)
        {
            s3FilesUploader = new S3FilesUploader(args);
            if(s3FilesUploader.aWSUploaderArguments.function == "save")
            {
                s3FilesUploader.aWSUploaderArguments.Save();
                Environment.Exit(0);
            }
        }

        public async Task RunUpload()
        {
            await s3FilesUploader.UploadAsync();
        }
    }
    }
