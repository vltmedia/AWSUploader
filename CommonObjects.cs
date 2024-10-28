using Amazon.Runtime;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace AWSUploader
{
    [System.Serializable]
    public class AWSUploaderArguments
    {
        public UploadType uploadType = UploadType.File;
        public string accessKey = "";
        public string secretKey = "";
        public Amazon.RegionEndpoint region = Amazon.RegionEndpoint.USEast1;
        public string localDirectory = "";
        public string localFile = "";
        public string targetDirectory = "";
        public string targetFile = "";
        public string bucketName = "";
        public string savePath = "";
        public string function = "upload";
        public DataItemsToUpload dataToUpload;
        public bool isPublic = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="AWSUploaderArguments"/> class.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public AWSUploaderArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var key_ = args[i];
                switch (key_)
                {
                    case "--accesskey":
                        accessKey = args[++i];
                        break;
                    case "--secretkey":
                        secretKey = args[++i];
                        break;
                    case "--region":
                        region = ParseRegion(args[++i]);
                        break;
                    case "--function":
                        function = args[++i];
                        break;
                    case "--savePath":
                        savePath = args[++i];
                        break;
                    case "--directory":
                        uploadType = UploadType.Directory;
                        localDirectory = args[++i];
                        break;
                    case "--targetDirectory":
                        uploadType = UploadType.Directory;
                        targetDirectory = args[++i];
                        break;
                    case "--bucketName":
                        bucketName = args[++i];
                        break;
                    case "--public":
                        isPublic = true;
                        break;
                    case "--private":
                        isPublic = false;
                        break;
                    case "--file":
                        uploadType = UploadType.File;
                        localFile = args[++i];
                        break;
                    case "--targetFile":
                        uploadType = UploadType.File;
                        targetFile = args[++i];
                        break;
                    case "--data":
                        uploadType = UploadType.Multiple;
                        dataToUpload = DataItemsToUpload.LoadFromFile(args[++i]);
                        break;

                    case "--config":
                        var configString = args[++i];
                        Load(configString);
                        break;

                    case "--help":
                        Console.WriteLine("Usage: AWSUploader --accesskey <accesskey> --secretkey <secretkey> --region <region> --directory <localDirectory> --targetDirectory <targetDirectory> --bucketName <bucketName> [--public | --private] [--file <localFile> --targetFile <targetFile>]");
                        return;
                    default:
                        Console.WriteLine("Unknown argument: " + args[i]);
                        break;
                }
            }

        }

        private void Load(string configString)
        {
            AWSUploaderArguments config = JsonConvert.DeserializeObject<AWSUploaderArguments>(File.ReadAllText(configString));
            if (config != null)
            {
                Clone(config);
            }
        }

        public void Save(string path)
        {
            string jsonData = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(path, jsonData);

        }
        public void Save()
        {
            Save(savePath);

        }
        public void Clone(AWSUploaderArguments config)
        {
            accessKey = config.accessKey;
            secretKey = config.secretKey;
            region = config.region;
            function = config.function;
            localDirectory = config.localDirectory;
            localFile = config.localFile;
            targetDirectory = config.targetDirectory;
            targetFile = config.targetFile;
            bucketName = config.bucketName;
            isPublic = config.isPublic;
        }

        /// <summary>
        /// Parses the region from a string.
        /// </summary>
        /// <param name="region_">The region string.</param>
        /// <returns>The parsed <see cref="Amazon.RegionEndpoint"/>.</returns>
        public Amazon.RegionEndpoint ParseRegion(string region_ = "us-east-2")
        {
            switch (region_)
            {
                case "us-east-1":
                    return Amazon.RegionEndpoint.USEast1;
                case "us-east-2":
                    return Amazon.RegionEndpoint.USEast2;
                case "us-west-1":
                    return Amazon.RegionEndpoint.USWest1;
                case "us-west-2":
                    return Amazon.RegionEndpoint.USWest2;
                case "ap-east-1":
                    return Amazon.RegionEndpoint.APEast1;
                case "ap-south-1":
                    return Amazon.RegionEndpoint.APSouth1;
                case "ap-southeast-1":
                    return Amazon.RegionEndpoint.APSoutheast1;
                case "ap-southeast-2":
                    return Amazon.RegionEndpoint.APSoutheast2;
                case "ap-northeast-1":
                    return Amazon.RegionEndpoint.APNortheast1;
                case "ap-northeast-2":
                    return Amazon.RegionEndpoint.APNortheast2;
                case "ca-central-1":
                    return Amazon.RegionEndpoint.CACentral1;
                case "cn-north-1":
                    return Amazon.RegionEndpoint.CNNorth1;
                case "cn-northwest-1":
                    return Amazon.RegionEndpoint.CNNorthWest1;
                case "eu-central-1":
                    return Amazon.RegionEndpoint.EUCentral1;
                case "eu-west-1":
                    return Amazon.RegionEndpoint.EUWest1;
                case "eu-west-2":
                    return Amazon.RegionEndpoint.EUWest2;
                case "eu-west-3":
                    return Amazon.RegionEndpoint.EUWest3;
                case "eu-north-1":
                    return Amazon.RegionEndpoint.EUNorth1;
                case "sa-east-1":
                    return Amazon.RegionEndpoint.SAEast1;
                case "us-gov-east-1":
                    return Amazon.RegionEndpoint.USGovCloudEast1;
                case "us-gov-west-1":
                    return Amazon.RegionEndpoint.USGovCloudWest1;
                default:
                    return Amazon.RegionEndpoint.USEast1;
            }
        }
    }

    [System.Serializable]
    public class DataToUpload
    {
        public string path = "";
        public string targetPath = "";
        public bool isPublic = true;
    }

    [System.Serializable]
    public class DataItemsToUpload
    {
        public List<DataToUpload> files = new List<DataToUpload>();
        public List<DataToUpload> directories = new List<DataToUpload>();

        /// <summary>
        /// Saves this instance to a file.
        /// </summary>
        /// <param name="filePath">The file path to save to.</param>
        public void SaveToFile(string filePath)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                Console.WriteLine($"Data saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving data: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads an instance from a file.
        /// </summary>
        /// <param name="filePath">The file path to load from.</param>
        /// <returns>The loaded <see cref="DataItemsToUpload"/> instance.</returns>
        public static DataItemsToUpload LoadFromFile(string filePath)
        {
            try
            {
                if (!File.Exists(filePath))
                {
                    Console.WriteLine($"File not found: {filePath}");
                    return null;
                }

                string jsonData = File.ReadAllText(filePath);
                var dataItems = JsonConvert.DeserializeObject<DataItemsToUpload>(jsonData);
                Console.WriteLine($"Data loaded from {filePath}");
                return dataItems;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading data: {ex.Message}");
                return null;
            }
        }
    }

    [System.Serializable]
    public enum UploadType
    {
        File,
        Directory,
        Multiple
    }
}
