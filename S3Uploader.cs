using Amazon.S3.Transfer;
using Amazon.S3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSUploader
{
    public class S3DirectoryUploader
    {
        private readonly IAmazonS3 _s3Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3DirectoryUploader"/> class.
        /// </summary>
        /// <param name="s3Client">The S3 client.</param>
        public S3DirectoryUploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        /// <summary>
        /// Uploads a directory to the specified S3 bucket.
        /// </summary>
        /// <param name="localDirectory">The local directory to upload.</param>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="targetPath">The target path in the S3 bucket.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadDirectoryAsync(string localDirectory, string bucketName, string targetPath)
        {
            var transferUtility = new TransferUtility(_s3Client);

            foreach (var filePath in Directory.GetFiles(localDirectory, "*.*", SearchOption.AllDirectories))
            {
                var relativePath = Path.GetRelativePath(localDirectory, filePath);
                var s3Key = Path.Combine(targetPath, relativePath).Replace("\\", "/");

                Console.WriteLine($"Uploading {filePath} to s3://{bucketName}/{s3Key}");

                await transferUtility.UploadAsync(filePath, bucketName, s3Key);
            }
        }
    }

    public class S3FileUploader
    {
        private readonly IAmazonS3 _s3Client;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3FileUploader"/> class.
        /// </summary>
        /// <param name="s3Client">The S3 client.</param>
        public S3FileUploader(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        /// <summary>
        /// Uploads a file to the specified S3 bucket.
        /// </summary>
        /// <param name="localFile">The local file to upload.</param>
        /// <param name="bucketName">The name of the S3 bucket.</param>
        /// <param name="targetPath">The target path in the S3 bucket.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadFileAsync(string localFile, string bucketName, string targetPath)
        {
            var transferUtility = new TransferUtility(S3FilesUploader.Instance.aWSUploaderArguments.accessKey, S3FilesUploader.Instance.aWSUploaderArguments.secretKey, S3FilesUploader.Instance.aWSUploaderArguments.region);

            var s3Key = targetPath.Replace("\\", "/");

            Console.WriteLine($"Uploading {localFile} to s3://{bucketName}/{s3Key}");

            await transferUtility.UploadAsync(localFile, bucketName, s3Key);
        }
    }

    public class S3FilesUploader
    {
        public AWSUploaderArguments aWSUploaderArguments = null;
        public S3FileUploader s3FileUploader;
        public S3DirectoryUploader s3DirectoryUploader;
        private readonly IAmazonS3 _s3Client;
        public static S3FilesUploader Instance;

        /// <summary>
        /// Initializes a new instance of the <see cref="S3FilesUploader"/> class.
        /// </summary>
        /// <param name="args">The arguments for the uploader.</param>
        public S3FilesUploader(string[] args)
        {
            if (Instance == null)
            {
                Instance = this;

                aWSUploaderArguments = new AWSUploaderArguments(args);
                _s3Client = new AmazonS3Client(aWSUploaderArguments.accessKey, aWSUploaderArguments.secretKey, aWSUploaderArguments.region);
                s3FileUploader = new S3FileUploader(_s3Client);
                s3DirectoryUploader = new S3DirectoryUploader(_s3Client);
            }
        }

        /// <summary>
        /// Uploads data to S3 based on the specified upload type.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadDataAsync()
        {
            await UploadDataToUploadFiles();
            await UploadDataToUploadDirectory();
        }

        /// <summary>
        /// Uploads data to S3 based on the specified upload type.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadAsync()
        {
            switch (aWSUploaderArguments.uploadType)
            {
                case UploadType.Directory:
                    await UploadDirectoryAsync();
                    break;
                case UploadType.File:
                    await UploadFileAsync();
                    break;
                case UploadType.Multiple:
                    await UploadDataAsync();
                    break;
            }
        }

        /// <summary>
        /// Uploads directories to S3.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task UploadDataToUploadDirectory()
        {
            for (int i = 0; i < aWSUploaderArguments.dataToUpload.directories.Count; i++)
            {
                await s3DirectoryUploader.UploadDirectoryAsync(aWSUploaderArguments.dataToUpload.directories[i].path, aWSUploaderArguments.bucketName, aWSUploaderArguments.dataToUpload.directories[i].targetPath);
            }
        }

        /// <summary>
        /// Uploads files to S3.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        private async Task UploadDataToUploadFiles()
        {
            for (int i = 0; i < aWSUploaderArguments.dataToUpload.files.Count; i++)
            {
                await s3FileUploader.UploadFileAsync(aWSUploaderArguments.dataToUpload.files[i].path, aWSUploaderArguments.bucketName, aWSUploaderArguments.dataToUpload.files[i].targetPath);
            }
        }

        /// <summary>
        /// Uploads a single file to S3.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadFileAsync()
        {
            await s3FileUploader.UploadFileAsync(aWSUploaderArguments.localFile, aWSUploaderArguments.bucketName, aWSUploaderArguments.targetFile);
        }

        /// <summary>
        /// Uploads a single directory to S3.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task UploadDirectoryAsync()
        {
            await s3FileUploader.UploadFileAsync(aWSUploaderArguments.localFile, aWSUploaderArguments.bucketName, aWSUploaderArguments.targetFile);
        }
    }
}
